// ==============================
// 3. SnakeModel (Stores Snake Data)
// ==============================
using Unity.Netcode;
using UnityEngine;

// Enum for Directions
public enum Direction { Up, Down, Left, Right }
public class SnakeModel { 
    public NetworkVariable<Vector3> Position { get; private set; }
    public NetworkVariable<Direction> MoveDirection { get; private set; }

    public SnakeModel(Vector3 startPosition) {
        Position = new NetworkVariable<Vector3>(startPosition);
        MoveDirection = new NetworkVariable<Direction>(Direction.Right);
    }
}