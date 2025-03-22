// SnakeAutoAttack.cs
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    [RequireComponent(typeof(NetworkObject))]
    public class SnakeAutoAttack : NetworkBehaviour
    {
        [Header("Attack Settings")]
        [Tooltip("Time in seconds between automatic attacks.")]
        [SerializeField] private float attackInterval = 5f;

        [Tooltip("Radius in which to search for targets.")]
        [SerializeField] private float attackRadius = 5f;

        [Tooltip("Projectile prefab to use for attacks (must have a NetworkObject).")]
        [SerializeField] private GameObject projectilePrefab;

        [Tooltip("Speed at which the projectile moves.")]
        [SerializeField] private float projectileSpeed = 10f;

        private float attackTimer = 0f;

        private void Update()
        {
            if (!IsServer)
                return;

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                AutoAttack();
            }
        }

        private void AutoAttack()
        {
            Transform target = FindNearestTarget();
            if (target != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                NetworkObject netObj = projectile.GetComponent<NetworkObject>();
                netObj.Spawn();

                Projectile projectileScript = projectile.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    //projectileScript.SetTarget(target);
                    projectileScript.SetSpeed(projectileSpeed);
                }

                Debug.Log($"AutoAttack: Launched projectile toward {target.name} at {Time.time}");
            }
            else
            {
                Debug.Log($"AutoAttack: No valid target found within radius at {Time.time}");
            }
        }

        private Transform FindNearestTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);
            Transform nearestTarget = null;
            float minDistance = Mathf.Infinity;

            foreach (Collider2D hit in hits)
            {
                if (hit.transform == transform)
                    continue;

                if (hit.CompareTag("Enemy"))
                {
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestTarget = hit.transform;
                    }
                }
            }
            return nearestTarget;
        }
    }
}
