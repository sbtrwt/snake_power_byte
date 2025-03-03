using UnityEngine;

namespace SnakePowerByte.Snake
{
    [CreateAssetMenu(fileName = "SnakePartSO", menuName = "Snake/SnakePartSO")]
    public class SnakePartSO:ScriptableObject
    {
        public float Speed;
        public float RotationSpeed;
    }
}