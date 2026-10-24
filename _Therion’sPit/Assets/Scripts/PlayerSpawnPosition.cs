using UnityEngine;
using Unity.Netcode;
using KinematicCharacterController;

public class PlayerSpawnPosition : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // 1. Only the server should decide where players spawn
        if (!IsServer) return;

        // 2. Find all spawn points
        var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        
        if (spawnPoints.Length == 0) 
        {
            Debug.LogError("No spawn points found! Did you tag them?");
            return;
        }

        // 3. Pick a spawn point
        int index = (int)(OwnerClientId % (ulong)spawnPoints.Length);
        Vector3 spawnPos = spawnPoints[index].transform.position;
        Quaternion spawnRot = spawnPoints[index].transform.rotation;

        // --- THIS IS THE DEBUG LOG ---
        Debug.Log($"Teleporting Player {OwnerClientId} to SpawnPoint index {index} at {spawnPos}");
        

        // 4. Teleport the KCC Motor properly
        if (TryGetComponent<KinematicCharacterMotor>(out var motor))
        {
            motor.SetPositionAndRotation(spawnPos, spawnRot, true);
        }
        else
        {
            // Fallback if motor isn't on the root
            transform.position = spawnPos;
            transform.rotation = spawnRot;
        }
    }
}