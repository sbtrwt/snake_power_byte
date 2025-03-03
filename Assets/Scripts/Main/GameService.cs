using SnakePowerByte.Events;
using UnityEngine;

namespace SnakePowerByte
{
    public class GameService : MonoBehaviour
    {
        private EventService _eventService;

       

        private void Start()
        {
            InitializeServices();
        }

        private void InitializeServices()
        {
            _eventService = new EventService();
        }

        private void InjectDependencies()
        {
            
        }
    }
}
