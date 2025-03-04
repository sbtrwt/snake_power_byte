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
        private PlayerInputAction _playerInput;
        private InputAction _move;
        private SnakeView _view;
        public SnakeController(SnakeSO snakeSO)
        {
            _view =GameObject.Instantiate(snakeSO.PrefabSnakeView ) ;
            _view.Controller = this;

            _playerInput = new PlayerInputAction();
            _playerInput.Enable();
        }

        public void Init()
        {
            _move = _playerInput.Snake.Move;
          _move.performed += ctx => OnMove();   
        }
        public void Update()
        {
            //Vector2 moveInput = move.ReadValue<Vector2>();
        }
        public void OnMove(){
             Vector2 moveInput = _move.ReadValue<Vector2>();

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
                    break;
                case Direction.Right:
                    break;
                case Direction.Up:
                    break;
                case Direction.Down:
                    break;
            }
        }
    }
}