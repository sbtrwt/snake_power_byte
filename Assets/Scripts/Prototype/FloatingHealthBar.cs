using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class FloatingHealthBar : MonoBehaviour
{
    [Tooltip("The UI Image that serves as the fill (must use Image Type = Filled).")]
    [SerializeField] private Image fillImage;
    [Tooltip("Offset above the target (in world units).")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);

    [SerializeField]private Transform target;          // The object this bar follows.
    [SerializeField]private Health targetHealth;       // The Health component of the target.
    [SerializeField]private Camera mainCamera;

    /// <summary>
    /// Initializes the floating health bar with the target to follow.
    /// </summary>
    /// <param name="target">The transform of the target (snake/enemy).</param>
    /// <param name="health">The Health component of the target.</param>
    public void Initialize(Transform target, Health health)
    {
        this.target = target;
        this.targetHealth = health;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        // Update the position above the target.
        transform.position = target.position + offset;
        // Make the health bar face the camera (billboarding).
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }

        // Update fill based on current health.
        if (targetHealth != null && targetHealth.MaxHealth > 0)
        {
            fillImage.fillAmount = targetHealth.CurrentHealth.Value / targetHealth.MaxHealth;
        }
    }
}
