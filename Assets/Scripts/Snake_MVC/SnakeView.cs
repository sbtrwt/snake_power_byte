// ==============================
// 4. SnakeView (Handles Visuals & Networking)
// ==============================
using Unity.Netcode;
using UnityEngine;

public class SnakeView : NetworkBehaviour
{
    private SnakeController _controller;
    // Using NetworkVariables to sync position and direction
    private NetworkVariable<Vector3> _netPosition = new();
    private NetworkVariable<Direction> _netDirection = new();

    public void SetController(SnakeController controller)
    {
        _controller = controller;
        _controller.Initialize(this);
        Debug.Log($"[SnakeView] SetController called. IsOwner: {IsOwner}, OwnerClientId: {OwnerClientId}");
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"[SnakeView] OnNetworkSpawn called. IsOwner: {IsOwner}, OwnerClientId: {OwnerClientId}");
        // On the server (host) we already created the controller.
        // On remote clients, if this instance is the owner but has no controller, create one locally.
        if (IsOwner && !IsServer && _controller == null)
        {
            _controller = new SnakeController(new SnakeModel(transform.position), this);
            _controller.Initialize(this);
            Debug.Log("[SnakeView] Created local controller for client owner");
        }

        if (IsOwner && IsServer)
        {
            _netPosition.Value = transform.position;
            _netDirection.Value = Direction.Right;
        }

        _netPosition.OnValueChanged += OnPositionChanged;
        _netDirection.OnValueChanged += OnDirectionChanged;
    }

    private void Update()
    {
        // For the owner, let the controller process input and update movement.
        if (IsOwner)
        {
            _controller?.Update(Time.deltaTime);
        }
        // All instances sync their visual position from the network variable.
        transform.position = _netPosition.Value;
    }

    public void Move(Vector3 newPosition, Direction newDirection)
    {
        Debug.Log($"[Move] Requesting move to {newPosition} in direction {newDirection} (IsServer: {IsServer})");
        if (IsServer)
        {
            _netPosition.Value = newPosition;
            _netDirection.Value = newDirection;
        }
        else
        {
            MoveServerRpc(newPosition, newDirection);
        }
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 newPosition, Direction newDirection)
    {
        Debug.Log($"[RPC] MoveServerRpc called with {newPosition}");
        _netPosition.Value = newPosition;
        _netDirection.Value = newDirection;
    }

    private void OnPositionChanged(Vector3 oldPos, Vector3 newPos)
    {
        Debug.Log($"[OnPositionChanged] Position updated from {oldPos} to {newPos}");
        transform.position = newPos;
    }

    private void OnDirectionChanged(Direction oldDir, Direction newDir)
    {
        // Optionally update visuals (rotation, etc.) based on newDir.
    }
}