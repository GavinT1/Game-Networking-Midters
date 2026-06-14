using UnityEngine;
using Unity.Netcode; // This tells the script how to find Netcode commands

public class Coin : NetworkBehaviour // Changed from MonoBehaviour to NetworkBehaviour
{
    public int pointValue = 1;

    void OnTriggerEnter(Collider other)
    {
        // Make sure we are only calculating this on the server/host to avoid double-scoring!
        if (!NetworkManager.Singleton.IsServer) return;

        // NEW FIX: Look for the NetworkObject component instead of checking tags
        NetworkObject playerNetworkObject = other.gameObject.GetComponent<NetworkObject>();

        // If the object that touched us is an active network player object, process the score!
        if (playerNetworkObject != null)
        {
            // Grab their unique network connection ID (0 for P1, 1 for P2)
            ulong scorerId = playerNetworkObject.OwnerClientId;

            // Pass the numerical ID directly to the updated GameManager
            GameManager.Instance.AddScore(scorerId, pointValue);
            
            // Destroy the coin across the network safely for everyone
            GetComponent<NetworkObject>().Despawn();
        }
    }
}