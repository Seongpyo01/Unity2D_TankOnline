using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coinPrefab;
    [SerializeField] private int maxCoin = 50;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;
    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    private GameObject parent;

    private float coinRadius;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) { return; }
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for(int i= 0; i < maxCoin; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.SetCoinValue(coinValue);
        coinInstance.transform.SetParent(this.transform);

        coinInstance.OnCollected += GetHandleCoinCollected;
    }

    private void GetHandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        Vector2 coinSpawnPoint;

        while (true)
        {
            coinSpawnPoint.x = Random.Range(xSpawnRange.x, ySpawnRange.x);
            coinSpawnPoint.y = Random.Range(xSpawnRange.y, ySpawnRange.y);
            int numColliders = Physics2D.OverlapCircleNonAlloc(coinSpawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0)
            {
                return coinSpawnPoint;
            }
        }
    }
}
