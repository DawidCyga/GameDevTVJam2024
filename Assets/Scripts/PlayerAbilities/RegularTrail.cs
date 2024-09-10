using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularTrail : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _timeToSelfDestruct;

    [Header("For Debugging Only")]
    [SerializeField] private float _timeSinceAlive;

    private void Update()
    {
        _timeSinceAlive += Time.deltaTime;

        DestroyAfterTime();
    }

    private void DestroyAfterTime()
    {
        if (_timeSinceAlive > _timeToSelfDestruct)
        {
            Destroy(gameObject);
        }
    }
}
