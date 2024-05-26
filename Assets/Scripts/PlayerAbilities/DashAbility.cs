using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _timeBetweenDashes;
    [SerializeField] private float _distanceBetweenTrailElements = 1;

    private bool _canDash;
    private bool _isDashing = false;
    private float _timeSinceLastUsedDash;
    private float _timeSinceStartedDash;
    private Vector2 _dashDirection;
    private Rigidbody2D _rigidbody;
    private Action _finishPerformingDash;
    private Vector3 _lastTrailPosition;

    [Header("Cache References")]
    private Player _player;

    [Header("Poisonous Trail Parameters")]
    [SerializeField] private Transform _poisonousTrailElement;
    [SerializeField] private Transform _poisonousTrailParent;
    [SerializeField] private Queue<Transform> _poisonousTrailElementsQueue = new Queue<Transform>();

    private void Awake()
    {
        _timeSinceLastUsedDash = 0;
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        UpdateCanDash();
    }

    private void FixedUpdate()
    {
        if (_isDashing)
        {
            PerformDash();
        }
    }

    private void UpdateCanDash()
    {
        _canDash = (_timeSinceLastUsedDash > _timeBetweenDashes);
        _timeSinceLastUsedDash += Time.deltaTime;
    }

    public bool TryPerformDash(Vector2 direction, Rigidbody2D rigidbody, bool isGrounded, Action finishPerformingDash)
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

            StartCoroutine(DashRoutine(direction, rigidbody, finishPerformingDash));
            return true;
        }
        else
        {
            Debug.Log($"Cannot perform dash yet. You need to wait {_timeBetweenDashes - _timeSinceLastUsedDash}s");
            return false;
        }
    }

    private IEnumerator DashRoutine(Vector2 direction, Rigidbody2D rigidbody, Action finishPerformingDash)
    {
        _dashDirection = direction.normalized;
        _rigidbody = rigidbody;
        _finishPerformingDash = finishPerformingDash;
        _isDashing = true;
        _timeSinceStartedDash = 0;
        _lastTrailPosition = transform.position;

        yield return new WaitForSeconds(_dashDuration);

        _isDashing = false;
        _rigidbody.velocity = Vector2.zero;
        _timeSinceLastUsedDash = 0;
        Debug.Log("Finished dash");
        _finishPerformingDash();
    }

    private void PerformDash()
    {
        _timeSinceStartedDash += Time.fixedDeltaTime;
        Vector2 dashVelocity = _dashSpeed * _dashDirection;
        _rigidbody.velocity = dashVelocity;

        // Lerp the position to ensure smooth movement
        Vector3 newPosition = Vector3.Lerp(transform.position, transform.position + (Vector3)_dashDirection * _dashSpeed * Time.fixedDeltaTime, 0.5f);
        transform.position = newPosition;

        // Check if we need to instantiate a new trail element
        if ((transform.position - _lastTrailPosition).magnitude >= _distanceBetweenTrailElements)
        {
            InstantiatePoisonousTrailElement(transform.position);
            _lastTrailPosition = transform.position;
        }

        if (_player.IsDetectingWall())
        {
            Debug.Log("Broken on wall detection");
            _isDashing = false;
            _rigidbody.velocity = Vector2.zero;
        }
    }

    private void InstantiatePoisonousTrailElement(Vector3 position)
    {
        Transform poisonousTrailElementInstance = Instantiate(_poisonousTrailElement, position, Quaternion.identity, _poisonousTrailParent);
        _poisonousTrailElementsQueue.Enqueue(poisonousTrailElementInstance);
    }
}
