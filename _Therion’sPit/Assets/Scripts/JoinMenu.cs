using UnityEngine;
using UnityEngine.UI;
using TMPro;          
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class JoinMenu : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField joinCodeInput; 
    public TMP_InputField passwordInput; 
    public Button joinButton;            
    public TMP_Text statusText;          

    private async void Start()
    {
        
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        if (statusText != null) statusText.text = "";
        
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    private async void OnJoinClicked()
    {
        if (string.IsNullOrEmpty(joinCodeInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            if (statusText != null) statusText.text = "Please enter a Code and Password.";
            return;
        }

        joinButton.interactable = false;
        if (statusText != null) statusText.text = "Connecting...";

        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls")
            );

            NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(passwordInput.text);
            NetworkManager.Singleton.StartClient();
            
            if (statusText != null) statusText.text = "Connecting to Host...";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to join: {e.Message}");
            if (statusText != null) statusText.text = "Error: Invalid Code or Host Offline.";
            joinButton.interactable = true; 
        }
    }
}