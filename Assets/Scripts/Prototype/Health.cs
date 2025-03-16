using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    
    // The current health is replicated to all clients.
    public NetworkVariable<float> CurrentHealth = new NetworkVariable<float>(
        100f, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    private void Start()
    {
        if (IsServer)
        {
            CurrentHealth.Value = maxHealth;
        }
    }

    /// <summary>
    /// Reduces health by the specified damage amount.
    /// </summary>
    /// <param name="damage">The damage amount.</param>
    public void TakeDamage(float damage)
    {
        if (!IsServer) return;

        CurrentHealth.Value = Mathf.Max(CurrentHealth.Value - damage, 0);
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {CurrentHealth.Value}");
        
        if (CurrentHealth.Value <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Increases health by the specified amount, up to maxHealth.
    /// </summary>
    /// <param name="amount">The healing amount.</param>
    public void Heal(float amount)
    {
        if (!IsServer) return;

        CurrentHealth.Value = Mathf.Min(CurrentHealth.Value + amount, maxHealth);
        Debug.Log($"{gameObject.name} healed by {amount}. Current health: {CurrentHealth.Value}");
       
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Insert additional death logic here (e.g., play animation, notify game manager).
        GetComponent<NetworkObject>().Despawn();
    }
}
