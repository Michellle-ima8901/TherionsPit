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

public class CreateMenu : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField passwordInput; // The password box the host types in
    public Button createButton;          // The "Create" button
    public TMP_Text joinCodeDisplay;     // A Text element to show the generated code

    [Header("Spawning")]
    public GameObject partyManagerPrefab; // Drag your PartyManager prefab here

    async void Start()
    {
        // Initialize Unity Services for Relay
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // Hide the code display until the party is created
        if (joinCodeDisplay != null) joinCodeDisplay.gameObject.SetActive(false);

        // Connect the button
        createButton.onClick.AddListener(OnCreateClicked);
    }

    private async void OnCreateClicked()
    {
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Debug.LogWarning("Please enter a password for your party.");
            return;
        }

        try
        {
            Debug.Log("Creating Party...");

            // 1. Create the Relay allocation
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3); // max 3 clients + host
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            // 2. Set the Transport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                AllocationUtils.ToRelayServerData(allocation, "dtls")
            );

            // 3. Spawn the PartyManager over the network
            GameObject pm = Instantiate(partyManagerPrefab);
            pm.GetComponent<NetworkObject>().Spawn(true);

            // 4. Save the password on the Server
            PartyManager pmScript = pm.GetComponent<PartyManager>();
            pmScript.serverPassword = passwordInput.text; 

            // 5. Start the Host
            NetworkManager.Singleton.StartHost();

            // 6. Show the Join Code to the player
            if (joinCodeDisplay != null)
            {
                joinCodeDisplay.text = $"Party Code: {relayJoinCode}";
                joinCodeDisplay.gameObject.SetActive(true);
            }
            
            Debug.Log($"Party Created! Code: {relayJoinCode}");
            
            // Hide the Create Menu UI
            gameObject.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create party: {e.Message}");
        }
    }
}