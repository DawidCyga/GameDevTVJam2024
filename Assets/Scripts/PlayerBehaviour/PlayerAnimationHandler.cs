using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private int _runningAnimHash = Animator.StringToHash("isMoving");
    private int _jumpingAnimHash = Animator.StringToHash("isJumping");

    private Player _player;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        _animator.SetBool(_runningAnimHash, _player.IsMoving());
        _animator.SetBool(_jumpingAnimHash, !_player.IsGrounded());
    }
}
