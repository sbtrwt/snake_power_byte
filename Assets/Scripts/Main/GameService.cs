using SnakePowerByte.Events;
using SnakePowerByte.Snake;
using UnityEngine;

namespace SnakePowerByte
{
    public class GameService : MonoBehaviour
    {
        private EventService _eventService;
        private SnakeService _snakeService;
       

        [Header("Scriptable Objects")]
        [SerializeField] private SnakeSO _snakeSO;
        private void Start()
        {
            InitializeServices();
            InjectDependencies();
        }

        private void InitializeServices()
        {
            _eventService = new EventService();
            _snakeService = new SnakeService(_snakeSO);
        }

        private void InjectDependencies()
        {
            _snakeService.Init();
        }
    }
}
