using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : MonoBehaviour
{
    void Start()
    {
        // Register the callback to handle approval
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // For now, just approve everyone so the timeout warning goes away
        response.Approved = true;
        response.CreatePlayerObject = true; 
        
        // Note: You will eventually read the player's role choice from 'request.Payload' here
        // and set response.PlayerPrefabHash to the Mage or Knight prefab's ID.
    }
}