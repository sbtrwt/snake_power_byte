using SnakePowerByte.Events;
using SnakePowerByte.Snake;
using UnityEngine;
using Unity.Netcode;

namespace SnakePowerByte
{
    public class GameService : NetworkBehaviour
    {
        private EventService _eventService;
        private SnakeService _snakeService;

        [Header("Scriptable Objects")]
        [SerializeField] private SnakeSO _snakeSO;
        
        private void Start()
        {
            // Only the client that owns this GameService will request a spawn
            if (IsOwner)
            {
                //RequestInitializeServicesServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestInitializeServicesServerRpc(ulong ownerClientId)
        {
            // This code runs on the server.
            _eventService = new EventService();
            _snakeService = new SnakeService(_snakeSO, ownerClientId);
            _snakeService.Init();
        }
    }
}
