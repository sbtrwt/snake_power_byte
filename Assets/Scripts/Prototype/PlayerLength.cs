using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLength : NetworkBehaviour
{
    [SerializeField] private GameObject _tailPrefab;
    private Transform _lastTailTransform;
    private Collider2D _collider2D;

    // The length is modified only by the server.
    public NetworkVariable<ushort> Length = new NetworkVariable<ushort>(1,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private List<GameObject> _tails = new List<GameObject>();
    private FoodType _foodType = FoodType.O;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _lastTailTransform = transform;
        _collider2D = GetComponent<Collider2D>();

        // Do not subscribe OnValueChanged on clients if you're also instantiating tails on the server.
        // This avoids spawning duplicate tail segments.
        // If needed, you can subscribe on the server only.
        // if (IsServer)
        //     Length.OnValueChanged += OnLengthChanged;
    }

    // Instantiates a tail segment on the server.
    private void InstantiateTail(FoodType foodType = FoodType.O)
    {
        GameObject tailGameObject = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        tailGameObject.GetComponent<SpriteRenderer>().sortingOrder = -Length.Value;

        Tail tailComponent = null;
        if (tailGameObject.TryGetComponent(out Tail tail))
        {
            tailComponent = tail;
            // Set the local follow transform immediately.
            tail._followTransform = _lastTailTransform;
        }

        _tails.Add(tailGameObject);
        NetworkObject networkObject = tailGameObject.GetComponent<NetworkObject>();
        networkObject.Spawn();  // Spawn the tail so its NetworkBehaviour is initialized

        // Now update the NetworkVariable safely
        if (tailComponent != null)
        {
            NetworkObject followNetObj = _lastTailTransform.GetComponent<NetworkObject>();
            if (followNetObj != null)
            {
                tailComponent.FollowTransformId.Value = followNetObj.NetworkObjectId;
            }

            // Update _lastTailTransform to be the new tail's transform.
            _lastTailTransform = tailGameObject.transform;
            Physics2D.IgnoreCollision(_collider2D, tailGameObject.GetComponent<Collider2D>());
            tailComponent.SetFoodType(foodType);
            tailComponent.SetFoodTypeText(foodType);
        }
    }


    // Only the server should call this method.
    [ContextMenu("Add Length")]
    public void AddLength(FoodType foodType)
    {
        if (!IsServer) return; // Prevent non-server instances from modifying length.

        Length.Value += 1;
        _foodType = foodType;
        InstantiateTail(foodType);
    }

    // Optionally, if you want to handle length changes from other sources, 
    // you could subscribe to Length.OnValueChanged on the server only.
    // private void OnLengthChanged(ushort previousValue, ushort newValue)
    // {
    //     Debug.Log("Length changed from " + previousValue + " to " + newValue);
    //     if (newValue > previousValue)
    //     {
    //         InstantiateTail(_foodType);
    //     }
    //     else if (newValue < previousValue)
    //     {
    //         Destroy(_tails[_tails.Count - 1]);
    //         _tails.RemoveAt(_tails.Count - 1);
    //     }
    // }
}
