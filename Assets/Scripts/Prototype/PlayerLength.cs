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
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _tails = new List<GameObject>();
        _lastTailTransform = transform;
        _collider2D = GetComponent<Collider2D>();
        if(!IsServer)
        Length.OnValueChanged += OnLengthChanged;
    }
    private void InstantiateTail()
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
            InstantiateTail();
        }
        else if (newValue < previousValue)
        {
            Destroy(_tails[_tails.Count - 1]);
            _tails.RemoveAt(_tails.Count - 1);
        }
    }
    [ContextMenu("Add Length")]
    public void AddLength()
    {
        Length.Value += 1;
        InstantiateTail();
    }

}