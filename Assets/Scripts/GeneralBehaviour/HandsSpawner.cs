using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsSpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [SerializeField] private Transform _objectToSpawn;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _spawnObjectParent;

    public void Spawn()
    {
        Transform handInstance = Instantiate(_objectToSpawn, _spawnPoint.position, Quaternion.identity, _spawnObjectParent);
        BabaiHand babaiHand = handInstance.GetComponent<BabaiHand>();
        babaiHand.AssignBabai(transform);
    }
}
