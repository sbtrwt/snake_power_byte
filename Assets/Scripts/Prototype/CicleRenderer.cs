using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour
{
    [Tooltip("Radius of the circle.")]
    public float radius = 1f;
    
    [Tooltip("Number of segments used to approximate the circle.")]
    public int segments = 100;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Configure the line renderer as needed.
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
    }

    private void Start()
    {
        DrawCircle();
    }

    /// <summary>
    /// Draws a circle with the specified radius and segments.
    /// </summary>
    public void DrawCircle()
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

    /// <summary>
    /// Optionally update the circle in runtime if radius changes.
    /// </summary>
    private void Update()
    {
        // If you want the circle to update dynamically, uncomment the following line:
        // DrawCircle();
    }
}
