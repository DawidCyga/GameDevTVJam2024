using System;
using UnityEditor.iOS.Xcode;
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
    [SerializeField] private bool _isCancellingJump;
    [SerializeField] private bool _wasInAir;
    //wallCheck bool
    [SerializeField] private bool _isDetectingWall;
    //Dropping Box ability
    [SerializeField] private bool _isAttemptingDropBox;
    [SerializeField] private bool _hasDroppedBox;
    [Space]
    //dash
    [SerializeField] private bool _isAttemptingDash;
    [SerializeField] private bool _isPerformingDash = false;
    [SerializeField] private bool _hasPerformedDash;


    [Header("Cache References")]
    private Rigidbody2D _rigidbody;
    private DropBoxAbility _dropBoxAbility;
    private DashAbility _dashAbilityVer2;

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
        _dropBoxAbility = GetComponent<DropBoxAbility>();
        _dashAbilityVer2 = GetComponent<DashAbility>();

        _playerState = PlayerState.Grounded;
    }

    private void Start()
    {
        PlayerInputHandler.Instance.OnRegisterMoveInputNormalized += PlayerInputHandler_OnRegisterMoveInputNormalized;
        PlayerInputHandler.Instance.OnJumpButtonPressed += PlayerInputHandler_OnJumpButtonPressed;
        PlayerInputHandler.Instance.OnJumpButtonReleased += PlayerInputHandler_OnJumpButtonReleased;
        PlayerInputHandler.Instance.OnDropBoxButtonPressed += PlayerInputHandler_OnDropBoxButtonPressed;
        PlayerInputHandler.Instance.OnDropBoxButtonReleased += PlayerInputHander_OnDropBoxButtonReleased;
        PlayerInputHandler.Instance.OnDashButtonPressed += PlayerInputHandler_OnDashButtonPressed;
    }

    private void PlayerInputHandler_OnRegisterMoveInputNormalized(object sender, PlayerInputHandler.OnRegisterMoveInputNormalizedEventArgs e)
    {
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

    private void PlayerInputHandler_OnDropBoxButtonPressed(object sender, EventArgs e)
    {
        Debug.Log("Is attempting Drop Box");
        _isAttemptingDropBox = true;
    }

    private void PlayerInputHander_OnDropBoxButtonReleased(object sender, EventArgs e)
    {
        Debug.Log("No longer attempting Drop Box");
        _isAttemptingDropBox = false;
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

    private void OnDestroy()
    {
        PlayerInputHandler.Instance.OnRegisterMoveInputNormalized -= PlayerInputHandler_OnRegisterMoveInputNormalized;
        PlayerInputHandler.Instance.OnJumpButtonPressed -= PlayerInputHandler_OnJumpButtonPressed;
        PlayerInputHandler.Instance.OnJumpButtonReleased -= PlayerInputHandler_OnJumpButtonReleased;
        PlayerInputHandler.Instance.OnDashButtonPressed -= PlayerInputHandler_OnDashButtonPressed;
    }

    private void HandleGroundState()
    {
        //resetting jump state
        if (_hasPerformedDoubleJump)
        {
            _hasPerformedDoubleJump = false;
        }

        if (_hasPerformedFirstJump && _rigidbody.velocity.y < 1)
        {
            _hasPerformedFirstJump = false;
        }

        if (_hasDroppedBox && _rigidbody.velocity.y < 1)
        {
            _hasDroppedBox = false;
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

    private void TryUseDropBoxAbility()
    {
        if (_hasDroppedBox) return;
        if (!_isAttemptingDropBox) return;
        if (_hasPerformedDoubleJump) return;
        if (!_isAttemptingJump) return;

        Debug.Log("Successfully dropped box");
        _dropBoxAbility.TryDropKillingBox();
        _hasDroppedBox = true;
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

        TryUseDropBoxAbility();
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
            _hasPerformedFirstJump = true;
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
        if (_isAttemptingJump && _canDoubleJump && _hasPerformedFirstJump)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            Vector2 airDoubleJumpVelocity = new Vector2(_rigidbody.velocity.x, _airDoubleJumpForce);
            _rigidbody.AddForce(airDoubleJumpVelocity, ForceMode2D.Impulse);
            _canDoubleJump = false;
            _hasPerformedDoubleJump = true;
            _isAttemptingJump = false;
        }
        else if (_isAttemptingJump && _canDoubleJump && !_hasPerformedFirstJump)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0f);
            Vector2 airFirstJumpVelocity = new Vector2(_rigidbody.velocity.x, _airFirstJumpForce);
            _rigidbody.AddForce(airFirstJumpVelocity, ForceMode2D.Impulse);
            _hasPerformedFirstJump = true;
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
        RaycastHit2D hit = Physics2D.Raycast(_wallCheckTransform.position, transform.right, _wallCheckDistance, _whatIsGround);
        if (hit.collider != null)
        {
            _isDetectingWall = true;
            _lastDetectedWallPosition = hit.point;
            Debug.Log(_lastDetectedWallPosition + " detected wall position");
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

    private void FinishPerformingDash()
    {
        _isPerformingDash = false;
        _hasPerformedDash = true;
    }

    public bool IsMoving() => _isMoving;
    public bool IsGrounded() => _isGrounded;

    public bool IsDetectingWall() => _isDetectingWall;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_groundCheckTransform.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_wallCheckTransform.position, new Vector3(_wallCheckTransform.position.x + _wallCheckDistance * _facingDirectionValue, _wallCheckTransform.position.y));
    }

}
