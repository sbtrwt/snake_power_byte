// SnakeView.cs
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
        private float moveTimer = 0f;
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

        // A history of head positions (most recent at index 0)
        private List<Vector3> positionHistory = new List<Vector3>();

        // Input system
        private PlayerInputAction playerInput;
        private InputAction moveAction;

        private LevelGrid levelGrid;
        private PlayerLength playerLength;
        private XPManager xpManager;
        private SnakePowerManager powerManager;

        private void Awake()
        {
            gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
            positionHistory.Add(transform.position);

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

        private void Update()
        {
            if (!IsOwner)
                return;

            moveTimer += Time.deltaTime;
            if (moveTimer >= moveInterval)
            {
                moveTimer = 0f;
                MoveSnake();
            }
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

        void MoveSnake()
        {
            Vector3 previousHeadPos = transform.position;
            Vector2Int moveDir = GetDirectionVector();
            gridPosition += moveDir;
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            transform.position = new Vector3(gridPosition.x, gridPosition.y, transform.position.z);
            positionHistory.Insert(0, transform.position);

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
            }
        }
    }
}
