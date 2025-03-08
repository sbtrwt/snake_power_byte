using Unity.Netcode;
using UnityEngine;

public class Tail:NetworkBehaviour
{
    public Transform _followTransform;
    [SerializeField]private float offset = 0.3f;
    [Range(0.01f, 1f)]
    [SerializeField]private float smoothSpeed = 0.125f;
    [SerializeField]private float moveStep = 0.1f;
    private Vector3 _targetPosition;

    private void Update()
    {
        if(_followTransform == null)
        {
            return;
        }
        _targetPosition = _followTransform.position - _followTransform.forward * offset;
        _targetPosition += (transform.position - _targetPosition) * smoothSpeed;
         _targetPosition.z=0;

     transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * moveStep);
    }
    private void LateUpdate()
    {
    
    }
}