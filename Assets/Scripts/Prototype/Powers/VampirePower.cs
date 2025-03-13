using SnakePowerByte.Prototype;
using UnityEngine;

[CreateAssetMenu(menuName = "Snake/VampirePower", fileName = "NewVampirePower")]
public class VampirePower : PowerDefinition
{
    [Header("Vampire Power Settings")]
    [Tooltip("Damage to drain per interval (per enemy).")]
    public float drainRate = 5f;
    [Tooltip("Radius in which to drain enemy health.")]
    public float drainRadius = 4f;
    [Tooltip("Fraction of drained damage converted to healing for the snake (0 to 1).")]
    public float healFactor = 0.5f;
    [Tooltip("Interval (in seconds) between drain cycles.")]
    public float drainInterval = 1f;

    public override void Activate(GameObject snake)
    {
        base.Activate(snake);
        // Get or add a PowerEffectHandler on the snake.
        PowerEffectHandler handler = snake.GetComponent<PowerEffectHandler>();
        if (handler == null)
        {
            handler = snake.AddComponent<PowerEffectHandler>();
        }
        // Start the vampire effect.
        handler.StartVampireEffect(drainRate, drainRadius, healFactor, duration, drainInterval);
    }
}
