using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterBody _body;

    [Min(0)]
    [SerializeField] public float _speed = 5;

    [Min(0)]
    [SerializeField] public float _jumpHeight = 2.5f;

    [SerializeField] private Animator animator;

    private float _jumpSpeed;

    [Min(0)]
    [SerializeField] public float _stopJumpFactor = 2.5f;

    private float _velocity;
    private bool _isJumping;

    private Vector3 _originalScale;

    void Update()
    {
        _body.SetLocomotionVelocity(_velocity);

        animator.SetFloat("Speed", Mathf.Abs(_velocity));

        if(_body.State == CharacterState.Grounded)
        {
            _isJumping = false;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        _velocity = value.x * _speed;

        if(value.x != 0)
        {
            var scale = _originalScale;
            scale.x = value.x > 0 ? _originalScale.x : -_originalScale.x;
            transform.localScale = scale;
        }
    }

    private void Awake()
    {
        _jumpSpeed = Mathf.Sqrt(2 * _jumpHeight * Physics2D.gravity.magnitude * _body.GravityFactor);
        _originalScale = transform.localScale;
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
            _body.Velocity = new Vector2(
                velocity.x,
                velocity.y / _stopJumpFactor);
        }
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnStateChanged(CharacterState previous, CharacterState current)
    {
        Debug.Log("player state event" + current + " и предыдущ " + previous);

        animator.SetBool("Grounded", current == CharacterState.Grounded);

        if (current == CharacterState.Grounded)
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
