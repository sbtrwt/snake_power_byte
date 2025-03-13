using SnakePowerByte.Level;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Tail : NetworkBehaviour
{
    public Transform _followTransform;
    [SerializeField] private float offset = 0.3f;
    [Range(0.01f, 1f)]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float moveStep = 0.1f;
    [SerializeField] private TMP_Text textFoodType;
    private Vector3 _targetPosition;
    private FoodType _foodType;

    // Using a NetworkVariable to replicate the food type.
    public NetworkVariable<FoodType> networkFoodType = new NetworkVariable<FoodType>(
        FoodType.X,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    // New: Replicate the network ID of the transform to follow.
    public NetworkVariable<ulong> FollowTransformId = new NetworkVariable<ulong>(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Subscribe for food type changes on clients.
        if (!IsServer)
        {
            networkFoodType.OnValueChanged += OnFoodTypeChanged;
            // Immediately update the text with the current value.
            SetFoodTypeText(networkFoodType.Value);

            // Use the FollowTransformId to set _followTransform on clients.
            if (FollowTransformId.Value != 0 &&
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(FollowTransformId.Value, out NetworkObject followNetObj))
            {
                _followTransform = followNetObj.transform;
            }
        }
    }

    private void Update()
    {
        if (_followTransform == null)
        {
            return;
        }

        // Calculate target position based on follow transform with an offset.
        _targetPosition = _followTransform.position - _followTransform.forward * offset;
        _targetPosition += (transform.position - _targetPosition) * smoothSpeed;
        _targetPosition.z = 0;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveStep);
    }

    public void SetFoodType(FoodType foodType)
    {
        _foodType = foodType;
        // Only the server should write to the network variable.
        if (IsServer)
        {
            networkFoodType.Value = foodType;
        }
    }

    public void SetFoodTypeText(FoodType type)
    {
        if (textFoodType != null)
        {
            textFoodType.text = type.ToString();
        }
    }

    private void OnFoodTypeChanged(FoodType previousValue, FoodType newValue)
    {
        Debug.Log("FoodType changed from " + previousValue + " to " + newValue);
        SetFoodTypeText(newValue);
    }
}
