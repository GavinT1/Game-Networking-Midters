using UnityEngine;
using Unity.Netcode; // Essential for NetworkBehaviour

public class CoinSpawner : NetworkBehaviour
{
    [Header("Coin Setup")]
    public GameObject coinPrefab;    
    public int numberOfCoins = 20;   

    [Header("Arena Boundaries")]
    public float minX = -32f;        
    public float maxX = 32f;         
    public float minZ = -32f;
    public float maxZ = 32f;
    public float spawnHeight = 0.5f; 

    // Changed from Start() to OnNetworkSpawn()
    public override void OnNetworkSpawn()
    {
        // ONLY the Host runs this logic. Clients will automatically receive the spawned objects!
        if (!IsServer) return; 

        SpawnAllCoins();
    }

    void SpawnAllCoins()
    {
        if (coinPrefab == null) return;

        for (int i = 0; i < numberOfCoins; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

            // 1. Instantiate the coin locally on the host
            GameObject spawnedCoin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform);
            
            // 2. CRITICAL: This tells Netcode to spawn it on Player 2's screen instantly!
            spawnedCoin.GetComponent<NetworkObject>().Spawn();
        }
    }
}