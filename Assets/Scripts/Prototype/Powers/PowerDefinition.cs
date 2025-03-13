// PowerDefinition.cs
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    public enum PowerType
    {
        SpeedBoost,
        Vampire,
        Invincibility,
        DoubleScore,
        None
    }

    public interface IPowerEffect
    {
        void Activate(GameObject snake);
    }

    [CreateAssetMenu(menuName = "Snake/PowerDefinition", fileName = "NewPowerDefinition")]
    public class PowerDefinition : ScriptableObject, IPowerEffect
    {
        [Tooltip("Combo string that triggers this power (e.g., 'XOXO')")]
        public string comboPattern;

        [Tooltip("Duration in seconds for which the power is active")]
        public float duration;

        [Tooltip("Display name for the power")]
        public string powerName;

        [Tooltip("Initial level of the power")]
        public int powerLevel = 1;

        public virtual void Activate(GameObject snake)
        {
            Debug.Log($"Power '{powerName}' (Level {powerLevel}) activated on {snake.name} for {duration} seconds.");
            // Default behavior; override this method in derived assets for custom effects.
        }
    }
}
