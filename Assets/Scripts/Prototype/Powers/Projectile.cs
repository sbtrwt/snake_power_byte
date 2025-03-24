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
        [SerializeField] private Material bulletMaterial;
        [SerializeField] private float muzzleFlashDuration = 0.1f;

        private float muzzleFlashTime;
        [SerializeField] private float trailLength = 2f;
        [SerializeField] private ParticleSystem smokeRing;
        private void Start()
        {
            if (!IsServer) return; // Only the server handles targeting

            // Find the closest enemy when the projectile is spawned
            FindClosestEnemy();
        }

        private void FindClosestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float closestDistance = Mathf.Infinity;
            GameObject closestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            if (closestEnemy != null)
            {
                target = closestEnemy.transform;
                // Trigger muzzle flash
                muzzleFlashTime = Time.time;
                bulletMaterial.SetFloat("_MuzzleFlash", 1.0f);
                // Play smoke ring particle effect
                if(smokeRing != null){
                    var tempSmokeRing = Instantiate(smokeRing);
                    tempSmokeRing.transform.position = new Vector3( transform.position.x, transform.position.y, transform.position.z - 1);
                   
                    tempSmokeRing.Play();
                    Destroy(tempSmokeRing.gameObject, tempSmokeRing.main.duration);
                }
            }
            else
            {
                Debug.LogWarning("No enemy found. Destroying projectile.");
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!IsServer) return; // Only the server handles movement

            if (target == null)
            {
                // If the target is destroyed, find a new one
                FindClosestEnemy();
                return;
            }

            // Fade out muzzle flash
            float muzzleFlash = Mathf.Clamp01(1.0f - (Time.time - muzzleFlashTime) / muzzleFlashDuration);
            bulletMaterial.SetFloat("_MuzzleFlash", muzzleFlash);

            // Move towards the target
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Update trail length based on bullet speed
            bulletMaterial.SetFloat("_TrailLength", trailLength);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("OnTriggerEnter hit enemy.");
            if (!IsServer) return; // Only the server handles collisions

            // Apply damage to the hit object
            if (other.CompareTag("Enemy"))
            {
                Debug.Log("Projectile hit enemy.");
                Health health = other.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    // Destroy the projectile
                    Destroy(gameObject);
                }
            }


        }

        // Function to set the projectile's speed
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }
    }
}