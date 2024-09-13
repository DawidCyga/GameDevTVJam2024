using UnityEngine;

public class ForestManager : MonoBehaviour
{
    [Header("Tree Management")]
    [SerializeField] public Transform[] treeSpawnpoints;
    [SerializeField] public GameObject treePrefab;

    void Start() => spawnTrees();
    private void spawnTrees()
    {
        for (int i = 0; i < treeSpawnpoints.Length; i++)
        {
            Instantiate(treePrefab, treeSpawnpoints[i].position, Quaternion.identity);
        }
    }
}
