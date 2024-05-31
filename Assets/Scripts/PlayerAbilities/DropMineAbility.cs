using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMineAbility : MonoBehaviour
{
    [Header("SlowingMine Drop Setup")]
    [SerializeField] private Transform _slowingMine;
    [SerializeField] private Transform _slowingMineSpawnPoint;
    [SerializeField] private Transform _slowingMinesParent;

    [Header("Ground Distance Check")]
    [SerializeField] private float _minDistanceFromGround;
    [SerializeField] private bool _canDropBox;
    [SerializeField] private LayerMask _whatIsGround;

    [SerializeField] private float _dropForce;

    public void TryDropSlowingMine()
    {
        if (!_canDropBox) return;

        Transform killingBoxInstance = Instantiate(_slowingMine, _slowingMineSpawnPoint.position, Quaternion.identity, _slowingMinesParent);

        SlowingMine killingBox = killingBoxInstance.GetComponent<SlowingMine>();
        killingBox.Drop(_dropForce);
    }

    private void Update()
    {
        UpdateCanDropBox();
    }

    private void UpdateCanDropBox()
    {
        _canDropBox = !Physics2D.Raycast(_slowingMineSpawnPoint.position, Vector2.down, _minDistanceFromGround, _whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_slowingMineSpawnPoint.position, new Vector3(_slowingMineSpawnPoint.position.x, _slowingMineSpawnPoint.position.y - _minDistanceFromGround));
    }
}
