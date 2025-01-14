using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class PlayerMovement : NetworkBehaviour
{
    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;

    #region Header Settings
    [Space(10)]
    [Header("Settings")]
    #endregion
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turningRate = 30f;

    private Rigidbody2D rb;
    private Vector2 previousDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.moveEvent += HandleMove; 
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.moveEvent -= HandleMove; 
    }

    private void Update()
    {
        if (!IsOwner) return;

        float rotate = previousDir.x * -turningRate * Time.deltaTime;
        bodyTransform.Rotate(0f, 0f, rotate);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.velocity = (Vector2)bodyTransform.up * previousDir.y * moveSpeed;
    }

    public void HandleMove(Vector2 direction)
    {
        previousDir = direction;
    }


}
