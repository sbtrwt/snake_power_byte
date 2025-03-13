using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using SnakePowerByte;
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
        public int segmentGap = 5;  // Increase this value for a larger gap

        // A history of head positions (most recent at index 0)
        private List<Vector3> positionHistory = new List<Vector3>();

        // Input system (assumes you have a generated PlayerInputAction class)
        private PlayerInputAction playerInput;
        private InputAction moveAction;

        private LevelGrid levelGrid;
        private PlayerLength playerLength;
        private void Awake()
        {
            // Initialize grid position from current world position.
            gridPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

            // Initialize the history with the starting position.
            positionHistory.Add(transform.position);

            playerInput = new PlayerInputAction();
            playerInput.Enable();
            moveAction = playerInput.Snake.Move;
            moveAction.performed += ctx => HandleInput(ctx.ReadValue<Vector2>());
            levelGrid = new LevelGrid(500, 500);
            playerLength = GetComponent<PlayerLength>();
        }

        private void OnDestroy()
        {
            playerInput?.Dispose();
        }

        private void Update()
        {
            // Only the owning client should process input and move the snake.
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
            // Update direction based on input.
            if (input.x > 0) direction = Direction.Right;
            else if (input.x < 0) direction = Direction.Left;
            else if (input.y > 0) direction = Direction.Up;
            else if (input.y < 0) direction = Direction.Down;
        }

        void MoveSnake()
        {
            // Save current head position before moving.
            Vector3 previousHeadPos = transform.position;

            // Calculate new grid position based on current direction.
            Vector2Int moveDir = GetDirectionVector();
            gridPosition += moveDir;
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            transform.position = new Vector3(gridPosition.x, gridPosition.y, transform.position.z);

            // Insert the new head position into the history at index 0.
            positionHistory.Insert(0, transform.position);

            // Limit the history length to what is needed.
            int maxHistoryCount = (bodySegments.Count + 1) * segmentGap;
            if (positionHistory.Count > maxHistoryCount)
            {
                positionHistory.RemoveAt(positionHistory.Count - 1);
            }

            // Update each body segment's position based on the historical positions.
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
            }
            return Vector2Int.zero;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // When colliding with a food object (tagged "Food"), request to eat it.
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
            FoodType foodType = FoodType.X; // default
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(foodObjectId, out NetworkObject foodObject))
            {
                foodType = foodObject.GetComponent<Food>().GetFoodType();
                foodObject.Despawn();
            }

            // Grow the snake (or add tail) if needed.
            if (IsServer)
            {
                if (playerLength != null)
                {
                    playerLength.AddLength(foodType);
                }
            }

            // Assume you represent food types as a letter ("X" or "O")
            string foodLetter = foodType.ToString().Substring(0, 1);

            // Get the SnakePowerManager component and add the food to the combo.
            SnakePowerManager powerManager = GetComponent<SnakePowerManager>();
            if (powerManager != null)
            {
                powerManager.AddFoodToCombo(foodLetter);
            }
        }



        // Called on the server to add a new body segment.
        void GrowSnake()
        {
            // Determine spawn position: use the last historical position if available.
            Vector3 spawnPos = transform.position;
            if (bodySegments.Count > 0)
            {
                int index = bodySegments.Count * segmentGap;
                if (index < positionHistory.Count)
                    spawnPos = positionHistory[index];
            }

            // Instantiate and spawn the new segment.
            //GameObject newSegment = Instantiate(bodyPrefab, spawnPos, Quaternion.identity);
            //NetworkObject netObj = newSegment.GetComponent<NetworkObject>();
            //netObj.Spawn();

            // Add the new segment to the list.
            //bodySegments.Add(newSegment.transform);
        }
        public override void OnNetworkSpawn()
        {
            // If this is the local player's snake, assign it as the camera target.
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