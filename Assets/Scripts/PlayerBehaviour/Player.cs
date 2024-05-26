using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Ground State Parameters")]
    [SerializeField] private float _groundMoveSpeed = 5f;
    [SerializeField] private float _groundJumpForce = 5f;

    [Header("In Air State Parameters")]
    [SerializeField] private float _inAirHorizontalMoveSpeed;
    [SerializeField] private float _airJumpForce;

    [SerializeField] private float _fallingGravityDrag;

    private float _moveDirectionX;
    private Vector2 _moveVelocity;

    [Space]
    //For ground check
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Transform _groundCheckTransform;

    //bools
    [SerializeField] private bool _isMoving;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isAttemptingJump;
    [SerializeField] private bool _isCancellingJump;
    [SerializeField] private bool _hasPerformedDoubleJump;
    [SerializeField] private bool _wasInAir;

    [SerializeField] private bool _canDoubleJump;

    //cache references
    private Rigidbody2D _rigidbody;

    private enum PlayerState
    {
        Grounded,
        InAir
    }
    [SerializeField] private PlayerState _playerState;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        _playerState = PlayerState.Grounded;
    }

    private void Start()
    {
        PlayerInputHandler.Instance.OnRegisterMoveInputNormalized += PlayerInputHandler_OnRegisterMoveInputNormalized;
        PlayerInputHandler.Instance.OnJumpButtonPressed += PlayerInputHandler_OnJumpButtonPressed;
        PlayerInputHandler.Instance.OnJumpButtonReleased += PlayerInputHandler_OnJumpButtonReleased;
    }

    private void Update()
    {
        UpdateIsMoving();
        UpdateGrounded();

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
                ApplyGroundMovement();
                ApplyGroundJump();
                break;
            case PlayerState.InAir:
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


        //ESCAPE STATE
        if (!_isGrounded)
        {
            _wasInAir = true;
            _playerState = PlayerState.InAir;
        }
    }
    private void HandleInAirState()
    {
        //ENTER STATE

        if (!_canDoubleJump && !_hasPerformedDoubleJump)
        {
            _canDoubleJump = true;
        }

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
        _moveVelocity = new Vector2(Mathf.RoundToInt(_moveDirectionX) * _groundMoveSpeed, 0f);
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
        float yValue = (_rigidbody.velocity.y <= 0) ? _fallingGravityDrag : _rigidbody.velocity.y;

        _moveVelocity = new Vector2(Mathf.RoundToInt(_moveDirectionX) * _inAirHorizontalMoveSpeed, yValue);
        _rigidbody.AddForce(_moveVelocity, ForceMode2D.Force);
        Debug.Log("yValue: " + yValue);
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
        _isMoving = (_moveDirectionX != 0) ? true : false;
    }

    private void UpdateGrounded()
    {
        _isGrounded = Physics2D.OverlapBox(_groundCheckTransform.position, _groundCheckSize, 0f, _whatIsGround);
    }

    private void PlayerInputHandler_OnRegisterMoveInputNormalized(object sender, PlayerInputHandler.OnRegisterMoveInputNormalizedEventArgs e)
    {
        //TODO: persist the same speed even if holding up arrow, while still allowing for dash direction choice
        _moveDirectionX = e.DirectionInput.x;
    }

    private void PlayerInputHandler_OnJumpButtonPressed(object sender, System.EventArgs e)
    {
        _isAttemptingJump = true;
    }

    private void PlayerInputHandler_OnJumpButtonReleased(object sender, System.EventArgs e)
    {
        _isCancellingJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_groundCheckTransform.position, _groundCheckSize);
    }

}
