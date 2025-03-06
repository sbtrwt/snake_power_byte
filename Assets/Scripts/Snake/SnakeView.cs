using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Snake
{
    public class SnakeView : NetworkBehaviour
    {
        public SnakeController Controller;
        void Awake()
        {
            Controller = new SnakeController(this);
            Controller.Init();
        }
        void Update()
        {
            if(!IsOwner ) return;
            Controller?.Update();
        }

    }
}