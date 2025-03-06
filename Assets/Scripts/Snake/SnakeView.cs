using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Snake
{
    public class SnakeView : NetworkBehaviour
    {
        public SnakeController Controller;
        
        void Awake()
        {
            // Only create a new controller if one hasn't been set already.
            if (Controller == null)
            {
                Controller = new SnakeController(this);
                Controller.Init();
            }
        }
        
        void Update()
        {
            if (!IsOwner) return;
            Controller?.Update();
        }
    }
}
