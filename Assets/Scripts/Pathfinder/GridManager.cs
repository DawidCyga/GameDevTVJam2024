using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager
{
    private int _width;
    private int _height;

    Vector3Int _gridOrigin;

    private PathNode[,] _pathNodes;
    private Dictionary<Vector2Int, PathNode> _gridNodeDictionary = new Dictionary<Vector2Int, PathNode>();

    public GridManager(int width, int height, Tilemap collisionsTilemap, Vector3Int gridOrigin)
    {
        _width = width;
        _height = height;
        _gridOrigin = gridOrigin;

        _pathNodes = new PathNode[width, height];

        for (int x = 0; x < _pathNodes.GetLength(0); x++)
        {
            for (int y = 0; y < _pathNodes.GetLength(1); y++)
            {
                Vector2Int gridPosition = new Vector2Int(x + gridOrigin.x, y + gridOrigin.y);
                bool isWalkable = collisionsTilemap.GetTile(new Vector3Int(x + gridOrigin.x, y + gridOrigin.y, 0)) == null;

                if (!_gridNodeDictionary.ContainsKey(gridPosition))
                {
                    _pathNodes[x, y] = new PathNode(gridPosition, isWalkable);
                    _gridNodeDictionary[gridPosition] = _pathNodes[x, y];
                }

            }
        }
        _gridOrigin = gridOrigin;
    }

    public void Reset(Tilemap collisionsTilemap)
    {
        for (int x = 0; x < _pathNodes.GetLength(0); x++)
        {
            for (int y = 0; y < _pathNodes.GetLength(1); y++)
            {
                bool isWalkable = collisionsTilemap.GetTile(new Vector3Int(x + _gridOrigin.x, y + _gridOrigin.y, 0)) == null;
                _pathNodes[x, y].Reset(isWalkable);
            }
        }
    }

    public PathNode GetNodeAtTilemapGridPosition(Vector2Int gridPosition)
    {
        int localX = gridPosition.x - _gridOrigin.x;
        int localY = gridPosition.y - _gridOrigin.y;

        if (localX >= 0 && localX < _width && localY >= 0 && localY < _height)
        {
            return _pathNodes[localX, localY];
        }
        return null;
    }
}
