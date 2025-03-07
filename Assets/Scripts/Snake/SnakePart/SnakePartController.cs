using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Snake
{
    public class SnakePartController
    {
        private SnakePartView _view;
        public Transform Transform => _view.transform;
        public SnakePartController(SnakePartSO snakePartSO)
        {
            _view = GameObject.Instantiate(snakePartSO.Prefab);
            _view.Controller = this;
        }

        public void NetworkSpawn(ulong ownerClientId)
        {
            var segmentNetworkObject = _view.GetComponent<NetworkObject>();
            if (segmentNetworkObject != null)
            {
                segmentNetworkObject.SpawnWithOwnership(ownerClientId);
            }
            _view.gameObject.SetActive(true);
        }
        public void SetPosition(Vector3 position)
        {
            _view.transform.position = position;
        }
    }
}