using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : NetworkBehaviour
{
    [Tooltip("Number of segments used to approximate the circle.")]
    public int segments = 100;

    // Network variable for radius synchronization
    private NetworkVariable<float> networkRadius = new NetworkVariable<float>(1f);

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Subscribe to changes in the network radius
        networkRadius.OnValueChanged += OnRadiusChanged;

        // Draw the initial circle
        DrawCircle(networkRadius.Value);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Unsubscribe from network variable changes
        networkRadius.OnValueChanged -= OnRadiusChanged;
    }

    private void OnRadiusChanged(float oldRadius, float newRadius)
    {
        Debug.Log($"Radius changed from {oldRadius} to {newRadius}");
        DrawCircle(newRadius);
    }

    /// <summary>
    /// Sets the radius of the circle and synchronizes it across the network.
    /// </summary>
    public void SetRadius(float newRadius)
    {
        if (IsServer)
        {
            networkRadius.Value = newRadius;
        }
    }

    /// <summary>
    /// Draws a circle with the specified radius and segments.
    /// </summary>
    private void DrawCircle(float radius)
    {
        lineRenderer.positionCount = segments;
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * (angleStep * i);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}