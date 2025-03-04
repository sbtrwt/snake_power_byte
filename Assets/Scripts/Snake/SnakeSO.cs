using UnityEngine;

namespace SnakePowerByte.Snake
{
    [CreateAssetMenu(fileName = "SnakeSO", menuName = "Snake/SnakeSO")]
    public class SnakeSO:ScriptableObject
    {
        public float Speed;
        public float RotationSpeed;
        public SnakeView PrefabSnakeView;
    }
}