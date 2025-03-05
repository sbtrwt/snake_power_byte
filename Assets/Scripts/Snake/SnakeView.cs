using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Snake
{
    public class SnakeView : NetworkBehaviour
    {
        public SnakeController Controller;
        void Update()
        {
            Controller?.Update();
        }

    }
}