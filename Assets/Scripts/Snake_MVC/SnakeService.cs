using System.Collections.Generic;
using SnakePowerByte.Snake;
using Unity.Netcode;
using UnityEngine;

// ==============================
// 2. SnakeService (Handles Snake Instantiation)
// ==============================
public class SnakeService
{
 private readonly SnakeSO _snakeSO;
    private Dictionary<ulong, SnakeController> _snakes = new();

    public SnakeService(SnakeSO snakeSO) {
        _snakeSO = snakeSO;
    }

    public void SpawnSnake(ulong clientId, Vector3 spawnPosition) {
        if (!NetworkManager.Singleton.IsServer) return;

        SnakeView snakeObj = GameObject.Instantiate(
            _snakeSO.PrefabSnakeView,
            spawnPosition,
            Quaternion.identity
        );

        NetworkObject netObj = snakeObj.GetComponent<NetworkObject>();
        // Spawn with ownership based on the provided clientId.
        netObj.SpawnWithOwnership(clientId, true);

        // Create and assign the controller to the view
        SnakeController controller = new SnakeController(new SnakeModel(spawnPosition), snakeObj);
        snakeObj.SetController(controller);
        _snakes[clientId] = controller;

        Debug.Log($"[Spawn] Spawned snake for client {clientId} at {spawnPosition}");
    }
}