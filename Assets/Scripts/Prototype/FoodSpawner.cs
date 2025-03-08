using Unity.Netcode;
using UnityEngine;

public class FoodSpawner : NetworkBehaviour
{
    [Header("Food Settings")]
    [Tooltip("Prefab for the food object. Must have a NetworkObject component and be tagged 'Food'.")]
    [SerializeField] private GameObject foodPrefab;
    
    [Tooltip("Time interval (in seconds) between spawns.")]
    [SerializeField] private float spawnInterval = 5f;
    
    [Tooltip("The grid size for spawning food (e.g., a grid of 200x200).")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int(200, 200);

    private float spawnTimer;

    private void Update()
    {
        // Only run the spawner on the server.
        if (!IsServer)
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnFood();
        }
    }

    private void SpawnFood()
    {
        // Generate a random grid position.
        int x = Random.Range(0, gridSize.x);
        int y = Random.Range(0, gridSize.y);
        Vector3 spawnPosition = new Vector3(x, y, 0);

        // Instantiate the food prefab at the random position.
        GameObject newFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        NetworkObject netObj = newFood.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
    }
}
