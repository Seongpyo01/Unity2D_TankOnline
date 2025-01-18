using System;
using System.Collections;
using UnityEngine;

public class RespawningCoin : Coin
{
    public event Action<RespawningCoin> OnCollected;

    private Vector3 previousPosition;

    private void Update()
    {
        if (previousPosition != transform.position)
        {
            DisplayCoin(true);
        }

        previousPosition = transform.position;
    }

    public override int CoinCollect()
    {
        if (!IsServer)
        {
            DisplayCoin(false);
            return 0;
        }
        
        if (alreadyCollected) return 0;

        alreadyCollected = true;

        OnCollected?.Invoke(this);

        return coinValue;
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}