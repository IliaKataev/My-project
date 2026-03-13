using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterBody _body;
    [SerializeField] private Animator animator;

    [Min(0)][SerializeField] private float _speed = 5f;
    [Min(0)][SerializeField] private float _jumpHeight = 2.5f;
    [Min(0)][SerializeField] private float _stopJumpFactor = 2.5f;

    private float _velocity;              // горизонтальная скорость
    private bool _isJumping;              // флаг прыжка
    private Vector3 _originalScale;       // для поворота персонажа
    private float _jumpSpeed;             // рассчитанная скорость прыжка
    private bool _wantRun;                // хотим ли двигаться вправо/влево

    private void Awake()
    {
        _originalScale = transform.localScale;
        _jumpSpeed = Mathf.Sqrt(2 * _jumpHeight * Physics2D.gravity.magnitude * _body.GravityFactor);
    }

    private void Update()
    {
        // Двигаем тело персонажа
        _body.SetLocomotionVelocity(_velocity);

        // Определяем, хотим ли бегать
        _wantRun = Mathf.Abs(_velocity) > 0.1f;

        // Обновляем параметры Animator
        animator.SetFloat("Speed", Mathf.Abs(_velocity));
        animator.SetBool("Grounded", _body.State == CharacterState.Grounded);
        animator.SetBool("wantRun", _wantRun && _body.State == CharacterState.Grounded);

        // Флаг прыжка сбрасываем при касании земли
        if (_body.State == CharacterState.Grounded)
            _isJumping = false;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        _velocity = value.x * _speed;

        // Поворачиваем персонажа
        if (value.x != 0)
        {
            var scale = _originalScale;
            scale.x = value.x > 0 ? _originalScale.x : -_originalScale.x;
            transform.localScale = scale;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();
        }
        else if (context.canceled)
        {
            StopJumping();
        }
    }

    private void Jump()
    {
        if (_body.State == CharacterState.Grounded)
        {
            _isJumping = true;
            _body.Jump(_jumpSpeed);
        }
    }

    private void StopJumping()
    {
        var velocity = _body.Velocity;

        if (_isJumping && velocity.y > 0)
        {
            _isJumping = false;
            _body.Velocity = new Vector2(velocity.x, velocity.y / _stopJumpFactor);
        }
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnStateChanged(CharacterState previous, CharacterState current)
    {
        // Лэндинг триггер
        if (current == CharacterState.Grounded && previous == CharacterState.Airborne)
        {
            animator.SetTrigger("Land");
        }
    }

    private void OnEnable()
    {
        _body.StateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        _body.StateChanged -= OnStateChanged;
    }
}