using UnityEngine;

public interface IPowerEffect
{
    // Activate the power effect on the snake.
    // 'snake' could be a reference to your snake GameObject or controller.
    void Activate(GameObject snake);
}

[CreateAssetMenu(menuName = "Snake/PowerDefinition", fileName = "NewPowerDefinition")]
public class PowerDefinition : ScriptableObject, IPowerEffect
{
    [Tooltip("Combo string that triggers this power (e.g., 'XOXO').")]
    public string comboPattern;

    [Tooltip("Duration of the power effect in seconds.")]
    public float duration;

    [Tooltip("Optional description or name.")]
    public string powerName;

    // Override this method in derived assets to apply custom effects.
    public virtual void Activate(GameObject snake)
    {
        Debug.Log($"Power '{powerName}' activated for snake: {snake.name} for {duration} seconds.");
        // Implement your effect here.
        // For example, you might boost speed, enable shooting, grant invincibility, etc.
    }
}
