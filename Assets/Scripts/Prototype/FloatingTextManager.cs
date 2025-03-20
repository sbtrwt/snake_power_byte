using UnityEngine;
using Unity.Netcode;

public class FloatingTextManager : NetworkBehaviour
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
    /// Spawns a floating text object at the given world position on all clients and the server.
    /// </summary>
    public void ShowFloatingText(Vector3 worldPosition, string text, Color color)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning("FloatingTextPrefab is not assigned in FloatingTextManager.");
            return;
        }

        // Only the server can spawn networked objects
        if (IsServer)
        {
            Debug.Log("Server is spawning floating text.");
            SpawnFloatingText(worldPosition, text, color);
        }
    }

    private void SpawnFloatingText(Vector3 worldPosition, string text, Color color)
    {
        Debug.Log("Spawning FloatingText at position: " + worldPosition);
        GameObject instance = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity);
        FloatingText ft = instance.GetComponent<FloatingText>();
        if (ft != null)
        {
            // Spawn the object on the network
            instance.GetComponent<NetworkObject>().Spawn();

            // Initialize the text and color after spawning
            ft.Initialize(text, color);
        }
        else
        {
            Debug.LogError("FloatingText component is missing on the prefab!");
        }
    }
}