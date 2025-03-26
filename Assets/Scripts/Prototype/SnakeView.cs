using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using SnakePowerByte.Level;

namespace SnakePowerByte.Prototype
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public class SnakeView : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Time in seconds between grid moves.")]
        public float moveInterval = 0.3f;

        public Vector2Int gridPosition;

        [Tooltip("Current moving direction.")]
        public Direction direction = Direction.Right;

        [Header("Body Settings")]
        [Tooltip("List of snake body segments.")]
        public List<Transform> bodySegments = new List<Transform>();

        [Tooltip("Prefab for new body segments (must have a NetworkObject).")]
        public GameObject bodyPrefab;

        [Header("Gap Settings")]
        [Tooltip("Number of grid moves between each body segment.")]
        public int segmentGap = 5;

        [SerializeField] private GameObject floatingHealthBarPrefab;
        // A history of head positions (if needed for body follow logic)
        private List<Vector3> positionHistory = new List<Vector3>();

        // Input system
        private PlayerInputAction playerInput;
        private InputAction moveAction;

        private LevelGrid levelGrid;
        private PlayerLength playerLength;
        private XPManager xpManager;
        private SnakePowerManager powerManager;

        // Variables for interpolation smoothing.
        private float moveAccumulator = 0f;
        private Vector3 previousGridPos;
        private Vector3 currentGridPos;
        public Material snakeMaterial;

        private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
        private void Awake()
        {
            gridPosition = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y));
            positionHistory.Add(transform.position);

            // Initialize previous and current grid positions.
            previousGridPos = transform.position;
            currentGridPos = transform.position;

            playerInput = new PlayerInputAction();
            playerInput.Enable();
            moveAction = playerInput.Snake.Move;
            moveAction.performed += ctx => HandleInput(ctx.ReadValue<Vector2>());

            levelGrid = new LevelGrid(500, 500);
            playerLength = GetComponent<PlayerLength>();
            xpManager = GetComponent<XPManager>();
            powerManager = GetComponent<SnakePowerManager>();
        }

        private void OnDestroy()
        {
            playerInput?.Dispose();
        }

        // Use FixedUpdate for discrete simulation.
        private void FixedUpdate()
        {
            if (!IsOwner)
                return;

            moveAccumulator += Time.fixedDeltaTime;

            while (moveAccumulator >= moveInterval)
            {
                // Store current state as previous.
                previousGridPos = currentGridPos;

                // Update grid position discretely.
                Vector3 previousHeadPos = transform.position;
                Vector2Int moveDir = GetDirectionVector();
                // Apply the scroll speed to the material
                if (snakeMaterial != null)
                    snakeMaterial.SetVector("_ScrollSpeed", new Vector2(moveDir.x * 5, moveDir.y * 5));
                gridPosition += moveDir;
                //gridPosition = levelGrid.ValidateGridPosition(gridPosition);

                // Set the new current grid position.
                currentGridPos = new Vector3(gridPosition.x, gridPosition.y, transform.position.z);

                // Update discrete simulation for body segments if needed.
                positionHistory.Insert(0, currentGridPos);
                int maxHistoryCount = (bodySegments.Count + 1) * segmentGap;
                if (positionHistory.Count > maxHistoryCount)
                {
                    positionHistory.RemoveAt(positionHistory.Count - 1);
                }
                for (int i = 0; i < bodySegments.Count; i++)
                {
                    int historyIndex = (i + 1) * segmentGap;
                    if (historyIndex < positionHistory.Count)
                    {
                        bodySegments[i].position = positionHistory[historyIndex];
                    }
                }

                moveAccumulator -= moveInterval;
            }
        }

        // In Update, interpolate between the previous and current discrete positions.
        private void Update()
        {
            if (!IsOwner)
                return;
            //if (IsServer)
            //{
                // Standard interpolation factor: (Time.time - Time.fixedTime) / Time.fixedDeltaTime.
                float alpha = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
                transform.position = Vector3.Lerp(previousGridPos, currentGridPos, alpha);
                networkPosition.Value = transform.position;
            //}
            // else if (IsClient)
            // {
            //     SyncClientPosition();
            // }
        }
        private void SyncClientPosition()
        {
            // Smoothly interpolate to the network position
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * 10f);

        }
        void HandleInput(Vector2 input)
        {
            if (input.x > 0)
                direction = Direction.Right;
            else if (input.x < 0)
                direction = Direction.Left;
            else if (input.y > 0)
                direction = Direction.Up;
            else if (input.y < 0)
                direction = Direction.Down;
        }

        Vector2Int GetDirectionVector()
        {
            switch (direction)
            {
                case Direction.Up: return new Vector2Int(0, 1);
                case Direction.Down: return new Vector2Int(0, -1);
                case Direction.Left: return new Vector2Int(-1, 0);
                case Direction.Right: return new Vector2Int(1, 0);
                default: return Vector2Int.zero;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Food"))
            {
                if (IsOwner)
                {
                    EatFoodServerRpc(collision.GetComponent<NetworkObject>().NetworkObjectId);
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void EatFoodServerRpc(ulong foodObjectId)
        {
            FoodType foodType = FoodType.X; // default food type
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(foodObjectId, out NetworkObject foodObject))
            {
                foodType = foodObject.GetComponent<Food>().GetFoodType();
                foodObject.Despawn();
            }

            if (playerLength != null)
            {
                playerLength.AddLength(foodType);
            }

            string foodLetter = foodType.ToString().Substring(0, 1);
            if (powerManager != null)
            {
                powerManager.AddFoodToCombo(foodLetter);
            }

            if (xpManager != null)
            {
                xpManager.AddExperience(10);
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
                if (cameraFollow != null)
                {
                    cameraFollow.target = transform;
                }
                AddFloatinHealthBar(gameObject);
            }
        }
        private void AddFloatinHealthBar(GameObject parent)
        {
            if (floatingHealthBarPrefab != null)
            {
                GameObject hb = Instantiate(floatingHealthBarPrefab);
                // Option A: set as child so it moves with the enemy.
                hb.transform.SetParent(parent.transform);
                FloatingHealthBar floatingBar = hb.GetComponent<FloatingHealthBar>();
                Health enemyHealth = parent.GetComponent<Health>();
                floatingBar.Initialize(parent.transform, enemyHealth);
                NetworkObject hbNetworkObject = hb.GetComponent<NetworkObject>();
                if(hbNetworkObject != null)
                hbNetworkObject.Spawn();

            }
        }
    }
}
