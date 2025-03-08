using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The target for the camera to follow (for example, the snake head).
    public Transform target;
    
    // The offset from the target's position.
    public Vector3 offset = new Vector3(0, 0, -10);
    
    // A smoothing factor for camera movement.
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position with offset.
        Vector3 desiredPosition = target.position + offset;
        // Smoothly interpolate between the current position and the desired position.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
