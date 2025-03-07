using System.Collections;
using System.Collections.Generic;
using SnakePowerByte.Level;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakePowerByte.Snake
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public class SnakeController
    {
        private const float MAX_SPEED = 100f;

        private PlayerInputAction _playerInput;
        private InputAction _move;
        private SnakeView _view;
        private Direction _direction;
        private Vector2Int _gridPosition;
        private float _gridMoveTimer;
        // Set grid move time based on MAX_SPEED (e.g., moving one grid unit per move)
        private float _gridMoveTimeMax = 1f / MAX_SPEED;
        private LevelGrid _levelGrid;
         // List for body segments
        private List<SnakePartController> _bodySegments = new List<SnakePartController>();
        // Store owner client id for network spawning of new segments
        private ulong _ownerClientId;
          // Reference to the snake configuration (includes segment prefab)
        private SnakeSO _snakeSO;
        // Constructors...
        public SnakeController(SnakeSO snakeSO, ulong ownerClientId)
        { 
            _snakeSO = snakeSO;
            _ownerClientId = ownerClientId;
            _view = GameObject.Instantiate(snakeSO.PrefabSnakeView);
            _view.Controller = this;
            var instanceNetworkObject = _view.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(ownerClientId);
            _playerInput = new PlayerInputAction();
            _playerInput.Enable();
            _levelGrid = new LevelGrid(200, 200);
            _gridMoveTimeMax = 1f / MAX_SPEED;

            // for (int i = 0; i < 30; i++)
            // {
            //     Grow();
            // }
        }

        public SnakeController(SnakeView view)
        {
            _view = view;
            _view.Controller = this;
            _playerInput = new PlayerInputAction();
            _playerInput.Enable();
            _levelGrid = new LevelGrid(200, 200);
        }

        public void Init()
        {
            _move = _playerInput.Snake.Move;
            _move.performed += ctx => OnMove();
            
        }

        public void Update()
        {
            HandleGridMovement();
        }

        public void OnMove()
        {
            Vector2 moveInput = _move.ReadValue<Vector2>();
            Debug.Log(moveInput);
            if (moveInput.x > 0)
                Move(Direction.Right);
            else if (moveInput.x < 0)
                Move(Direction.Left);
            else if (moveInput.y > 0)
                Move(Direction.Up);
            else if (moveInput.y < 0)
                Move(Direction.Down);
        }

        public void Move(Direction direction)
        {
            _direction = direction;
        }

        private Vector2Int GetMoveDirectionVector()
        {
            switch (_direction)
            {
                default:
                case Direction.Right: return new Vector2Int(1, 0);
                case Direction.Left: return new Vector2Int(-1, 0);
                case Direction.Up: return new Vector2Int(0, 1);
                case Direction.Down: return new Vector2Int(0, -1);
            }
        }

        private float GetAngleFromVector(Vector2Int dir)
        {
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }

        private void HandleGridMovement()
        {
            _gridMoveTimer += Time.deltaTime;
            if (_gridMoveTimer > _gridMoveTimeMax)
            {  // Save the current head position for updating segments
                Vector3 previousHeadPosition = _view.transform.position;

                Vector2Int gridMoveDirectionVector = Vector2Int.RoundToInt(GetMoveDirectionVector());
                _gridPosition += gridMoveDirectionVector;
                _gridPosition = _levelGrid.ValidateGridPosition(_gridPosition);
                _gridMoveTimer -= _gridMoveTimeMax;

                _view.transform.position = new Vector3(_gridPosition.x, _gridPosition.y);
                _view.transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);

                // Update body segments positions:
                if (_bodySegments.Count > 0)
                {
                    Vector3 prevPos = previousHeadPosition;
                    for (int i = 0; i < _bodySegments.Count; i++)
                    {
                        Vector3 temp = _bodySegments[i].Transform.position;
                        _bodySegments[i].Transform.position = prevPos;
                        prevPos = temp;
                    }
                }
            }
        }
   
    // Call this method when the snake eats food to grow the body.
        public void Grow()
        {
            Vector3 spawnPosition;
            Vector2Int moveDirection = GetMoveDirectionVector();
            if (_bodySegments.Count == 0)
            {
                // If there are no segments yet, use the head's position.
                spawnPosition = _view.transform.position ;
            }
            else
            {
                // Otherwise, spawn at the position of the last segment.
                spawnPosition = _bodySegments[_bodySegments.Count - 1].Transform.position;
            }
            
            // Create a new body segment.            
            SnakePartController snakePartController = new SnakePartController(_snakeSO.SnakePartSO);
            // If your body segments are networked, spawn them as network objects.
           
            snakePartController.NetworkSpawn(_ownerClientId);
            snakePartController.SetPosition(spawnPosition);
            // Add the segment's transform to the list.
            _bodySegments.Add(snakePartController);
        }

        private IEnumerator GrowRoutine()
        {
            // Grow the snake.
            Grow();
            // Wait for the next growth opportunity.
            yield return new WaitForSeconds(1);
        }
    }
}
