using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public class SniperBullet : NetworkBehaviour
    {
        [SerializeField] private float speed = 10f;
        public int damage = 10;
        private Transform target;
        [SerializeField] private Material bulletMaterial;
        [SerializeField] private float muzzleFlashDuration = 0.1f;
        private float muzzleFlashTime;
        [SerializeField] private float trailLength = 2f;
        [SerializeField] private ParticleSystem smokeRing;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            
            // Trigger muzzle flash
            muzzleFlashTime = Time.time;
            bulletMaterial.SetFloat("_MuzzleFlash", 1.0f);
            
            // Play smoke ring particle effect
            if(smokeRing != null && target != null)
            {
                var tempSmokeRing = Instantiate(smokeRing);
                Vector3 direction = (target.position - transform.position).normalized * -10;
                tempSmokeRing.transform.position = new Vector3(
                    transform.position.x + direction.x, 
                    transform.position.y + direction.y, 
                    transform.position.z + direction.z - 1
                );
                
                tempSmokeRing.Play();
                Rigidbody2D smokeRb = tempSmokeRing.GetComponent<Rigidbody2D>();
                if (smokeRb != null)
                {
                    smokeRb.linearVelocity = direction;
                }
                Destroy(tempSmokeRing.gameObject, tempSmokeRing.main.duration);
            }
        }

        private void Update()
        {
            if (!IsServer || target == null) return;

            // Fade out muzzle flash
            float muzzleFlash = Mathf.Clamp01(1.0f - (Time.time - muzzleFlashTime) / muzzleFlashDuration);
            bulletMaterial.SetFloat("_MuzzleFlash", muzzleFlash);

            // Move towards the target
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Update trail length
            bulletMaterial.SetFloat("_TrailLength", trailLength);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsServer) return;

            if (other.CompareTag("Enemy"))
            {
                Health health = other.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    Destroy(gameObject);
                }
            }
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }
    }
}