using System;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    
    [Header("Path Setup Configuration")]
    [SerializeField] private Transform[] _path;

    [Header("Following Behaviour")]
    [SerializeField] private bool _shouldStartFromBeginning;
    [SerializeField] private bool _shouldReverseAtFinish;
    [SerializeField] private bool _shouldUpdateHorizontalFacingDirection;

    [Header("Automatic follow configuration")]
    [SerializeField] private bool _isFollowingAutomatically;
    [SerializeField] private float _automaticFollowSpeed;

    [Header("For debugging only")]
    [SerializeField] private int _targetPathIndex;
    [SerializeField] private bool _isMovingForward;

    private void Start()
    {
        AssignWaypoints();

        _isMovingForward = true;

        if (_isFollowingAutomatically)
        {
            FollowAutomatically();
        }
    }

    private void AssignWaypoints()
    {
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("BabaiWaypoint");
        _path = new Transform[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            _path[i] = waypoints[i].transform;
        }
        Array.Sort(_path, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
    }

    private void FollowAutomatically() => Follow(_automaticFollowSpeed);

    public void Follow(float moveDelta)
    {
        if (_shouldStartFromBeginning)
        {
            _targetPathIndex = 0;
        }

        if (_shouldUpdateHorizontalFacingDirection)
        {
            UpdateHorizontalFacingDirection();
        }

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = _path[_targetPathIndex].position;

        if (Vector3.Distance(currentPosition, targetPosition) < .1f)
        {
            if (_isMovingForward)
            {
                _targetPathIndex++;
            }
            else
            {
                _targetPathIndex--;
            }
        }

        if (_targetPathIndex == _path.Length)
        {
            if (_shouldReverseAtFinish)
            {
                _isMovingForward = false;
                _targetPathIndex--;
            }
            else
            {
                _targetPathIndex = 0;
            }
            
        }
        else if (_targetPathIndex == 0)
        {
            _isMovingForward = true;
        }

        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, moveDelta * Time.deltaTime);
    
    }

    private void UpdateHorizontalFacingDirection()
    {
        float targetHorizontalDirection = Mathf.Sign(_path[_targetPathIndex].position.x - transform.position.x);

        if (targetHorizontalDirection < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
