using Unity.Netcode;
using UnityEngine;
namespace SnakePowerByte.Prototype
{
    public class SnakeBodySegment : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        private void Update()
        {
            // Non-owner clients update their transform based on the network variable.
            if (!IsOwner)
            {
                transform.position = Position.Value;
            }
        }

        // Call this from the snake head simulation (running on the owner) via an RPC or similar.
        public void UpdatePosition(Vector3 newPosition)
        {
            if (IsOwner)
            {
                Position.Value = newPosition;
                transform.position = newPosition;
            }
        }
    }
}