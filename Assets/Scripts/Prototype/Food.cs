using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum FoodType{
    X,
    O
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
    private void Update()
    {
        // Rotate the food object.
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    public FoodType GetFoodType()
    {
        return foodType;
    }
    public void SetFoodType(FoodType type)
    {
        foodType = type;
    }
    public void SetFoodTypeText(FoodType type)
    {
        textFoodType.text = type.ToString();
    }
}
