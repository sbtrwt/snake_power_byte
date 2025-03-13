// Food.cs
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public enum FoodType
    {
        X,
        O,
        // Add additional food types as needed.
    }

    public class Food : NetworkBehaviour
    {
        [SerializeField] private FoodType foodType = FoodType.X;

        public FoodType GetFoodType()
        {
            return foodType;
        }
    }
}
