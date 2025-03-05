using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    
    [SerializeField]private Button startHostButton;
    [SerializeField]private Button startClientButton;
    private void Awake()
    {
        startHostButton.onClick.AddListener(StartHost);
        startClientButton.onClick.AddListener(StartClient);
    }
    private void StartHost()
    {
        Debug.Log("Start Host");
        NetworkManager.Singleton.StartHost();
        HideUI();
    }
    private void StartClient()
    {
        Debug.Log("Start Client");
        NetworkManager.Singleton.StartClient();
        HideUI();
    }
    private void HideUI()
    {
        //startHostButton.gameObject.SetActive(false);
        //startClientButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
