using UnityEngine;
using Unity.Netcode;

public class RoleSelector : MonoBehaviour
{
    private void OnGUI()
    {
        if (NetworkManager.Singleton == null) return;

        // Only show menu if not connected
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
            return;

        GUILayout.BeginArea(new Rect(10, 10, 220, 300));

        GUILayout.Label("Choose your role:");

        if (GUILayout.Button("Mage", GUILayout.Height(40)))
        {
            ConnectAsRole(PlayerRoles.Mage);
        }

        if (GUILayout.Button("Knight", GUILayout.Height(40)))
        {
            ConnectAsRole(PlayerRoles.Knight);
        }

        GUILayout.Space(20);
        GUILayout.Label("--- OR ---");

        // For server testing — also let the host pick a role
        if (GUILayout.Button("Host as Mage", GUILayout.Height(30)))
        {
            HostAsRole(PlayerRoles.Mage);
        }
        if (GUILayout.Button("Host as Knight", GUILayout.Height(30)))
        {
            HostAsRole(PlayerRoles.Knight);
        }

        GUILayout.EndArea();
    }

    private void ConnectAsRole(PlayerRoles role)
    {
        // Send the role index to the server via connection data
        NetworkManager.Singleton.NetworkConfig.ConnectionData =
            System.BitConverter.GetBytes((int)role);

        NetworkManager.Singleton.StartClient();
    }

    private void HostAsRole(PlayerRoles role)
    {
        // Host also needs to send its role
        NetworkManager.Singleton.NetworkConfig.ConnectionData =
            System.BitConverter.GetBytes((int)role);

        NetworkManager.Singleton.StartHost();
        if (NetworkManager.Singleton.IsHost)
        {
            // We call a method on our ServerConnectionHandler to spawn the host's player
            var handler = FindObjectOfType<ServerConnectionHandler>();
            if (handler != null)
            {
                handler.SpawnHostPlayer();
            }
        }
    }
}