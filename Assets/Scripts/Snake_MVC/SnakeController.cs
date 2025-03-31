// ==============================
// 5. SnakeController (Handles Input & Logic)
// ==============================
using SnakePowerByte;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController
{
  private SnakeModel _model;
    private SnakeView _view;
    private PlayerInputAction _inputActions;
    private float _moveTimer;
    private const float MoveSpeed = 5f;
    private const float GridMoveTime = 1f / MoveSpeed;

    public SnakeController(SnakeModel model, SnakeView view) {
        _model = model;
        _view = view;
        SetupInput();
    }

    public void Initialize(SnakeView view) {
        _view = view;
        Debug.Log($"[SnakeController] Initialized for snake. IsOwner: {_view.IsOwner}");
    }

    private void SetupInput() {
        _inputActions = new PlayerInputAction();
        _inputActions.Snake.Enable();
        _inputActions.Snake.Move.performed += OnMovePerformed;
        Debug.Log("[SnakeController] Input setup complete.");
    }

    private void OnMovePerformed(InputAction.CallbackContext context) {
        // For debugging, temporarily remove ownership check:
        // if (!_view.IsOwner) return;
        Vector2 input = context.ReadValue<Vector2>();
        Debug.Log($"[Input] Received: {input}");
        UpdateDirection(input);
    }

    private void UpdateDirection(Vector2 input) {
        if (input.x > 0) _model.MoveDirection.Value = Direction.Right;
        else if (input.x < 0) _model.MoveDirection.Value = Direction.Left;
        else if (input.y > 0) _model.MoveDirection.Value = Direction.Up;
        else if (input.y < 0) _model.MoveDirection.Value = Direction.Down;
        Debug.Log($"[Direction] Updated to {_model.MoveDirection.Value}");
    }

    public void Update(float deltaTime) {
        if (!_view.IsOwner) return;
        _moveTimer += deltaTime;
        if (_moveTimer >= GridMoveTime) {
            _moveTimer = 0f;
            MoveSnake();
        }
    }

    private void MoveSnake() {
        Vector3 newPosition = _view.transform.position + GetMoveVector();
        Debug.Log($"[MoveSnake] Moving snake to {newPosition}");
        _view.Move(newPosition, _model.MoveDirection.Value);
    }

    private Vector3 GetMoveVector() {
        return _model.MoveDirection.Value switch {
            Direction.Right => Vector3.right,
            Direction.Left  => Vector3.left,
            Direction.Up    => Vector3.up,
            Direction.Down  => Vector3.down,
            _               => Vector3.right,
        };
    }
}