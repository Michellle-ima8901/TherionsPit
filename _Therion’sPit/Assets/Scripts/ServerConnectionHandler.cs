using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ServerConnectionHandler : NetworkBehaviour
{
    [SerializeField] private List<GameObject> playerPrefabs;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        // Read the role index sent by the client
        int roleIndex = System.BitConverter.ToInt32(request.Payload, 0);

        if (roleIndex < 0 || roleIndex >= playerPrefabs.Count)
        {
            Debug.LogError($"Invalid role index: {roleIndex}");
            response.Approved = false;
            response.Reason = "Invalid role selection";
            return;
        }

        GameObject prefab = playerPrefabs[roleIndex];
        NetworkObject netObj = prefab.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError($"Prefab at index {roleIndex} has no NetworkObject");
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = netObj.PrefabIdHash;
        response.Position = Vector3.zero;   // server will move it via PlayerSpawnPosition
        response.Rotation = Quaternion.identity;
    }



    public void SpawnHostPlayer()
    {
        if (!IsServer) return;

        // Read the role from connection data just like the callback would
        int roleIndex = System.BitConverter.ToInt32(NetworkManager.Singleton.NetworkConfig.ConnectionData, 0);
        
        if (roleIndex >= 0 && roleIndex < playerPrefabs.Count)
        {
            GameObject prefab = playerPrefabs[roleIndex];
            var instance = Instantiate(prefab);
            var netObj = instance.GetComponent<NetworkObject>();
            
            // Spawn it for the host's client ID
            netObj.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        }
    }
}


