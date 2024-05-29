using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBoxAbility : MonoBehaviour
{
    [Header("KillingBox Drop Setup")]
    [SerializeField] private Transform _killingBox;
    [SerializeField] private Transform _killingBoxSpawnPoint;
    [SerializeField] private Transform _KillingBoxesParent;

    [Header("Ground Distance Check")]
    [SerializeField] private float _minDistanceFromGround;
    [SerializeField] private bool _canDropBox;
    [SerializeField] private LayerMask _whatIsGround;

    [SerializeField] private float _dropForce;

    public void TryDropKillingBox()
    {
        if (!_canDropBox) return;

        Transform killingBoxInstance = Instantiate(_killingBox, _killingBoxSpawnPoint.position, Quaternion.identity, _KillingBoxesParent);

        KillingBox killingBox = killingBoxInstance.GetComponent<KillingBox>();
        killingBox.Drop(_dropForce);
    }

    private void Update()
    {
        UpdateCanDropBox();
    }

    private void UpdateCanDropBox()
    {
        _canDropBox = !Physics2D.Raycast(_killingBoxSpawnPoint.position, Vector2.down, _minDistanceFromGround, _whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_killingBoxSpawnPoint.position, new Vector3(_killingBoxSpawnPoint.position.x, _killingBoxSpawnPoint.position.y - _minDistanceFromGround));
    }
}
