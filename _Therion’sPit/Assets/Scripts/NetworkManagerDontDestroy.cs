using UnityEngine;
using Unity.Netcode;

public class NetworkManagerDontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
