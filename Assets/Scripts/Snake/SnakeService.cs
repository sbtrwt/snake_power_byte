namespace SnakePowerByte.Snake
{
    public class SnakeService
    {
        SnakeController _snakeController;

        public SnakeService(SnakeSO snakeSO)
        {
            _snakeController = new SnakeController(snakeSO);
        }
    }
}