using UnityEngine;
using Unity.Netcode; 

public class Coin : NetworkBehaviour 
{
    public int pointValue = 1;

    void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        NetworkObject playerNetworkObject = other.gameObject.GetComponent<NetworkObject>();

        if (playerNetworkObject != null)
        {
            ulong scorerId = playerNetworkObject.OwnerClientId;

            GameManager.Instance.AddScore(scorerId, pointValue);
            
            GetComponent<NetworkObject>().Despawn();
        }
    }
}