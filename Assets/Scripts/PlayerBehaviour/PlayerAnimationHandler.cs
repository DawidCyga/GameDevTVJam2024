using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private readonly int _runningAnimHash = Animator.StringToHash("isMoving");
    private readonly int _jumpingAnimHash = Animator.StringToHash("isJumping");
    private readonly int _isDeadAnimHash = Animator.StringToHash("IsDead");

    private Player _player;
    private Animator _animator;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PlayerHitBox.Instance.OnPlayerDying += PlayerHitBox_OnPlayerDying;
    }

    private void PlayerHitBox_OnPlayerDying(object sender, EventArgs e)
    {
        _animator.SetBool(_isDeadAnimHash, true);
    }

    public void TriggerDeath()
    {
        PlayerHitBox.Instance.Die();
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
