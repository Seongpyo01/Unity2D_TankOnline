using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class ProjectileLauncher : NetworkBehaviour
{
    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;

    #region Header Settings
    [Space(10)]
    [Header("Settings")]
    #endregion
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;
    [SerializeField] private int costToFire;

    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;
    private Collider2D playerCollider;
    private CoinWallet coinWallet;

    private void Awake()
    {
        playerCollider = GetComponentInChildren<Collider2D>();
        coinWallet = GetComponent<CoinWallet>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.primaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        
        inputReader.primaryFireEvent -= HandlePrimaryFire;
    }

    private void Update()
    {
        if(muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if(muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) return;

        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (!shouldFire) return;

        // fireRate = the number of shots per second; firing interval is (1 / fireRate)added to previousFireTime to determine when to fire next
        
        if (timer > 0) return;

        if(coinWallet.totalCoins.Value < costToFire) return;

        coinWallet.SpendCoins(costToFire);

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1/fireRate;
    }

    public void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        GameObject projectileObject = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);

        projectileObject.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileObject.GetComponent<Collider2D>());

        if(projectileObject.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamageOnContact))
        {
            dealDamageOnContact.SetOwner(OwnerClientId);
        }

        if (projectileObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (IsOwner) return;

        SpawnDummyProjectile(spawnPos, direction);
        
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileObject = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectileObject.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileObject.GetComponent<Collider2D>());

        if (projectileObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }
}
