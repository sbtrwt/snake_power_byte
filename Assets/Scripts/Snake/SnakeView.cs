using UnityEngine;

namespace SnakePowerByte.Snake
{
    public class SnakeView : MonoBehaviour
    {
        public SnakeController Controller;
        void Update()
        {
            Controller?.Update();
        }

    }
}