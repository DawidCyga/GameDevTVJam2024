using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarPathfinder : MonoBehaviour
{
    public static AStarPathfinder Instance { get; private set; }

    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _collisionsTilemap;

    [SerializeField] private List<PathNode> _openList;
    [SerializeField] private HashSet<PathNode> _closedHashSet;

    GridManager _gridManager;

    private void Awake()
    {
        Instance = this;
    }

    public void ClearReserved(Vector2Int position)
    {
        PathNode node = _gridManager.GetNodeAtTilemapGridPosition(position);
        node.SetOccupied(false);
    }

    public Stack<Vector3> BuildPath(Vector2Int startGridPosition, Vector2Int targetGridPosition)
    {
        if (_gridManager != null)
        {
            _gridManager.Reset(_collisionsTilemap);

        }

        _gridManager = new GridManager(_groundTilemap.size.x, _groundTilemap.size.y, _collisionsTilemap, _groundTilemap.origin);


        _openList = new List<PathNode>();
        _closedHashSet = new HashSet<PathNode>();


        PathNode startNode = _gridManager.GetNodeAtTilemapGridPosition(startGridPosition);
        PathNode targetNode = _gridManager.GetNodeAtTilemapGridPosition(targetGridPosition);

        Debug.Log(targetNode.IsOccupied());
        while (targetNode.IsOccupied())
        {
            targetNode = _gridManager.GetNodeAtTilemapGridPosition(targetNode.GetGridPosition() + Vector2Int.down * 1);
        }
        targetNode.SetOccupied(true);

        PathNode endPathNode = FindShortestPath(startNode, targetNode, _gridManager, _openList, _closedHashSet, _groundTilemap);

        if (endPathNode != null)
        {
            //this will convert grid positions in the found path to world positions, so we get back from it steps in world positions
            return CreatePathStack(endPathNode, _groundTilemap);
        }
        //if we didn't find a path
        return null;
    }

    private Stack<Vector3> CreatePathStack(PathNode targetNode, Tilemap tilemap)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        PathNode nextNode = targetNode;

        //Get mid point of square
        Vector3 cellMidSquare = tilemap.cellSize * .5f;
        cellMidSquare.z = 0;

        while (nextNode != null)
        {
            //Convert grid position to world position
            Grid grid = tilemap.GetComponentInParent<Grid>();         

            Vector3 worldPosition = tilemap.GetCellCenterWorld(new Vector3Int(nextNode.GetGridPosition().x, nextNode.GetGridPosition().y, 0));
            movementPathStack.Push(worldPosition);

            nextNode = nextNode.GetParentNode();
        }

        return movementPathStack;
    }

    private PathNode FindShortestPath(PathNode startNode, PathNode targetNode, GridManager gridManager, List<PathNode> openList, HashSet<PathNode> closedHashSet, Tilemap tilemap)
    {
        _openList.Add(startNode);

        while (_openList.Count > 0)
        {
            openList.Sort();
            PathNode currentNode = _openList[0];
            openList.RemoveAt(0);
            if (currentNode == targetNode)
            {
                return currentNode;
            }

            //If we haven't found the path
            closedHashSet.Add(currentNode);

            //evaluate FCost for each neighbour of the curernt node
            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridManager, _openList, _closedHashSet, _groundTilemap);
        }

        //If we run out of open nodes and haven't found target, it means haven't found a path
        return null;
    }



    private void EvaluateCurrentNodeNeighbours(PathNode currentNode, PathNode targetNode, GridManager gridManager, List<PathNode> openList, HashSet<PathNode> closedHashSet, Tilemap tilemap)
    {
        Vector2Int currentNodeGridPosition = currentNode.GetGridPosition();
        PathNode validNeighbourNode;
        //Loop through all directions
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridManager, closedHashSet, tilemap);

                if (validNeighbourNode != null)
                {
                    int newCostToNeighbour;

                    newCostToNeighbour = currentNode.GetGCost() + GetDistance(currentNode, validNeighbourNode);

                    bool isValidNeighbourNodeInOpenList = _openList.Contains(validNeighbourNode);

                    if (newCostToNeighbour < validNeighbourNode.GetGCost() || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.SetNewGCost(newCostToNeighbour);
                        validNeighbourNode.SetHCost(GetDistance(validNeighbourNode, targetNode));
                        validNeighbourNode.SetParentNode(currentNode);

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    public int GetDistance(PathNode node1, PathNode node2)
    {
        int distanceX = Mathf.Abs(node1.GetGridPosition().x - node2.GetGridPosition().x);
        int distanceY = Mathf.Abs(node1.GetGridPosition().y - node2.GetGridPosition().y);

        if (distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }

    //Will return null if neighbour is not valid
    private PathNode GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridManager gridConstructor, HashSet<PathNode> closedHashSet, Tilemap tilemap)
    {
        if (neighbourNodeXPosition >= tilemap.size.x || neighbourNodeXPosition < tilemap.origin.x || neighbourNodeYPosition >= tilemap.size.y || neighbourNodeYPosition < tilemap.origin.y)
        {
            return null;
        }
        PathNode neighbourNode = gridConstructor.GetNodeAtTilemapGridPosition(new Vector2Int(neighbourNodeXPosition, neighbourNodeYPosition));

        //Skip it, as it has already been evaluated
        if (closedHashSet.Contains(neighbourNode) || !neighbourNode.GetIsWalkable())
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }
}
