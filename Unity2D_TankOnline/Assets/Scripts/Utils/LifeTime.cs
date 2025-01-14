using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class LifeTime : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;

    private void Start()  
    {
        Destroy(gameObject, lifeTime);
    }

}
