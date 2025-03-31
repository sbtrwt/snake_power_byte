// ==============================
// 1. GameService (Manages Game State)
// ==============================


using SnakePowerByte.Snake;
using Unity.Netcode;
using UnityEngine;

public class GameService : NetworkBehaviour {
[SerializeField] private SnakeSO _snakeSO;
    [SerializeField] private Transform[] _spawnPoints;
    private SnakeService _snakeService;

    public override void OnNetworkSpawn() {
        if (IsServer) {
            _snakeService = new SnakeService(_snakeSO);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId) {
        if (_spawnPoints.Length == 0) return;
        int spawnIndex = (int)clientId % _spawnPoints.Length;
        _snakeService.SpawnSnake(clientId, _spawnPoints[spawnIndex].position);
    }

    public override void OnDestroy() {
        if (IsServer && NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}