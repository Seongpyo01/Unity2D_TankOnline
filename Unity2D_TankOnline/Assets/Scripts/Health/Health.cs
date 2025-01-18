using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int maxHealth { get; private set; } = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    private bool isDead;

    public Action<Health> onDie;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        currentHealth.Value = maxHealth;
    }

    public void TakeDamage(int DamageValue)
    {
        ModifyHealth(-DamageValue);
    }

    public void RestoreHealth(int healthValue)
    {
        ModifyHealth(healthValue);
    }

    private void ModifyHealth(int value)
    {
        if (isDead) return;

        int health = currentHealth.Value + value;
        currentHealth.Value = Math.Clamp(health, 0, maxHealth);

        if(currentHealth.Value == 0)
        {
            onDie?.Invoke(this);
            isDead = true; 
        }
    }
}
