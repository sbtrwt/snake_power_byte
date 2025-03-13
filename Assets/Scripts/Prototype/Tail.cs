// Tail.cs
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
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

        // NetworkVariable for replicating food type.
        public NetworkVariable<FoodType> networkFoodType = new NetworkVariable<FoodType>(
            FoodType.X,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        // NetworkVariable for the follow target's NetworkObjectId.
        public NetworkVariable<ulong> FollowTransformId = new NetworkVariable<ulong>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer)
            {
                networkFoodType.OnValueChanged += OnFoodTypeChanged;
                SetFoodTypeText(networkFoodType.Value);

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
                return;

            _targetPosition = _followTransform.position - _followTransform.forward * offset;
            _targetPosition += (transform.position - _targetPosition) * smoothSpeed;
            _targetPosition.z = 0;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveStep);
        }

        public void SetFoodType(FoodType foodType)
        {
            _foodType = foodType;
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
}
