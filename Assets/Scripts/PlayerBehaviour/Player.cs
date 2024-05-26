using System;
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
    [SerializeField] private float _airJumpForce;

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

    [Header("Facing Direction")]
    [SerializeField] private bool _isFacingRight = true;
    [SerializeField] private float _facingDirectionValue = 1;

    [Header("Booleans")]
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isAttemptingJump;
    [SerializeField] private bool _isCancellingJump;
    [SerializeField] private bool _hasPerformedDoubleJump;
    [SerializeField] private bool _wasInAir;
    //wallCheck bool
    [SerializeField] private bool _isDetectingWall;
    //dash
    [SerializeField] private bool _isAttemptingDash;
    [SerializeField] private bool _isPerformingDash = false;

    [SerializeField] private bool _canDoubleJump;

    [Header("Cache References")]
    private Rigidbody2D _rigidbody;
    private DashAbility _dashAbility;

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
        _dashAbility = GetComponent<DashAbility>();

        _playerState = PlayerState.Grounded;
    }

    private void Start()
    {
        PlayerInputHandler.Instance.OnRegisterMoveInputNormalized += PlayerInputHandler_OnRegisterMoveInputNormalized;
        PlayerInputHandler.Instance.OnJumpButtonPressed += PlayerInputHandler_OnJumpButtonPressed;
        PlayerInputHandler.Instance.OnJumpButtonReleased += PlayerInputHandler_OnJumpButtonReleased;
        PlayerInputHandler.Instance.OnDashButtonPressed += PlayerInputHandler_OnDashButtonPressed;
    }

    private void PlayerInputHandler_OnDashButtonPressed(object sender, EventArgs e)
    {
        _isAttemptingDash = true;
    }

    private void Update()
    {
        UpdateIsMoving();
        UpdateGrounded();
        UpdateWallCheckDetection();
        UpdateFacingDirection();

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
        //ENTER STATE
        //resetting jump state
        if (_hasPerformedDoubleJump)
        {
            _hasPerformedDoubleJump = false;
        }

        TryUseDashAbility(_isGrounded);

        //ESCAPE STATE
        if (!_isGrounded)
        {
            _wasInAir = true;
            _playerState = PlayerState.InAir;
        }
    }

    private void TryUseDashAbility(bool isGrounded)
    {
        if (_isAttemptingDash && !_isDetectingWall)
        {
            if (_dashAbility.TryPerformDash(_moveDirection, _rigidbody, isGrounded, FinishPerformingDash))
            {
                _isPerformingDash = true;
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
        //ENTER STATE
        if (!_canDoubleJump && !_hasPerformedDoubleJump)
        {
            _canDoubleJump = true;
        }

        TryUseDashAbility(_isGrounded);

        //ESCAPE STATE
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
        //INFO: Rounding so that diagonal input doesn't affect horizontal velocity. If time allows, will be refactored
        _moveVelocity = new Vector2(Mathf.RoundToInt(_moveDirection.x) * _groundMoveSpeed, 0f);
        _rigidbody.velocity = _moveVelocity;
    }

    private void DecelerateGroundedVelocity()
    {
        //INFO: Can be modified if we decide to allow for gradual deceleration instead of instant one
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
        }
    }

    private void ApplyInAirMovement()
    {
        //INFO: If you want to allow the player to decelerate the speed at which he's changing his position in the air while holding up or down button, just get rid of rounding
        float yValue = (_rigidbody.velocity.y <= 0) ? -_fallingGravityDrag : _rigidbody.velocity.y;

        _moveVelocity = new Vector2(Mathf.RoundToInt(_moveDirection.x) * _inAirHorizontalMoveSpeed, yValue);
        _rigidbody.AddForce(_moveVelocity, ForceMode2D.Force);
    }

    private void TryApplyInAirJump()
    {
        if (_isAttemptingJump && _canDoubleJump)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            Vector2 airJumpVelocity = new Vector2(_rigidbody.velocity.x, _airJumpForce);
            _rigidbody.AddForce(airJumpVelocity, ForceMode2D.Impulse);
            _canDoubleJump = false;
            _hasPerformedDoubleJump = true;
            _isAttemptingJump = false;
        }
        else
        {
            _isAttemptingJump = false;
        }
    }

    private void UpdateIsMoving()
    {
        _isMoving = (_moveDirection.x != 0) ? true : false;
    }

    private void UpdateGrounded()
    {
        _isGrounded = Physics2D.OverlapBox(_groundCheckTransform.position, _groundCheckSize, 0f, _whatIsGround);
    }

    private void UpdateWallCheckDetection()
    {
        _isDetectingWall = Physics2D.Raycast(_wallCheckTransform.position, transform.right, _wallCheckDistance, _whatIsGround);
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
        // transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        transform.Rotate(0, 180, 0);
        _isFacingRight = !_isFacingRight;
        _facingDirectionValue *= -1;
    }

    private void FinishPerformingDash()
    {
        _isPerformingDash = false;
    }

    private void PlayerInputHandler_OnRegisterMoveInputNormalized(object sender, PlayerInputHandler.OnRegisterMoveInputNormalizedEventArgs e)
    {
        //TODO: persist the same speed even if holding up arrow, while still allowing for dash direction choice
        _moveDirection = e.DirectionInput;
    }

    private void PlayerInputHandler_OnJumpButtonPressed(object sender, System.EventArgs e)
    {
        _isAttemptingJump = true;
    }

    private void PlayerInputHandler_OnJumpButtonReleased(object sender, System.EventArgs e)
    {
        _isCancellingJump = true;
    }

    public bool IsDetectingWall() => _isDetectingWall;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_groundCheckTransform.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wallCheckTransform.position, new Vector3(_wallCheckTransform.position.x + _wallCheckDistance * _facingDirectionValue, _wallCheckTransform.position.y));
    }

}
