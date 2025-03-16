using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Tooltip("Speed at which the text moves upward.")]
    public float moveSpeed = 1f;

    [Tooltip("Duration over which the text fades out.")]
    public float fadeDuration = 1f;

    private TextMeshPro textMesh;
    private Color originalColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        originalColor = textMesh.color;
    }

    public void Initialize(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        originalColor = color;
    }

    private void Update()
    {
        // Move upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out over time
        Color c = textMesh.color;
        c.a -= Time.deltaTime / fadeDuration;
        textMesh.color = c;

        if (c.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
