using System;
using UnityEngine;
[Serializable]
public class PathNode : IHeapItem<PathNode>
{
    private Vector2Int _gridPosition;

    private bool _isWalkable;
    private bool _isReserved;

    private int _gCost;
    private int _hCost;
    public int FCost { get { return _gCost + _hCost; } }


    private PathNode _parentNode;

    private int _heapIndex;
    public int HeapIndex { get => _heapIndex; set => _heapIndex = value; }

    public void Reset(bool state)
    {
        _isWalkable = state;
    }
    public PathNode(Vector2Int position, bool isWalkable)
    {
        _gridPosition = position;
        _isWalkable = isWalkable;
    }

    public bool GetIsWalkable()
    {
        return _isWalkable;
    }

    public void SetIsWalkable(bool state)
    {
        _isWalkable = state;
    }


    public Vector2Int GetGridPosition()
    {
        return _gridPosition;
    }

    public int GetGCost()
    {
        return _gCost;
    }

    public void SetNewGCost(int newGCost)
    {
        _gCost = newGCost;
    }

    public void SetHCost(int value)
    {
        _hCost = value;
    }

    public int GetHCost()
    {
        return _hCost;
    }

    public PathNode GetParentNode()
    {
        return _parentNode;
    }

    public void SetParentNode(PathNode parentNode)
    {
        _parentNode = parentNode;
    }

    public bool IsOccupied()
    {
        return _isReserved;
    }

    public void SetOccupied(bool state)
    {
        _isReserved = state;
    }

    public int CompareTo(PathNode nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
        {
            compare = _hCost.CompareTo(nodeToCompare._hCost);
        }
        // negative, because in our case higher priority is lower cost, and by default integers when compared return higher priority for
        // higher numbers
        return -compare;
    }
}
