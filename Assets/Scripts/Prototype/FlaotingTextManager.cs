using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    [Tooltip("Floating text prefab with the FloatingText component.")]
    public GameObject floatingTextPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Spawns a floating text object at the given world position.
    /// </summary>
    public void ShowFloatingText(Vector3 worldPosition, string text, Color color)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning("FloatingTextPrefab is not assigned in FloatingTextManager.");
            return;
        }

        GameObject instance = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity);
        FloatingText ft = instance.GetComponent<FloatingText>();
        if (ft != null)
        {
            ft.Initialize(text, color);
        }
    }
}
