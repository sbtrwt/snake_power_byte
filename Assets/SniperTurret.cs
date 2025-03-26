using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class SniperTurret : NetworkBehaviour
{
    public float rotationSpeed = 300f;
    public float detectionRange = 1000f;
    public LayerMask targetLayerMask;
    
    private Transform currentTarget;
    private Transform turretBase; // The part that should rotate

    private void Start()
    {
        // Assign the rotating part of your turret
        turretBase = transform.GetChild(0); // Change index as needed
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        FindTarget();
        RotateTurret();
    }

    private void FindTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, targetLayerMask);
        
        if (targets.Length > 0)
        {
            // Simple closest target finding
            float closestDistance = Mathf.Infinity;
            foreach (var target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = target.transform;
                }
            }
        }
        else
        {
            currentTarget = null;
        }
    }

    private void RotateTurret()
    {
        if (currentTarget != null && turretBase != null)
        {
            Vector3 direction = (currentTarget.position - turretBase.position).normalized;
            direction.y = 0; // Only rotate on Y axis
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                turretBase.rotation = Quaternion.Slerp(
                    turretBase.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}