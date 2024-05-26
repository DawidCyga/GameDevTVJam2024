using System;
using System.Collections;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    [SerializeField] private float _dashSpeed;

    [SerializeField] private float _timeSinceStartedDash;
    [SerializeField] private float _dashDuration;

    [SerializeField] private float _timeSinceLastUsedDash;
    [SerializeField] private float _timeBetweenDashes;
    private bool _canDash;

    [Header("Cache References")]
    private Player _player;

    private void Awake()
    {
        _timeSinceLastUsedDash = 0;
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        UpdateCanDash();
    }

    private void UpdateCanDash()
    {
        _canDash = (_timeSinceLastUsedDash > _timeBetweenDashes) ? true : false;
        _timeSinceLastUsedDash += Time.deltaTime;
    }

    public bool TryPerformDash(Vector2 direction, Rigidbody2D rigidbody, bool isGrounded, Action finishPerformingDash)
    {
        if (_canDash)
        {
            if (direction == Vector2.zero)
            {
                direction = transform.right;
                Debug.Log("Changed direction from 0 to right");
            }

            if (isGrounded)
            {
                if (direction.y < 0)
                {
                    return false;
                }
            }

            Debug.Log(direction);

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
        Vector2 dashVelocity = _dashSpeed * direction;

        while (_timeSinceStartedDash < _dashDuration)
        {
            _timeSinceStartedDash += Time.deltaTime;
            rigidbody.velocity = dashVelocity;

            if (_player.IsDetectingWall())
            {
                Debug.Log("Broken on wall detection");
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        rigidbody.velocity = Vector2.zero;
        _timeSinceStartedDash = 0;
        _timeSinceLastUsedDash = 0;
        Debug.Log("Finished dash");
        finishPerformingDash();
    }
}
