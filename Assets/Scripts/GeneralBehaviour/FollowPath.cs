using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField] private Transform[] _path;

    [Header("Following Behaviour")]
    [SerializeField] private bool _shouldStartFromBeginning;
    [SerializeField] private bool _shouldReverseAtFinish;

    [Header("Automatic follow configuration")]
    [SerializeField] private bool _isFollowingAutomatically;
    [SerializeField] private float _automaticFollowSpeed;

    [Header("For debugging only")]
    [SerializeField] private int _targetPathIndex;
    [SerializeField] private bool _isMovingForward;

    private void Start()
    {
        _isMovingForward = true;

        if (_isFollowingAutomatically)
        {
            FollowAutomatically();
        }
    }

    private void FollowAutomatically()
    {
        Follow(_automaticFollowSpeed);
    }

    public void Follow(float moveDelta)
    {
        if (_shouldStartFromBeginning)
        {
            _targetPathIndex = 0;
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
}
