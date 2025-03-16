using UnityEngine;
using Unity.Netcode;

namespace SnakePowerByte.Prototype
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("Enemy prefab with a NetworkObject and Health component.")]
        [SerializeField] private GameObject enemyPrefab;

        [Tooltip("Floating health bar prefab (World Space Canvas with FloatingHealthBar script).")]
        [SerializeField] private GameObject floatingHealthBarPrefab;

        [Tooltip("Time interval (in seconds) between enemy spawns.")]
        [SerializeField] private float spawnInterval = 5f;

        [Tooltip("Minimum spawn position (x, y).")]
        [SerializeField] private Vector2 spawnAreaMin = new Vector2(-10f, -10f);

        [Tooltip("Maximum spawn position (x, y).")]
        [SerializeField] private Vector2 spawnAreaMax = new Vector2(10f, 10f);

        private float spawnTimer = 0f;

        private void Update()
        {
            // Only the server spawns enemies.
            if (!NetworkManager.Singleton.IsServer)
                return;

            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            // Generate a random spawn position within defined area.
            float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            // Instantiate enemy prefab.
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            NetworkObject enemyNetObj = enemyInstance.GetComponent<NetworkObject>();
            if (enemyNetObj != null)
            {
                enemyNetObj.Spawn();
            }
            else
            {
                Debug.LogWarning("Enemy prefab missing NetworkObject component!");
            }

            // Instantiate and attach the floating health bar.
            if (floatingHealthBarPrefab != null)
            {
                // Instantiate the health bar at the same spawn position.
                GameObject hbInstance = Instantiate(floatingHealthBarPrefab, spawnPosition, Quaternion.identity);
                // Optionally, make the health bar a child of the enemy so it moves with it.
                hbInstance.transform.SetParent(enemyInstance.transform);

                FloatingHealthBar healthBar = hbInstance.GetComponent<FloatingHealthBar>();
                Health enemyHealth = enemyInstance.GetComponent<Health>();

                if (healthBar != null && enemyHealth != null)
                {
                    // Initialize the health bar with the enemy's transform and Health component.
                    healthBar.Initialize(enemyInstance.transform, enemyHealth);
                }
                else
                {
                    Debug.LogWarning("FloatingHealthBar or Health component not found on enemy!");
                }
            }
        }
    }
}
