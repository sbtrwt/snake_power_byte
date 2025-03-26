using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace SnakePowerByte.Prototype
{
    [CreateAssetMenu(menuName = "Snake/SniperPower", fileName = "SniperPower")]
    public class SniperPower : PowerDefinition
    {
        [Header("Sniper Settings")]
        [SerializeField] private GameObject sniperPrefab;
        [SerializeField] private float offsetY = 0.5f;
        [Tooltip("How fast the sniper rotates to face targets")]
        public float rotationSpeed = 5f;

        [Header("Shooting Settings")]
        [Tooltip("The projectile prefab to shoot.")]
        public GameObject projectilePrefab;
        [Tooltip("The time interval between shots.")]
        public float shootInterval = 1f;
        [Tooltip("The speed of the projectile.")]
        public float projectileSpeed = 10f;
        [Tooltip("Offset from sniper where projectiles should spawn")]
        public Vector2 projectileSpawnOffset = new Vector2(0.5f, 0);
        
        private GameObject currentSniper;
        private bool isShootingActive = false;
        private Transform currentTarget;
        private Coroutine shootingCoroutine;
        private Coroutine trackingCoroutine;

        public override void Activate(GameObject snakeHead)
        {
            base.Activate(snakeHead);
            if (sniperPrefab == null)
            {
                Debug.LogError("Sniper prefab is not assigned in SniperPower scriptable object!");
                return;
            }
            if (sniperPrefab == null)
            {
                Debug.LogError("Sniper prefab is not assigned in SniperPower scriptable object!");
                return;
            }
            
            if (NetworkManager.Singleton.IsServer)
            {
                SpawnAndAttachSniper(snakeHead);
                StartShooting(snakeHead);
            }
            else if (!NetworkManager.Singleton.IsClient)
            {
                AttachSniperLocal(snakeHead);
            }
        }
        
        private void SpawnAndAttachSniper(GameObject snakeHead)
        {
            var snakeHeadNetworkObject = snakeHead.GetComponent<NetworkObject>();
            if (snakeHeadNetworkObject == null)
            {
                Debug.LogError("Snake head doesn't have a NetworkObject component!");
                return;
            }
            
            var sniperInstance = Instantiate(sniperPrefab);
            var sniperNetworkObject = sniperInstance.GetComponent<NetworkObject>();
            sniperNetworkObject.SpawnWithOwnership(snakeHeadNetworkObject.OwnerClientId);
            
            AttachSniperLocal(snakeHead, sniperInstance);
            AttachSniperClientRpc(snakeHeadNetworkObject.NetworkObjectId, sniperNetworkObject.NetworkObjectId);
        }
        
        [ClientRpc]
        private void AttachSniperClientRpc(ulong snakeHeadId, ulong sniperId)
        {
            if (NetworkManager.Singleton.IsServer) return;
            
            if (NetworkManager.Singleton.SpawnManager == null)
            {
                Debug.LogError("SpawnManager is not initialized!");
                return;
            }
            
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(snakeHeadId, out var snakeHeadObject) &&
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(sniperId, out var sniperObject))
            {
                AttachSniperLocal(snakeHeadObject.gameObject, sniperObject.gameObject);
            }
        }
        
        private void AttachSniperLocal(GameObject snakeHead, GameObject sniperInstance = null)
        {
            if (sniperInstance == null)
            {
                sniperInstance = Instantiate(sniperPrefab);
            }
            
            sniperInstance.transform.SetParent(snakeHead.transform);
            sniperInstance.transform.localPosition = new Vector3(0, offsetY, 0);
            sniperInstance.transform.localRotation = Quaternion.identity;
            
            currentSniper = sniperInstance;
        }
        
        public override void Deactivate()
        {
            DeactivateShooting();
            
            if (currentSniper != null)
            {
                if (currentSniper.TryGetComponent<NetworkObject>(out var networkObject))
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        networkObject.Despawn();
                    }
                }
                else
                {
                    Destroy(currentSniper);
                }
                currentSniper = null;
            }
        }
    
        private void StartShooting(GameObject snake)
        {
            isShootingActive = true;
            SnakePowerManager powerManager = snake.GetComponent<SnakePowerManager>();
            if (powerManager != null)
            {
                // Start both tracking and shooting coroutines
                trackingCoroutine = powerManager.StartCoroutine(ContinuousTargetTracking());
                shootingCoroutine = powerManager.StartCoroutine(HandleShooting(powerManager));
            }
        }
        
        private IEnumerator ContinuousTargetTracking()
        {
            while (isShootingActive)
            {
                FindTarget();
                
                if (currentTarget != null && currentSniper != null)
                {
                    // Calculate direction to target
                    Vector2 direction = currentTarget.position - currentSniper.transform.position;
                    float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
                    
                    // Smoothly rotate towards target
                    currentSniper.transform.rotation = Quaternion.Slerp(
                        currentSniper.transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
                
                yield return null;
            }
        }
        
        private IEnumerator HandleShooting(SnakePowerManager powerManager)
        {
            while (isShootingActive)
            {
                if (currentTarget != null)
                {
                    // Wait until we're roughly facing the target (optional)
                    yield return new WaitUntil(() => IsFacingTarget() || currentTarget == null);
                    
                    if (currentTarget != null)
                    {
                        Shoot(powerManager);
                    }
                }
                
                yield return new WaitForSeconds(shootInterval);
            }
        }

        private bool IsFacingTarget()
        {
            if (currentTarget == null || currentSniper == null) return false;
            
            Vector2 directionToTarget = (currentTarget.position - currentSniper.transform.position).normalized;
            Vector2 sniperForward = currentSniper.transform.right; // Assuming sniper faces right
            
            // Check if angle is within acceptable threshold (e.g., 10 degrees)
            return Vector2.Angle(sniperForward, directionToTarget) < 10f;
        }

        private void FindTarget()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float closestDistance = Mathf.Infinity;
            GameObject closestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(currentSniper.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            currentTarget = closestEnemy?.transform;
        }

        private void Shoot(SnakePowerManager powerManager)
        {
            if (!powerManager.IsServer || currentSniper == null || currentTarget == null) return;

            Vector2 spawnPosition = (Vector2)currentSniper.transform.position + 
                                  (Vector2)currentSniper.transform.TransformDirection(projectileSpawnOffset);
            
            Quaternion spawnRotation = currentSniper.transform.rotation;

            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);
            NetworkObject projectileNetObj = projectile.GetComponent<NetworkObject>();
            projectileNetObj.Spawn();

            SniperBullet projectileScript = projectile.GetComponent<SniperBullet>();
            if (projectileScript != null)
            {
                projectileScript.SetSpeed(projectileSpeed);
                projectileScript.SetTarget(currentTarget);
            }
        }

        public void DeactivateShooting()
        {
            isShootingActive = false;
            currentTarget = null;
            
            // Stop coroutines if they're running
            if (shootingCoroutine != null)
            {
                //SnakePowerManager.Instance.StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
            
            if (trackingCoroutine != null)
            {
                //SnakePowerManager.Instance.StopCoroutine(trackingCoroutine);
                trackingCoroutine = null;
            }
        }
    }
}