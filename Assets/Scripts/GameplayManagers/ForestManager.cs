using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestManager : MonoBehaviour
{
    [Header("Tree Management")]
    [SerializeField] public Transform[] treeSpawnpoints;
    [SerializeField] public GameObject treePrefab;

    // Start is called before the first frame update
    void Start()
    {
        spawnTrees();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void spawnTrees()
    {
        for (int i = 0; i < treeSpawnpoints.Length; i++)
        {
            Instantiate(treePrefab, treeSpawnpoints[i].position, Quaternion.identity);
        }
    }
}
