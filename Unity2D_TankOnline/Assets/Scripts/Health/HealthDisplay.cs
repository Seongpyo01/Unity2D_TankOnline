using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HealthDisplay : NetworkBehaviour
{
    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    [SerializeField] private Image healthBarImage;

    private Health health;

    private void Awake()
    {
        health = GetComponentInParent<Health>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        health.currentHealth.OnValueChanged += HandleHealthChaged;
        HandleHealthChaged(0, health.currentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        
        health.currentHealth.OnValueChanged -= HandleHealthChaged;
    }

    private void HandleHealthChaged(int oldHealth, int newHealth)
    {
        healthBarImage.fillAmount = (float) newHealth / health.maxHealth;
    }
}
