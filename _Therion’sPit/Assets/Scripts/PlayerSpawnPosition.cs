using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using KinematicCharacterController;

public class PlayerSpawnPosition : NetworkBehaviour
{
    public override void OnNetworkSpawn()
{
    if (!IsServer) return;

    var spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    if (spawnPoints.Length == 0) return;

    int index = (int)(OwnerClientId % (ulong)spawnPoints.Length);
    Vector3 spawnPos = spawnPoints[index].transform.position;
    Quaternion spawnRot = spawnPoints[index].transform.rotation;

    // Teleport the KCC motor properly
    if (TryGetComponent<KinematicCharacterMotor>(out var motor))
    {
        motor.SetPositionAndRotation(spawnPos, spawnRot, true);
    }
    else
    {
        transform.position = spawnPos;
        transform.rotation = spawnRot;
    }

    if (TryGetComponent<NetworkTransform>(out var nt))
    {
        nt.Teleport(spawnPos, spawnRot, transform.localScale);
    }
}

}