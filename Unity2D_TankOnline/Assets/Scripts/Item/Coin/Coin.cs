using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Coin : NetworkBehaviour
{
    protected SpriteRenderer spriteRenderer;
    
    protected int coinValue;
    protected bool alreadyCollected;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public abstract int CoinCollect();

    public void SetCoinValue(int value)
    {
        coinValue = value;
    }

    protected void DisplayCoin(bool show)
    {
        spriteRenderer.enabled = show;
    }

}
