using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PathNode : IComparable<PathNode>
{
    private Vector2Int _gridPosition;

    private bool _isWalkable;
    private bool _isReserved;

    private int _gCost;
    private int _hCost;

    private PathNode _parentNode;

    public void Reset(bool state)
    {
        _isWalkable = state;
    }
    public PathNode(Vector2Int position, bool isWalkable)
    {
        _gridPosition = position;
        _isWalkable = isWalkable;
    }

    public int GetFCost()
    {
        return _gCost + _hCost;
    }

    public bool GetIsWalkable()
    {
        return _isWalkable;
    }

    public void SetIsWalkable(bool state)
    {
        _isWalkable = state;
    }

    public int CompareTo(PathNode other)
    {
        int result = GetFCost().CompareTo(other.GetFCost());

        if (result == 0)
        {
            result = _hCost.CompareTo(other._hCost);
        }
        return result;
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
}
