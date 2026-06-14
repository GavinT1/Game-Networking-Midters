using UnityEngine;
using Unity.Netcode;

public class CoinSpawner : NetworkBehaviour
{
    public static CoinSpawner Instance;

    [Header("Coin Setup")]
    public GameObject coinPrefab;    
    public int numberOfCoins = 20;   

    [Header("Arena Boundaries")]
    public float minX = -32f;        
    public float maxX = 32f;         
    public float minZ = -32f;
    public float maxZ = 32f;
    public float spawnHeight = 0.5f; 

    void Awake()
    {
        Instance = this;
    }

    public void SpawnAllCoins()
    {
        if (!IsServer || coinPrefab == null) return;

        if (coinPrefab.GetComponent<NetworkObject>() == null)
        {
            Debug.LogError("Your Coin Prefab is missing a NetworkObject component!");
            return;
        }

        for (int i = 0; i < numberOfCoins; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

            GameObject spawnedCoin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity, transform);
            
            spawnedCoin.GetComponent<NetworkObject>().Spawn();
        }
    }
}