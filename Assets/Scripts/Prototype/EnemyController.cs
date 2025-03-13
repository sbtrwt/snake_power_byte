// EnemyController.cs
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public class EnemyController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int health = 100;

        private void Update()
        {
            if (!IsServer) return;

            // Simple wandering logic; you can replace this with more sophisticated behavior.
            transform.position += (Vector3)Random.insideUnitCircle * moveSpeed * Time.deltaTime;
        }

        public void TakeDamage(int amount)
        {
            if (!IsServer) return;
            health -= amount;
            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // Optionally award XP to a player.
            NetworkObject netObj = GetComponent<NetworkObject>();
            netObj.Despawn();
        }
    }
}
