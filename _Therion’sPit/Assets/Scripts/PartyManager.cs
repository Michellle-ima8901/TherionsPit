using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;


public class PartyManager : NetworkBehaviour
{
    public List<GameObject> characterPrefabs; 
    public string serverPassword = ""; 
    private Dictionary<ulong, int> lockedInRoles = new Dictionary<ulong, int>();

    

    public override void OnNetworkSpawn()
    {
        
        if (IsServer)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LockInRoleServerRpc(int roleIndex, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        
       
        foreach (var role in lockedInRoles.Values)
        {
            if (role == roleIndex)
            {
                Debug.LogWarning($"Client {clientId} tried to pick role {roleIndex}, but it's already taken!");
          
                RoleTakenClientRpc(rpcParams.Receive.SenderClientId);
                return; 
            }
        }

       
        lockedInRoles[clientId] = roleIndex;
        Debug.Log($"Client {clientId} successfully locked in role {roleIndex}.");
    }

    [ClientRpc]
    private void RoleTakenClientRpc(ulong clientId)
    {
       
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
           
            MenuButtons ui = FindObjectOfType<MenuButtons>();
            if (ui != null)
            {
                ui.ShowError("That character is already taken! Pick another.");
            }
        }
    }

    [ServerRpc(RequireOwnership = true)] 
    public void StartGameServerRpc()
    {
        Debug.Log("Host started the game! Spawning everyone...");

        foreach (var kvp in lockedInRoles)
        {
            ulong clientId = kvp.Key;
            int roleIndex = kvp.Value;

            GameObject prefabToSpawn = characterPrefabs[roleIndex];
            GameObject instance = Instantiate(prefabToSpawn);
            
            
            instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
        
  
        lockedInRoles.Clear();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
 
        string clientPassword = System.Text.Encoding.UTF8.GetString(request.Payload);
        if (clientPassword != serverPassword)
        {
            response.Approved = false;
            response.Reason = "Incorrect Password!";
            return;
        }

   
        response.Approved = true;
        response.CreatePlayerObject = false; 

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelection", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        Debug.Log($"Client {request.ClientNetworkId} approved. Moving to Character Select.");
    }
}