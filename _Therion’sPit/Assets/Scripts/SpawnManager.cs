using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject magePrefab;
    [SerializeField] private GameObject knightPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        string roleString = System.Text.Encoding.UTF8.GetString(request.Payload);
        PlayerRoles role = System.Enum.Parse<PlayerRoles>(roleString);

        response.Approved = true;
        response.CreatePlayerObject = true;

        // Pick prefab based on role
        GameObject prefab = role == PlayerRoles.Mage ? magePrefab : knightPrefab;
        response.PlayerPrefabHash = prefab.GetComponent<NetworkObject>().PrefabIdHash;
    }
}


