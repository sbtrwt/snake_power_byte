using Unity.Netcode;
using UnityEngine;

public class TurretController : NetworkBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 120f;
    public float detectionRange = 15f;
    public LayerMask targetMask;

    [Header("References")]
    public Transform rotationBase; // The part that should rotate
    public LineRenderer laser;

    private Transform currentTarget;
    private Vector3 baseOffset;

    private void Start()
    {
        // Store initial offset from snake head
        baseOffset = transform.localPosition;
        
        // Initialize laser
        if(laser != null)
        {
            laser.positionCount = 2;
            laser.startWidth = laser.endWidth = 0.1f;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Maintain position offset from snake head
        transform.localPosition = baseOffset;

        FindTarget();
        RotateTurret();
        UpdateLaser();
    }

    private void FindTarget()
    {
        Collider[] targets = Physics.OverlapSphere(
            transform.position,
            detectionRange,
            targetMask
        );

        // Simple closest target selection
        currentTarget = targets.Length > 0 
            ? GetClosestTarget(targets) 
            : null;
    }

    private Transform GetClosestTarget(Collider[] targets)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider target in targets)
        {
            float distance = Vector3.Distance(
                transform.position, 
                target.transform.position
            );
            
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = target.transform;
            }
        }
        return closest;
    }

    private void RotateTurret()
    {
        if (currentTarget == null || rotationBase == null) return;

        // Calculate direction in LOCAL space
        Vector3 localTargetDir = transform.InverseTransformPoint(currentTarget.position);
        localTargetDir.y = 0; // Keep rotation horizontal

        // Create rotation
        Quaternion targetRotation = Quaternion.LookRotation(localTargetDir);
        
        // Smooth rotation using LOCAL space
        rotationBase.localRotation = Quaternion.RotateTowards(
            rotationBase.localRotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Debug visualization
        Debug.DrawRay(rotationBase.position, 
            rotationBase.forward * 10, 
            Color.red);
    }

    private void UpdateLaser()
    {
        if (laser == null) return;

        Vector3 startPoint = rotationBase.position;
        Vector3 endPoint = currentTarget != null 
            ? currentTarget.position 
            : startPoint + rotationBase.forward * 50;

        laser.SetPosition(0, startPoint);
        laser.SetPosition(1, endPoint);
    }
}