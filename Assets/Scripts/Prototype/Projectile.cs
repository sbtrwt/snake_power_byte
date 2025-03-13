// Projectile.cs
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private float speed = 10f;
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
    }
}
