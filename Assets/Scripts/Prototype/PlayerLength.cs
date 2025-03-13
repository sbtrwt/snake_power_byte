// PlayerLength.cs
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public class PlayerLength : NetworkBehaviour
    {
        [SerializeField] private GameObject _tailPrefab;
        private Transform _lastTailTransform;
        private Collider2D _collider2D;

        // Length is modified only by the server.
        public NetworkVariable<ushort> Length = new NetworkVariable<ushort>(1, 
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private List<GameObject> _tails = new List<GameObject>();
        private FoodType _foodType = FoodType.O;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _lastTailTransform = transform;
            _collider2D = GetComponent<Collider2D>();
        }

        public void AddLength(FoodType foodType)
        {
            if (!IsServer) return;

            Length.Value += 1;
            _foodType = foodType;
            InstantiateTail(foodType);
        }

        private void InstantiateTail(FoodType foodType = FoodType.O)
        {
            GameObject tailGameObject = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
            tailGameObject.GetComponent<SpriteRenderer>().sortingOrder = -Length.Value;

            Tail tailComponent = null;
            if (tailGameObject.TryGetComponent<Tail>(out Tail tail))
            {
                tailComponent = tail;
                tail._followTransform = _lastTailTransform;
            }

            _tails.Add(tailGameObject);
            NetworkObject networkObject = tailGameObject.GetComponent<NetworkObject>();
            networkObject.Spawn();

            if (tailComponent != null)
            {
                NetworkObject followNetObj = _lastTailTransform.GetComponent<NetworkObject>();
                if (followNetObj != null)
                {
                    tailComponent.FollowTransformId.Value = followNetObj.NetworkObjectId;
                }
                _lastTailTransform = tailGameObject.transform;
                Physics2D.IgnoreCollision(_collider2D, tailGameObject.GetComponent<Collider2D>());
                tailComponent.SetFoodType(foodType);
                tailComponent.SetFoodTypeText(foodType);
            }
        }
    }
}
