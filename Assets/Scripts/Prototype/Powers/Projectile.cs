// Projectile.cs
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float speed = 10f;
           
        [Tooltip("The damage dealt by the projectile.")]
        public int damage = 10;
        private Transform target;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }

        private void Update()
        {
            if (target == null) return;
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Only the server handles collisions

        // Apply damage to the hit object
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        // Destroy the projectile
        Destroy(gameObject);
    }
    }
}
