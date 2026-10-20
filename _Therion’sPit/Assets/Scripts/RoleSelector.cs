using UnityEngine;
using Unity.Netcode;

public class RoleSelector : MonoBehaviour
{
    void OnGUI()
    {
        if (NetworkManager.Singleton == null) return;

        // Only show buttons if we aren't connected yet
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUI.Button(new Rect(10, 10, 150, 40), "Host"))
            {
                NetworkManager.Singleton.StartHost();
            }
            if (GUI.Button(new Rect(10, 60, 150, 40), "Client"))
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}