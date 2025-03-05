using SnakePowerByte.Level;
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
        private const float MAX_SPEED = 3f;
        
        private PlayerInputAction _playerInput;
        private InputAction _move;
        private SnakeView _view;
        private Direction _direction;
         private Vector2Int _gridPosition;
          private float _gridMoveTimer;
        private float _gridMoveTimeMax;
        private LevelGrid _levelGrid;
        public SnakeController(SnakeSO snakeSO)
        {
            _view = GameObject.Instantiate(snakeSO.PrefabSnakeView ) ;
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
        { HandleGridMovement();
            //Vector2 moveInput = move.ReadValue<Vector2>();
        }
        public void OnMove()
        {
            Vector2 moveInput = _move.ReadValue<Vector2>();
            Debug.Log(moveInput);
            if(moveInput.x > 0){
                Move(Direction.Right);
            }
            else if(moveInput.x < 0){
                Move(Direction.Left);
            }
            else if(moveInput.y > 0){
                Move(Direction.Up);
            }
            else if(moveInput.y < 0){
                Move(Direction.Down);
            }
           
        }
        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    _direction = Direction.Left;
                   // _view.Move(Vector2.left);
                    break;
                case Direction.Right:
                    _direction = Direction.Right;
                    break;
                case Direction.Up:
                    _direction = Direction.Up;
                    break;
                case Direction.Down:
                    _direction = Direction.Down;
                    break;
            }
        }
        private Vector2Int GetMoveDirectionVector()
        {
            Vector2Int gridMoveDirectionVector;
            switch (_direction)
            {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, 1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }

            return gridMoveDirectionVector;
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
                Vector2Int gridMoveDirectionVector;

                //Insert snake position
                //snakeMovePositionList.Insert(0, new SnakeMovePosition(gridPosition, gridMoveDirection));

                //Position increment
                gridMoveDirectionVector = Vector2Int.RoundToInt(GetMoveDirectionVector() + new Vector2(MAX_SPEED * Time.deltaTime, MAX_SPEED * Time.deltaTime));
                _gridPosition += gridMoveDirectionVector;
                _gridPosition = _levelGrid.ValidateGridPosition(_gridPosition);

                _gridMoveTimer -= _gridMoveTimeMax;

                // if (snakeMovePositionList.Count >= snakeBodySize + 1)
                // {
                //     snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
                // }


                _view.transform.position = new Vector3(_gridPosition.x, _gridPosition.y);
                 _view.transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);

               

                
            }
        }
    }
}