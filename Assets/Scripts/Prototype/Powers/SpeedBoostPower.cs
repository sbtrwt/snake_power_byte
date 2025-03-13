using SnakePowerByte.Snake;
using UnityEngine;

[CreateAssetMenu(menuName = "Snake/SpeedBoostPower", fileName = "NewSpeedBoostPower")]
public class SpeedBoostPower : PowerDefinition
{
    [Tooltip("Multiplier for speed boost.")]
    public float speedMultiplier = 2f;

    public override void Activate(GameObject snake)
    {
        base.Activate(snake);
        // Implement the speed boost logic, e.g.:
        // SnakeController controller = snake.GetComponent<SnakeController>();
        // if (controller != null)
        // {
        //     controller.ApplySpeedBoost(speedMultiplier, duration);
        // }
    }
}
