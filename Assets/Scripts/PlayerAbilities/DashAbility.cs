using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [Header("Dash Setup")]
    [SerializeField] private float _dashSpeed;

    [SerializeField] private float _dashDistance = 5;

    [SerializeField] private float _timeBetweenDashes;
    [SerializeField] private float _timeSinceLastUsedDash;

    [SerializeField] private float _distanceBetweenTrailElements = 1;

    [SerializeField] private LayerMask _whatIsWall;

    private bool _canDash;

    [Header("Poisonous Trail Parameters")]
    [SerializeField] private Transform _poisonousTrailElement;
    [SerializeField] private Transform _poisonousTrailParent;
    [SerializeField] private Queue<Transform> _poisonousTrailElementsQueue = new Queue<Transform>();

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public bool TryPerformDash(Vector2 direction, bool isGrounded, Action finishPerformingDash)
    {
        if (_canDash)
        {
            if (direction == Vector2.zero)
            {
                direction = transform.right;
            }

            if (isGrounded && direction.y < 0)
            {
                return false;
            }

            StartCoroutine(DashRoutine(direction, finishPerformingDash));
            return true;
        }
        else
        {
            Debug.Log($"Cannot perform dash yet. You need to wait {_timeBetweenDashes - _timeSinceLastUsedDash}s");
            return false;
        }
    }

    private IEnumerator DashRoutine(Vector2 direction, Action finishPerformingDash)
    {
        Vector3 startPosition = transform.position;
        Vector3 dashDelta = new Vector3(direction.x, direction.y).normalized * _dashDistance;
        Vector3 targetPosition = startPosition + dashDelta;

        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, _dashDistance, _whatIsWall);
        if (hit.collider != null)
        {
            targetPosition = hit.point;
        }

        float timeSinceStartedDash = 0f;
        float dashDuration = _dashDistance / _dashSpeed;

        while (timeSinceStartedDash < dashDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeSinceStartedDash / dashDuration);
            timeSinceStartedDash += Time.deltaTime;

            yield return null;
        }

        float totalDistance = (targetPosition - startPosition).magnitude;
        int numberOfSteps = Mathf.CeilToInt(totalDistance / _distanceBetweenTrailElements);

        for (int i = 1; i < numberOfSteps; i++)
        {
            float t = (float)i / numberOfSteps;
            Vector3 normalizedPosition = Vector3.Lerp(startPosition, targetPosition, t);
            Transform poisonousTrailElementInstance = Instantiate(_poisonousTrailElement, normalizedPosition, Quaternion.identity, _poisonousTrailParent);
            _poisonousTrailElementsQueue.Enqueue(poisonousTrailElementInstance);
        }

        while (_poisonousTrailElementsQueue.Count > numberOfSteps - 1)
        {
            Transform poisonousTrailInstanceToDelete = _poisonousTrailElementsQueue.Dequeue();
            if (poisonousTrailInstanceToDelete != null )
            {
                GameObject.Destroy(poisonousTrailInstanceToDelete.gameObject);
            }
            yield return null;
        }

        _rigidbody.MovePosition(targetPosition);
        _timeSinceLastUsedDash = 0;
        finishPerformingDash();
    }

    private void Update()
    {
        UpdateCanDash();
    }

    private void UpdateCanDash()
    {
        _canDash = (_timeSinceLastUsedDash > _timeBetweenDashes);
        _timeSinceLastUsedDash += Time.deltaTime;
    }
}
