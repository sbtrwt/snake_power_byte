using System.Collections;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    [CreateAssetMenu(menuName = "Snake/ShootingPower", fileName = "NewShootingPower")]
    public class ShootingPower : PowerDefinition
    {
        [Tooltip("The projectile prefab to shoot.")]
        public GameObject projectilePrefab;

        [Tooltip("The time interval between shots.")]
        public float shootInterval = 1f;

        [Tooltip("The speed of the projectile.")]
        public float projectileSpeed = 10f;

        private bool isShootingActive = false;

        public override void Activate(GameObject snake)
        {
            base.Activate(snake);

            // Start shooting
            isShootingActive = true;
            SnakePowerManager powerManager = snake.GetComponent<SnakePowerManager>();
            if (powerManager != null)
            {
                powerManager.StartCoroutine(HandleShooting(powerManager));
            }
        }

        private IEnumerator HandleShooting(SnakePowerManager powerManager)
        {
            while (isShootingActive)
            {
                Shoot(powerManager);
                yield return new WaitForSeconds(shootInterval);
            }
        }

        private void Shoot(SnakePowerManager powerManager)
        {
            if (!powerManager.IsServer) return; // Only the server can spawn projectiles

            Debug.Log("Shooting projectile on the server.");
            powerManager.ShootClientRpc();
        }

        public void DeactivateShooting()
        {
            isShootingActive = false;
        }
    }
}