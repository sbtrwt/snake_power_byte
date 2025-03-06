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
        private const float MAX_SPEED = 30f;

        private PlayerInputAction _playerInput;
        private InputAction _move;
        private SnakeView _view;
        private Direction _direction;
        private Vector2Int _gridPosition;
        private float _gridMoveTimer;
        // Set grid move time based on MAX_SPEED (e.g., moving one grid unit per move)
        private float _gridMoveTimeMax = 1f / MAX_SPEED;
        private LevelGrid _levelGrid;

        // Constructors...
        public SnakeController(SnakeSO snakeSO, ulong ownerClientId)
        {
            _view = GameObject.Instantiate(snakeSO.PrefabSnakeView);
            _view.Controller = this;
            var instanceNetworkObject = _view.GetComponent<NetworkObject>();
            instanceNetworkObject.SpawnWithOwnership(ownerClientId);
            _playerInput = new PlayerInputAction();
            _playerInput.Enable();
            _levelGrid = new LevelGrid(200, 200);
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
            {
                Vector2Int gridMoveDirectionVector = Vector2Int.RoundToInt(GetMoveDirectionVector());
                _gridPosition += gridMoveDirectionVector;
                _gridPosition = _levelGrid.ValidateGridPosition(_gridPosition);
                _gridMoveTimer -= _gridMoveTimeMax;

                _view.transform.position = new Vector3(_gridPosition.x, _gridPosition.y);
                _view.transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
            }
        }
    }
}
