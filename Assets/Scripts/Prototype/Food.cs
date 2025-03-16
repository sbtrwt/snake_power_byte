// Food.cs
using TMPro;
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
        [Header("Food Settings")]
        [Tooltip("The type of food (X or O).")]
        [SerializeField] private FoodType foodType;

        [Tooltip("The speed at which the food rotates.")]
        [SerializeField] private float rotationSpeed = 100f;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TMP_Text textFoodType;
        public NetworkVariable<FoodType> networkFoodType = new NetworkVariable<FoodType>(FoodType.X, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsServer)
                networkFoodType.OnValueChanged += OnFoodTypeChanged;
        }
        private void Update()
        {
            // Rotate the food object.
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        private void OnFoodTypeChanged(FoodType previousValue, FoodType newValue)
        {
            Debug.Log("FoodType changed from " + previousValue + " to " + newValue);
            SetFoodTypeText(newValue);
        }
        public void SetFoodType(FoodType type)
        {
            foodType = type;
            networkFoodType.Value = type;
        }
        public void SetFoodTypeText(FoodType type)
        {
            textFoodType.text = type.ToString();
        }
        public FoodType GetFoodType()
        {
            return foodType;
        }
    }
}
