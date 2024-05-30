using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathfinderEnemy : Enemy
{
    [SerializeField] protected float _timeSinceLastUpdatedPath;

    [Header("Path Refresh Rate Configuration")]
    [SerializeField] protected float _timeBetweenPathUpdates;

    [SerializeField] protected Stack<Vector3> _pathToTarget;

    protected float _previousPlayerDistance;

    protected Coroutine _moveToPlayerRoutine;

    protected virtual void FindPathToPlayer()
    {
        Vector2Int startPosition = GetGridPositionFromWorldPosition(transform.position);
        Vector2Int targetPosition = GetGridPositionFromWorldPosition(_target.position);

        _pathToTarget = AStarPathfinder.Instance.BuildPath(startPosition, targetPosition);

        FollowPath();
    }
    private void FollowPath()
    {
        Debug.Log("Attempting to start following path");
        if (_moveToPlayerRoutine != null)
        {
            Debug.Log("Previous path stopped");
            StopCoroutine(_moveToPlayerRoutine);
            _moveToPlayerRoutine = null;
        }
        _moveToPlayerRoutine = StartCoroutine(FollowPathRoutine());
    }

    private IEnumerator FollowPathRoutine()
    {
        if (_pathToTarget.Count > 0)
        {
            _pathToTarget.Pop();
        }

        foreach (Vector3 step in _pathToTarget)
        {
            float travelPercentage = 0;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = step;

            while (travelPercentage < 1)
            {
                if (CanSeePlayer() && _isInAttackRange)
                {
                    Debug.Log("Yielded break");
                    yield break;
                }

                travelPercentage += _moveSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, targetPosition, travelPercentage);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    protected virtual bool NeedsPathUpdate()
    {
        float currentPlayerDistance = Vector3.Distance(transform.position, _target.position);
        bool needsUpdate = _timeSinceLastUpdatedPath > _timeBetweenPathUpdates || currentPlayerDistance > _previousPlayerDistance * 1.1f;
        _previousPlayerDistance = currentPlayerDistance;
        return needsUpdate;
    }

    private Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
        return gridPosition;
    }


}