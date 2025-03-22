using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Collections;

public class FloatingText : NetworkBehaviour
{
    [Tooltip("Speed at which the text moves upward.")]
    public float moveSpeed = 1f;

    [Tooltip("Duration over which the text fades out.")]
    public float fadeDuration = 1f;

    [SerializeField]private TextMeshPro textMesh;

    // Network variables to synchronize text and color
    [SerializeField]private NetworkVariable<FixedString32Bytes> networkText = new NetworkVariable<FixedString32Bytes>();
    [SerializeField]private NetworkVariable<Color> networkColor = new NetworkVariable<Color>();
   [SerializeField] private NetworkVariable<float> networkAlpha = new NetworkVariable<float>(1f);

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Initialize text and color from network variables
        textMesh.text = networkText.Value.ToString();
        textMesh.color = networkColor.Value;

        // Subscribe to changes in network variables
        networkText.OnValueChanged += OnTextChanged;
        networkColor.OnValueChanged += OnColorChanged;
        networkAlpha.OnValueChanged += OnAlphaChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        //Debug.Log("FloatingText spawned on the network.");

        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is missing!");
            return;
        }

        textMesh.text = networkText.Value.ToString();
        textMesh.color = networkColor.Value;

        //Debug.Log("TextMeshPro text set to: " + textMesh.text);
        //Debug.Log("TextMeshPro color set to: " + textMesh.color);
        // Unsubscribe from network variable changes
        networkText.OnValueChanged -= OnTextChanged;
        networkColor.OnValueChanged -= OnColorChanged;
        networkAlpha.OnValueChanged -= OnAlphaChanged;
    }

    public void Initialize(string text, Color color)
    {
        //Debug.Log("Initializing FloatingText with text: " + text);
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component is missing!");
            return;
        }
         textMesh.text = text;
        textMesh.color = color;
        // Debug.Log("TextMeshPro text set to: " + textMesh.text);
        //Debug.Log("TextMeshPro color set to: " + textMesh.color);
        // Set the text and color on the server
       if (IsSpawned)
        {
            networkText.Value = new FixedString32Bytes(text);
            networkColor.Value = color;
            networkAlpha.Value = 1f; // Reset alpha
        }
        else
        {
            Debug.LogWarning("NetworkObject is not spawned yet. Delaying NetworkVariable update.");
        }

    }

    private void OnTextChanged(FixedString32Bytes oldText, FixedString32Bytes newText)
    {
        // Update the text when the network variable changes
        //Debug.Log("Text changed to: " + newText);
        textMesh.text = newText.ToString();
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        // Update the color when the network variable changes
        textMesh.color = newColor;
    }

    private void OnAlphaChanged(float oldAlpha, float newAlpha)
    {
        // Update the alpha when the network variable changes
        Color c = textMesh.color;
        c.a = newAlpha;
        textMesh.color = c;
    }

    private void Update()
    {
        if (!IsServer) return; // Only the server handles movement and fading

        // Move upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out over time
        float newAlpha = networkAlpha.Value - (Time.deltaTime / fadeDuration);
        networkAlpha.Value = Mathf.Max(newAlpha, 0f);

        // Destroy the object when fully faded out
        if (networkAlpha.Value <= 0f)
        {
            Destroy(gameObject);
            gameObject.GetComponent<NetworkObject>().Despawn(true);
        }
    }
}