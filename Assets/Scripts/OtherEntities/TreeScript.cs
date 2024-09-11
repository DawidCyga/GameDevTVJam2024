using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeScript : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _burnTickRate;
    [SerializeField] private float _burnDamage;
    [SerializeField] private float _health;

    [Header("Debugging info")]
    [SerializeField] private float _currentHealth;
    [SerializeField] public bool _isIgnited;
    [SerializeField] private float _prevTime;

    private Slider healthBarSlider;
    public Image healthBarImage;

    private Animator _animator;
    private int _ignitedAnimHash = Animator.StringToHash("IsIgnited");

    private const string TRAIL_NAME_REFERENCE = "RegularTrailElement";

    public static event EventHandler OnAnyTreeIgnited;
    public static event EventHandler OnAnyTreeExtinguished;

    public static event EventHandler OnAnyTreeBurned;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        healthBarSlider = GetComponentInChildren<Slider>();
        _currentHealth = _health;
        _isIgnited = false;
        _prevTime = Time.fixedTime;
    }

    void Update()
    {
        updateHealthBar();
        TreeState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if (!_isIgnited)
            {
                _isIgnited = true;
                OnAnyTreeIgnited?.Invoke(this, EventArgs.Empty);
            }
        }

        if (collision.gameObject.name == TRAIL_NAME_REFERENCE || collision.gameObject.layer == 12)
        {
            Debug.Log("extinguished");
            if (_isIgnited)
            {
                _isIgnited = false;
                OnAnyTreeExtinguished?.Invoke(this, EventArgs.Empty);
            }
        }
        _animator.SetBool(_ignitedAnimHash, _isIgnited);
    }

    private void TreeState()
    {
        if (_isIgnited)
        {
            if (Time.fixedTime - _prevTime > _burnTickRate)
            {
                takeDamage();
                _prevTime = Time.fixedTime;
            }

        }
    }

    private void takeDamage()
    {
        _currentHealth -= _burnDamage;
        if (_currentHealth < 0)
        {
            OnAnyTreeBurned?.Invoke(this, EventArgs.Empty);
            Debug.Log("Tree burned");
        }
    }

    private void updateHealthBar()
    {

        float _healthPercentage = _currentHealth / _health;
        healthBarSlider.value = _healthPercentage;

        if (_healthPercentage < 0.6 && _healthPercentage >= 0.3)
        {
            healthBarImage.color = Color.yellow;
        }
        if (_healthPercentage < 0.3)
        {
            healthBarImage.color = Color.red;
        }
    }

    public bool IsIgnited() => _isIgnited;
}
