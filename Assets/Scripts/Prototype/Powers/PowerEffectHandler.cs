using System.Collections;
using SnakePowerByte.Prototype;
using UnityEngine;
namespace SnakePowerByte.Prototype
{
    public class PowerEffectHandler : MonoBehaviour
    {
        /// <summary>
        /// Starts the vampire effect: for the specified duration, every interval the snake drains enemy health.
        /// </summary>
        /// <param name="drainRate">Damage dealt per interval to each enemy in range.</param>
        /// <param name="drainRadius">Radius in which to search for enemies.</param>
        /// <param name="healFactor">Fraction of total damage drained to heal the snake.</param>
        /// <param name="duration">Duration of the effect in seconds.</param>
        /// <param name="interval">Time between each drain cycle (in seconds).</param>
        public void StartVampireEffect(float drainRate, float drainRadius, float healFactor, float duration, float interval = 1f)
        {

            // If a CircleRenderer is attached on a child object, update its radius.
            CircleRenderer circle = GetComponentInChildren<CircleRenderer>();
            if (circle != null)
            {
                circle.radius = drainRadius;
                circle.DrawCircle();
            }
            StartCoroutine(VampireEffectCoroutine(drainRate, drainRadius, healFactor, duration, interval));
        }

        private IEnumerator VampireEffectCoroutine(float drainRate, float drainRadius, float healFactor, float duration, float interval)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // Find enemies in range (assumes enemies are tagged "Enemy")
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, drainRadius);
                float totalDrained = 0f;
                foreach (Collider2D hit in hits)
                {
                    if (hit.CompareTag("Enemy"))
                    {
                        Debug.Log("Draining enemy health");
                        // Assume enemy has an EnemyController with a TakeDamage method.
                        EnemyController enemy = hit.GetComponent<EnemyController>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage((int)drainRate);
                            totalDrained += drainRate;
                        }
                    }
                }
                // Heal the snake if needed (assumes snake has a Health component)
                Health snakeHealth = GetComponent<Health>();
                if (snakeHealth != null)
                {
                    snakeHealth.Heal(totalDrained * healFactor);
                    FloatingTextManager.Instance?.ShowFloatingText(transform.position, "+" + (totalDrained * healFactor).ToString("F0"), Color.green);
                }
                elapsed += interval;
                yield return new WaitForSeconds(interval);
            }
            StartCoroutine(VampireEffectCoroutine(drainRate, drainRadius, healFactor, duration, interval));
        }
    }
}