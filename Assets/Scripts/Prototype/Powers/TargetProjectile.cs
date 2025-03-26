using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public class TargetProjectile : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 10f;
        [Tooltip("If > 0, projectile will home in on target")]
        [SerializeField] private float homingStrength = 0f;
        [Tooltip("Time before projectile self-destructs")]
        [SerializeField] private float lifetime = 5f;

        [Header("Combat Settings")]
        [Tooltip("The damage dealt by the projectile.")]
        public int damage = 10;
        public LayerMask hitLayerMask;

        [Header("Visual Effects")]
        [SerializeField] private Material bulletMaterial;
        [SerializeField] private float muzzleFlashDuration = 0.1f;
        [SerializeField] private float trailLength = 2f;
        [SerializeField] private ParticleSystem muzzleFlashEffect;
        [SerializeField] private ParticleSystem smokeRing;
        [SerializeField] private ParticleSystem impactEffect;

        private float spawnTime;
        private Transform target;
        private float muzzleFlashTime;
        private Vector3 initialDirection;
        private bool hasHit = false;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            spawnTime = Time.time;
            initialDirection = transform.forward;

            // If no target was pre-assigned, try to find one
            if (target == null)
            {
                FindClosestEnemy();
            }
            else
            {
                TriggerMuzzleFlash();
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (IsServer)
            {
                TriggerMuzzleFlash();
            }
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }

        public void SetHomingStrength(float strength)
        {
            homingStrength = strength;
        }

        private void TriggerMuzzleFlash()
        {
            muzzleFlashTime = Time.time;
            if (bulletMaterial != null)
            {
                bulletMaterial.SetFloat("_MuzzleFlash", 1.0f);
            }

            if (muzzleFlashEffect != null)
            {
                muzzleFlashEffect.Play();
            }

            if (smokeRing != null)
            {
                Vector3 spawnPos = transform.position - transform.forward * 0.5f;
                var smoke = Instantiate(smokeRing, spawnPos, Quaternion.identity);
                smoke.Play();
                Destroy(smoke.gameObject, smoke.main.duration);
            }
        }

        private void FindClosestEnemy()
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, 20f, LayerMask.GetMask("Enemies"));
            
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (Collider enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }

            if (closestEnemy != null)
            {
                target = closestEnemy;
                TriggerMuzzleFlash();
            }
            else if (lifetime > 0)
            {
                // If no target but has lifetime, just fly straight
                target = null;
            }
            else
            {
                // No target and no lifetime - self-destruct
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // Handle muzzle flash fade on all clients
            if (bulletMaterial != null)
            {
                float muzzleFlash = Mathf.Clamp01(1.0f - (Time.time - muzzleFlashTime) / muzzleFlashDuration);
                bulletMaterial.SetFloat("_MuzzleFlash", muzzleFlash);
                bulletMaterial.SetFloat("_TrailLength", trailLength);
            }

            if (!IsServer || hasHit) return;

            // Lifetime check
            if (lifetime > 0 && Time.time > spawnTime + lifetime)
            {
                Destroy(gameObject);
                return;
            }

            // Movement handling
            Vector3 direction;
            if (target != null && homingStrength > 0)
            {
                // Homing behavior
                direction = (target.position - transform.position).normalized;
                direction = Vector3.Lerp(initialDirection, direction, homingStrength * Time.deltaTime).normalized;
                initialDirection = direction; // Update for next frame
            }
            else if (target != null)
            {
                // Straight tracking
                direction = (target.position - transform.position).normalized;
            }
            else
            {
                // Straight flight
                direction = initialDirection;
            }

            transform.position += direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer || hasHit) return;

            // Check if we hit something we care about
            if (((1 << other.gameObject.layer) & hitLayerMask) != 0)
            {
                hasHit = true;
                
                // Apply damage
                Health health = other.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }

                // Play impact effect
                if (impactEffect != null)
                {
                    var impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
                    impact.Play();
                    Destroy(impact.gameObject, impact.main.duration);
                }

                // Destroy projectile
                Destroy(gameObject);
            }
        }
    }
}