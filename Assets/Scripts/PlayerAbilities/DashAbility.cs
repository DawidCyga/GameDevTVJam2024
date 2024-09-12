using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DashAbility : MonoBehaviour
{
    public static DashAbility Instance { get; private set; }

    [Header("Dash Setup")]
    [SerializeField] private float _dashSpeed;

    [SerializeField] private float _dashDistance = 5;

    [SerializeField] private float _timeBetweenDashes;
    [SerializeField] private float _timeSinceLastUsedDash;

    [SerializeField] private float _distanceBetweenTrailElements = 1;

    [SerializeField] private float _timePlayerSafeFromPoison;

    [SerializeField] private LayerMask _whatIsWall;

    private bool _canDash;
    private bool _shouldUsePoison;

    [Header("Trail Parameters")]
    [SerializeField] private Transform _poisonousTrailElement;
    [SerializeField] private Transform _regularTrailElement;
    [Space]
    [SerializeField] private Transform _trailParent;
    [SerializeField] private Queue<Transform> _trailElementsQueue = new Queue<Transform>();

    public event EventHandler<OnPerformedPoisonEventArgs> OnPerformedPoisonDash;
    public class OnPerformedPoisonEventArgs
    {
        public float TimePlayerSafeFromPoison { get; set; }
    }

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
         Instance = this;

        _rigidbody = GetComponent<Rigidbody2D>();

        CheckIfShoudUsePoison();
    }

    private void CheckIfShoudUsePoison()
    {
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            _shouldUsePoison = true;
        }
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

        Vector3[] wallCheckRayOrigins = {
        startPosition,
        startPosition + new Vector3(-0.5f, 0.5f, 0),
        startPosition + new Vector3(0.5f, 0.5f, 0),
        startPosition + new Vector3(-0.5f, -0.5f, 0),
        startPosition + new Vector3(0.5f, -0.5f, 0)
        };

        float minDistance = _dashDistance;

        foreach (Vector3 origin in wallCheckRayOrigins)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, _dashDistance, _whatIsWall);
            if (hit.collider != null && hit.distance < minDistance)
            {
                minDistance = hit.distance;
            }
        }

        targetPosition = startPosition + (Vector3)(direction.normalized * minDistance);

        float timeSinceStartedDash = 0f;
        float dashDuration = minDistance / _dashSpeed;

        bool usingPoison = false;

        Transform trailElementToSpawn = _regularTrailElement;

        if (_shouldUsePoison && PoisonOrbsCollector.Instance.GetAvailableUses() > 0)
        {
            trailElementToSpawn = _poisonousTrailElement;
            usingPoison = true;

            HitsCounter.Instance.SetInvincible();

            PoisonOrbsCollector.Instance.DecreaseOrbs();

            OnPerformedPoisonDash?.Invoke(this, new OnPerformedPoisonEventArgs { TimePlayerSafeFromPoison = _timePlayerSafeFromPoison });

        }
        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            HitsCounter.Instance.SetInvincible();
        }

        while (timeSinceStartedDash < dashDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeSinceStartedDash / dashDuration);
            timeSinceStartedDash += Time.deltaTime;
            yield return null;
        }

        float totalDistance = (targetPosition - startPosition).magnitude;
        int numberOfSteps = Mathf.CeilToInt(totalDistance / _distanceBetweenTrailElements);

      
        //here change 
        for (int i = 1; i <= numberOfSteps; i++)
        {
            float t = (float)i / numberOfSteps;
            Vector3 normalizedPosition = Vector3.Lerp(startPosition, targetPosition, t);

            InstantiateTrailElement(trailElementToSpawn, normalizedPosition, usingPoison);
        }

        while (_trailElementsQueue.Count > numberOfSteps)
        {
            Transform poisonousTrailInstanceToDelete = _trailElementsQueue.Dequeue();
            if (poisonousTrailInstanceToDelete != null)
            {
                GameObject.Destroy(poisonousTrailInstanceToDelete.gameObject);
            }
            yield return null;
        }

        _rigidbody.MovePosition(targetPosition);
        _timeSinceLastUsedDash = 0;
        finishPerformingDash();
    }

    private void InstantiateTrailElement(Transform trailElement, Vector3 normalizedPosition, bool usingPoison)
    {
        Transform trailElementInstance = Instantiate(trailElement, normalizedPosition, Quaternion.identity, _trailParent);

        if (usingPoison)
        {
            PoisonousTrail poisonousTrail = trailElementInstance.GetComponent<PoisonousTrail>();
            poisonousTrail.SetTrail(_timePlayerSafeFromPoison);
        }

        _trailElementsQueue.Enqueue(trailElementInstance);
    }

    private void Update() => UpdateCanDash();

    private void UpdateCanDash()
    {
        _canDash = (_timeSinceLastUsedDash > _timeBetweenDashes);
        _timeSinceLastUsedDash += Time.deltaTime;
    }

    public float GetTimePlayerSafeFromPoison() => _timePlayerSafeFromPoison;
}
