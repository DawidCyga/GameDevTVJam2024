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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBarSlider = GetComponentInChildren<Slider>();
        _currentHealth = _health;
        _isIgnited = false;
        _prevTime = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        updateHealthBar();
        TreeState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            _isIgnited = true;
        }

        if (collision.gameObject.name == "PoisonousTrailElement" || collision.gameObject.layer == 12)
        {
            Debug.Log("extinguished");
            _isIgnited = false;
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

}
