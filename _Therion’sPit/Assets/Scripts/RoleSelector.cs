using UnityEngine;
using Unity.Netcode;

public class RoleSelector : MonoBehaviour
{
    private int selectedRole = 0; 
    private bool hasLockedIn = false; 

    void OnGUI()
    {
        if (NetworkManager.Singleton == null) return;

        // --- 1. THE CONNECTION MENU ---
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            GUILayout.BeginArea(new Rect(10, 10, 250, 300));
            GUILayout.Label("Main Menu");
            if (GUILayout.Button("Host Game")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Join Game")) NetworkManager.Singleton.StartClient();
            GUILayout.EndArea();
            return;
        }

        // --- 2. THE WAITING SCREEN ---
        if (hasLockedIn)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.Label("Character locked in!");
            GUILayout.Label("Waiting for Host to start the game...");

            // Only show this if we are the Host (Server)
            if (NetworkManager.Singleton.IsServer)
            {
                GUILayout.Space(20);
                if (GUILayout.Button("START GAME"))
                {
                    PartyManager partyManager = FindObjectOfType<PartyManager>();
                    if (partyManager != null)
                    {
                        partyManager.StartGameServerRpc();
                    }
                    else
                    {
                        Debug.LogError("StartGame Failed: PartyManager not found in scene!");
                    }
                }
            }
            GUILayout.EndArea();
            return;
        }

        // --- 3. THE LOBBY MENU ---
        GUILayout.BeginArea(new Rect(10, 10, 300, 600));
        
        if (NetworkManager.Singleton.IsServer)
            GUILayout.Label("You are the Party Host.");
        else
            GUILayout.Label("You have joined the Party.");

        GUILayout.Space(20);
        GUILayout.Label("Choose Your Character:");
        
        if (GUILayout.Toggle(selectedRole == 0, "Mage")) selectedRole = 0;
        if (GUILayout.Toggle(selectedRole == 1, "Knight")) selectedRole = 1;
        if (GUILayout.Toggle(selectedRole == 2, "Ninja")) selectedRole = 2;
        if (GUILayout.Toggle(selectedRole == 3, "Archer")) selectedRole = 3;

        GUILayout.Space(20);
        if (GUILayout.Button("Lock In Character"))
        {
            PartyManager partyManager = FindObjectOfType<PartyManager>();
            
            // Safety checks to prevent the NullReferenceException
            if (partyManager == null)
            {
                Debug.LogError("LockIn Failed: PartyManager script not found in the scene!");
            }
            else if (!partyManager.IsSpawned)
            {
                Debug.LogError("LockIn Failed: PartyManager is not spawned on the network yet!");
            }
            else
            {
                partyManager.LockInRoleServerRpc(selectedRole);
                hasLockedIn = true; 
            }
        }

        GUILayout.EndArea();
    }
}