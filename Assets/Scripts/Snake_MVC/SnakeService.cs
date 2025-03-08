namespace SnakePowerByte.Snake
{
    public class SnakeService
    {
        private SnakeController _snakeController;

        // Modified constructor to accept an ownerClientId parameter
        public SnakeService(SnakeSO snakeSO, ulong ownerClientId)
        {
            _snakeController = new SnakeController(snakeSO, ownerClientId);
        }

        public void Init()
        {
            _snakeController.Init();
        }
    }
}