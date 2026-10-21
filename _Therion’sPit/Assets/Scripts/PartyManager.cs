using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PartyManager : NetworkBehaviour
{
    public List<GameObject> characterPrefabs; 

    // This keeps track of who picked what before the game starts
    private Dictionary<ulong, int> lockedInRoles = new Dictionary<ulong, int>();

    // [ServerRpc(RequireOwnership = false)]
    // public void LockInRoleServerRpc(int roleIndex, ServerRpcParams rpcParams = default)
    // {
    //     ulong clientId = rpcParams.Receive.SenderClientId;
        
    //     // Save their choice, but DON'T spawn them yet
    //     lockedInRoles[clientId] = roleIndex;
    //     Debug.Log($"Client {clientId} locked in role {roleIndex}. Waiting for host to start...");
    // }
    [ServerRpc(RequireOwnership = false)]
    public void LockInRoleServerRpc(int roleIndex, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        
        // 1. Check if someone else already picked this character
        foreach (var role in lockedInRoles.Values)
        {
            if (role == roleIndex)
            {
                Debug.LogWarning($"Client {clientId} tried to pick role {roleIndex}, but it's already taken!");
                // Tell the client they failed
                RoleTakenClientRpc(rpcParams.Receive.SenderClientId);
                return; // Stop here, don't save the choice
            }
        }

        // 2. If we get here, the character is free. Save it.
        lockedInRoles[clientId] = roleIndex;
        Debug.Log($"Client {clientId} successfully locked in role {roleIndex}.");
    }

    [ClientRpc]
    private void RoleTakenClientRpc(ulong clientId)
    {
        // This only runs on the specific client that tried to pick the taken role
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            RoleSelector ui = FindObjectOfType<RoleSelector>();
            if (ui != null)
            {
                ui.ShowError("That character is already taken! Pick another.");
            }
        }
    }

    [ServerRpc(RequireOwnership = true)] // RequireOwnership = true means ONLY the Host can call this
    public void StartGameServerRpc()
    {
        Debug.Log("Host started the game! Spawning everyone...");

        foreach (var kvp in lockedInRoles)
        {
            ulong clientId = kvp.Key;
            int roleIndex = kvp.Value;

            GameObject prefabToSpawn = characterPrefabs[roleIndex];
            GameObject instance = Instantiate(prefabToSpawn);
            
            // Spawn the character and assign it to the correct player
            instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
        
        // Clear the list so it's ready for next time
        lockedInRoles.Clear();
    }
}