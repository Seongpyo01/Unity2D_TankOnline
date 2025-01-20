using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{

    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;

    private Camera mainCamera;

    private void LateUpdate()
    {
        if (!IsOwner) return;

        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector2 aimScreenPosition = inputReader.aimPosition;
        Vector2 aimWorldPosition = mainCamera.ScreenToWorldPoint(aimScreenPosition);

        turretTransform.up = aimWorldPosition - (Vector2)turretTransform.position;
    }

   
}
