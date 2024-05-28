using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }

    private PlayerInputActions _playerInputActions;


    public event EventHandler<OnRegisterMoveInputNormalizedEventArgs> OnRegisterMoveInputNormalized;
    public class OnRegisterMoveInputNormalizedEventArgs { public Vector2 DirectionInput { get; set; } }

    public event EventHandler OnJumpButtonPressed;
    public event EventHandler OnJumpButtonReleased;

    public event EventHandler OnDashButtonPressed;

    public event EventHandler OnPauseButtonPressed;

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

        _playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Move.performed += RegisterMoveInputNormalized;
        _playerInputActions.Player.Move.canceled += RegisterMoveInputNormalized;

        _playerInputActions.Player.Jump.performed += RegisterJumpInput;
        _playerInputActions.Player.Jump.canceled += RegisterJumpInput;

        _playerInputActions.Player.Dash.performed += RegisterDashInput;

        _playerInputActions.Player.Pause.performed += RegisterPauseInput;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Move.performed -= RegisterMoveInputNormalized;
        _playerInputActions.Player.Move.canceled -= RegisterMoveInputNormalized;

        _playerInputActions.Player.Jump.performed -= RegisterJumpInput;
        _playerInputActions.Player.Jump.canceled -= RegisterJumpInput;

        _playerInputActions.Player.Dash.performed -= RegisterDashInput;

        _playerInputActions.Player.Pause.performed -= RegisterPauseInput;

        _playerInputActions.Player.Disable();
    }

    private void RegisterMoveInputNormalized(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector2 moveDirectionInputNormalized = obj.ReadValue<Vector2>().normalized;
            OnRegisterMoveInputNormalized?.Invoke(this, new OnRegisterMoveInputNormalizedEventArgs { DirectionInput = moveDirectionInputNormalized });
    }

    private void RegisterJumpInput(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            OnJumpButtonPressed?.Invoke(this, EventArgs.Empty);
        }
        else if (obj.canceled)
        {
            OnJumpButtonReleased?.Invoke(this, EventArgs.Empty);
        }
    }

    private void RegisterDashInput(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnDashButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    private void RegisterPauseInput(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseButtonPressed?.Invoke(this, EventArgs.Empty);
    }

}
