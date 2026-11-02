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

public class CreateMenu : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField passwordInput; 
    public TMP_Text joinCodeDisplay;     

    [Header("Spawning")]
    public GameObject partyManagerPrefab; 

    private string generatedCode = "";
    public Button createButton;
    private bool isGenerating = false;

    async void Start()
    {
        createButton.onClick.AddListener(OnCreateButtonClicked);
        if (isGenerating) return;
        isGenerating = true;

        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        joinCodeDisplay.text = "Generating code...";

        
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3); 
            generatedCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls")
            );

            joinCodeDisplay.text = $"JOIN CODE:\n<size=150%>{generatedCode}</size>";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to generate code: {e.Message}");
            joinCodeDisplay.text = "Error generating code.";
        }

    }

    public void OnCreateButtonClicked()
    {
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.LogWarning("Please enter a password.");
            return;
        }


        NetworkManager.Singleton.StartHost();
        GameObject pm = Instantiate(partyManagerPrefab);
        pm.GetComponent<NetworkObject>().Spawn(true);

        pm.GetComponent<PartyManager>().serverPassword = passwordInput.text; 

        //NetworkManager.Singleton.StartHost();
        Debug.Log("Host started with password: " + passwordInput.text);
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelection");
        
    }
}