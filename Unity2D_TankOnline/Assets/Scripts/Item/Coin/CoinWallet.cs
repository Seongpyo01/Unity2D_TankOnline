using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [HideInInspector] public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Coin>(out Coin coin)) return;

        int coinValue = coin.CoinCollect();

        if (!IsServer) return;

        totalCoins.Value += coinValue;
    }

    public void SpendCoins(int costToFire)
    {
        totalCoins.Value -= costToFire;
    }
}
