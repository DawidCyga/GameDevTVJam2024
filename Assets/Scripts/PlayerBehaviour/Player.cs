using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Ground State Parameters")]
    [SerializeField] private float _groundMoveSpeed = 5f;
    [SerializeField] private float _groundJumpForce = 5f;

    [Header("In Air State Parameters")]
    [SerializeField] private float _inAirHorizontalMoveSpeed;
    [SerializeField] private float _airDoubleJumpForce;
    [SerializeField] private float _airFirstJumpForce;

    [SerializeField] private float _fallingGravityDrag;

    private Vector2 _moveDirection;
    private Vector2 _moveVelocity;

    [Header("GroundCheck")]
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Transform _groundCheckTransform;
    [Header("Wall Check")]
    [SerializeField] private float _wallCheckDistance;
    [SerializeField] private Transform _wallCheckTransform;
    private Vector3 _lastDetectedWallPosition;

    [Header("Facing Direction")]
    [SerializeField] private bool _isFacingRight = true;
    [SerializeField] private float _facingDirectionValue = 1;

    [Header("Double Jumping")]
    [SerializeField] private bool _hasPerformedFirstJump;
    [SerializeField] private bool _hasPerformedDoubleJump;
    [SerializeField] private bool _canDoubleJump;

    [Header("Booleans - for debugging only")]
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isAttemptingJump;
    [SerializeField] private bool _wasInAir;
    //wallCheck bool
    [SerializeField] private bool _isDetectingWall;
    //dash
    [SerializeField] private bool _isAttemptingDash;
    [SerializeField] private bool _isPerformingDash = false;
    [SerializeField] private bool _hasPerformedDash;
    //Pause
    [SerializeField] private bool _isPaused;


    [Header("Cache References")]
    private Rigidbody2D _rigidbody;
    private DashAbility _dashAbilityVer2;
    private HitsCounter _hitsCounter;
    private PlayerHitBox _playerHitBox;

    public event EventHandler OnStartedJump;
    public event EventHandler OnStartedDash;

    private enum PlayerState
    {
        Grounded,
        InAir
    }
    [SerializeField] private PlayerState _playerState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _rigidbody = GetComponent<Rigidbody2D>();
        _dashAbilityVer2 = GetComponent<DashAbility>();
        _hitsCounter = GetComponent<HitsCounter>();
        _playerHitBox = GetComponentInChildren<PlayerHitBox>();


        _playerState = PlayerState.Grounded;
    }

    private void Start()
    {
        PlayerInputHandler.Instance.OnRegisterMoveInputNormalized += PlayerInputHandler_OnRegisterMoveInputNormalized;
        PlayerInputHandler.Instance.OnJumpButtonPressed += PlayerInputHandler_OnJumpButtonPressed;
        PlayerInputHandler.Instance.OnDashButtonPressed += PlayerInputHandler_OnDashButtonPressed;
    }

    private void OnDestroy()
    {
        PlayerInputHandler.Instance.OnRegisterMoveInputNormalized -= PlayerInputHandler_OnRegisterMoveInputNormalized;
        PlayerInputHandler.Instance.OnJumpButtonPressed -= PlayerInputHandler_OnJumpButtonPressed;
        PlayerInputHandler.Instance.OnDashButtonPressed -= PlayerInputHandler_OnDashButtonPressed;
    }

    private void PlayerInputHandler_OnRegisterMoveInputNormalized(object sender, PlayerInputHandler.OnRegisterMoveInputNormalizedEventArgs e)
    {
        _moveDirection = e.DirectionInput;
    }

    private void PlayerInputHandler_OnJumpButtonPressed(object sender, System.EventArgs e)
    {
        _isAttemptingJump = true;
    }

    private void PlayerInputHandler_OnDashButtonPressed(object sender, EventArgs e)
    {
        if (!_isPerformingDash)
        {
            _isAttemptingDash = true;
        }
    }

    private void Update()
    {
        UpdateIsMoving();
        UpdateGrounded();
        UpdateWallCheckDetection();
        UpdateFacingDirection();
        UpdateWalkSFX();


        switch (_playerState)
        {
            case PlayerState.Grounded:
                HandleGroundState();
                break;
            case PlayerState.InAir:
                HandleInAirState();
                break;
        }

    }

    private void FixedUpdate()
    {
        switch (_playerState)
        {
            case PlayerState.Grounded:
                if (_isPerformingDash) { return; }

                ApplyGroundMovement();
                ApplyGroundJump();

                break;
            case PlayerState.InAir:
                if (_isPerformingDash) { return; }

                ApplyInAirMovement();
                TryApplyInAirJump();

                break;
        }
    }

    private void HandleGroundState()
    {
        if (_hasPerformedDoubleJump)
        {
            _hasPerformedDoubleJump = false;
        }

        if (_hasPerformedFirstJump && _rigidbody.velocity.y < 1)
        {
            _hasPerformedFirstJump = false;
        }

        if (_hasPerformedDash && _isGrounded)
        {
            _hasPerformedDash = false;
            if (_isAttemptingDash)
            {
                _isAttemptingDash = false;
            }
        }

        TryUseDashAbility(_isGrounded);

        if (!_isGrounded)
        {
            _wasInAir = true;
            _playerState = PlayerState.InAir;
        }
    }

    private void TryUseDashAbility(bool isGrounded)
    {
        if (_hasPerformedDash || !_isAttemptingDash) return;

        bool canDash = !_isDetectingWall ||
                       (_moveDirection.x > 0 && _lastDetectedWallPosition.x < transform.position.x) ||
                       (_moveDirection.x < 0 && _lastDetectedWallPosition.x > transform.position.x) ||
                       (_moveDirection.y == 1);

        if (canDash)
        {
            if (_dashAbilityVer2.TryPerformDash(_moveDirection, isGrounded, FinishPerformingDash))
            {
                _isPerformingDash = true;
                OnStartedDash?.Invoke(this, EventArgs.Empty);
            }

            _isAttemptingDash = false;
        }
        else
        {
            _isAttemptingDash = false;
        }
    }

    private void HandleInAirState()
    {
        if (!_hasPerformedDoubleJump)
        {
            _canDoubleJump = true;
        }

        TryUseDashAbility(_isGrounded);

        if (_isGrounded)
        {
            _playerState = PlayerState.Grounded;
        }
    }

    private void ApplyGroundMovement()
    {
        if (_isMoving)
        {
            ApplyGroundedVelocity();
            _wasInAir = false;
        }
        else
        {
            DecelerateGroundedVelocity();
        }
    }

    private void ApplyGroundedVelocity()
    {
        _moveVelocity = new Vector2(Mathf.RoundToInt(_moveDirection.x) * _groundMoveSpeed, 0f);

        if (!_isDetectingWall)
        {
            _rigidbody.velocity = _moveVelocity;
        }
    }

    private void DecelerateGroundedVelocity()
    {
        if (_moveVelocity != Vector2.zero && !_wasInAir)
        {
            _rigidbody.velocity = new Vector2(0f, _rigidbody.velocity.y);
            _moveVelocity = Vector2.zero;
        }
    }

    private void ApplyGroundJump()
    {
        if (_isAttemptingJump)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);

            Vector2 groundJumpVelocity = new Vector2(_rigidbody.velocity.x, _groundJumpForce);
            _rigidbody.AddForce(groundJumpVelocity, ForceMode2D.Impulse);

            _isAttemptingJump = false;
            _hasPerformedFirstJump = true;

            OnStartedJump?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ApplyInAirMovement()
    {
        float yValue = (_rigidbody.velocity.y <= 0) ? -_fallingGravityDrag : _rigidbody.velocity.y;
        _moveVelocity = new Vector2(Mathf.RoundToInt(_moveDirection.x) * _inAirHorizontalMoveSpeed, yValue);
        _rigidbody.AddForce(_moveVelocity, ForceMode2D.Force);
    }

    private void TryApplyInAirJump()
    {
        if (_isAttemptingJump && _canDoubleJump && _hasPerformedFirstJump)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            Vector2 airDoubleJumpVelocity = new Vector2(_rigidbody.velocity.x, _airDoubleJumpForce);
            _rigidbody.AddForce(airDoubleJumpVelocity, ForceMode2D.Impulse);
            _canDoubleJump = false;
            _hasPerformedDoubleJump = true;
            _isAttemptingJump = false;
            OnStartedJump?.Invoke(this, EventArgs.Empty);
        }
        else if (_isAttemptingJump && _canDoubleJump && !_hasPerformedFirstJump)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            Vector2 airFirstJumpVelocity = new Vector2(_rigidbody.velocity.x, _airFirstJumpForce);
            _rigidbody.AddForce(airFirstJumpVelocity, ForceMode2D.Impulse);
            _hasPerformedFirstJump = true;
            _isAttemptingJump = false;
            OnStartedJump?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _isAttemptingJump = false;
        }
    }

    private void UpdateIsMoving()
    {
        bool hasInput = Mathf.Abs(_moveDirection.x) > 0;
        bool isActuallyMoving = hasInput && (Mathf.Abs(_rigidbody.velocity.x) > 0.1f || _isGrounded);

        _isMoving = isActuallyMoving && !_isDetectingWall;
    }

    private void UpdateGrounded()
    {
        _isGrounded = Physics2D.OverlapBox(_groundCheckTransform.position, _groundCheckSize, 0f, _whatIsGround);
    }

    private void UpdateWallCheckDetection()
    {
        RaycastHit2D hit = Physics2D.Raycast(_wallCheckTransform.position, transform.right, _wallCheckDistance, _whatIsGround);
        if (hit.collider != null)
        {
            _isDetectingWall = true;
            _lastDetectedWallPosition = hit.point;
        }
        else
        {
            _isDetectingWall = false;
        }
    }

    private void UpdateFacingDirection()
    {
        if (_isPerformingDash) { return; }
        if (_moveDirection.x > 0 && !_isFacingRight)
        {
            SwapFacingDirection();
        }
        else if (_moveDirection.x < 0 && _isFacingRight)
        {
            SwapFacingDirection();
        }
    }

    private void SwapFacingDirection()
    {
        transform.Rotate(0, 180, 0);
        _isFacingRight = !_isFacingRight;
        _facingDirectionValue *= -1;
    }

    private void UpdateWalkSFX()
    {
        if (StepsAudioManager.Instance != null)
        {
            if (TutorialGameManager.Instance != null
               && TutorialGameManager.Instance.GetCurrentState() != TutorialGameManager.GameState.Playing) return;

            if (GameStateManager.Instance != null
               && GameStateManager.Instance.GetCurrentGameState() != GameStateManager.GameState.Playing) return;

            if (_isMoving && _isGrounded)
            {
                if (!StepsAudioManager.Instance.IsPlayingWalkSound())
                    StepsAudioManager.Instance.PlayWalkSound();
            }
            else
            {
                StepsAudioManager.Instance.StopWalkSound();
            }
        }
    }

    private void FinishPerformingDash()
    {
        _isPerformingDash = false;
        _hasPerformedDash = true;
    }

    public bool IsMoving() => _isMoving;
    public bool IsGrounded() => _isGrounded;
    public bool IsDetectingWall() => _isDetectingWall;

    public void Pause()
    {
        _rigidbody.gravityScale = 0;

        _isMoving = false;
        this.enabled = false;
        _isPaused = true;
    }

    public void GoToNextScene()
    {
        _moveDirection.x = 1;

        // _isMoving = false;
        this.enabled = false;
        _isPaused = true;
    }

    public void Resume()
    {
        _rigidbody.gravityScale = 1;
        this.enabled = true;
        _isPaused = false;
    }

    public void MovePlayerAuto(float autoMovementTime)
    {
        StartCoroutine(MovePlayerAutoRoutine(autoMovementTime));
    }

    private IEnumerator MovePlayerAutoRoutine(float autoMovementTime)
    {
        _moveDirection.x = 1;
        _moveVelocity = new Vector2(Mathf.RoundToInt(_moveDirection.x) * _groundMoveSpeed, 0f);
        yield return new WaitForSeconds(autoMovementTime);
        _moveDirection.x = 0;
    }

    public bool isPaused() => _isPaused;

}
