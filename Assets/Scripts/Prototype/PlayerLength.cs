using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLength : NetworkBehaviour
{
    [SerializeField] private GameObject _tailPrefab;
    private Transform _lastTailTransform;
    private Collider2D _collider2D;
    public NetworkVariable<ushort> Length = new NetworkVariable<ushort>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private List<GameObject> _tails;
    private FoodType _foodType= FoodType.O;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _tails = new List<GameObject>();
        _lastTailTransform = transform;
        _collider2D = GetComponent<Collider2D>();
        if(!IsServer)
        Length.OnValueChanged += OnLengthChanged;
    }
    private void InstantiateTail(FoodType foodType = FoodType.O)
    {
        GameObject tailGameObject = Instantiate(_tailPrefab, transform.position, Quaternion.identity);
        tailGameObject.GetComponent<SpriteRenderer>().sortingOrder = -Length.Value;

        if(tailGameObject.TryGetComponent(out Tail tail))
        {
            //tail.own = transform;
            Debug.Log("Tail has Tail component");
            tail._followTransform = _lastTailTransform;
            _lastTailTransform = tailGameObject.transform;
            Physics2D.IgnoreCollision(_collider2D, tailGameObject.GetComponent<Collider2D>());
            tail.SetFoodType(foodType);
            tail.SetFoodTypeText(foodType);
        }
        NetworkObject networkObject = tailGameObject.GetComponent<NetworkObject>();
        networkObject.Spawn();
        _tails.Add(tailGameObject);
    }
    private void OnLengthChanged(ushort previousValue, ushort newValue)
    {
        Debug.Log("Length changed from " + previousValue + " to " + newValue);
        if (newValue > previousValue)
        {
            InstantiateTail(_foodType);
        }
        else if (newValue < previousValue)
        {
            Destroy(_tails[_tails.Count - 1]);
            _tails.RemoveAt(_tails.Count - 1);
        }
    }
    [ContextMenu("Add Length")]
    public void AddLength(FoodType foodType)
    {
        Length.Value += 1;
        _foodType = foodType;
        InstantiateTail(foodType);
    }

}