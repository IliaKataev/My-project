using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterBody _body;

    [Min(0)]
    [SerializeField] public float _speed = 5;

    [Min(0)]
    [SerializeField] public float _jumpHeight = 2.5f;

    private float _jumpSpeed;

    [Min(0)]
    [SerializeField] public float _stopJumpFactor = 2.5f;

    private float _velocity;
    private bool _isJumping;

    void Update()
    {
        _body.SetLocomotionVelocity(_velocity);

        if(_body.State == CharacterState.Grounded)
        {
            _isJumping = false;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        _velocity = value.x * _speed;
    }

    private void Awake()
    {
        _jumpSpeed = Mathf.Sqrt(2 * _jumpHeight * Physics2D.gravity.magnitude * _body.GravityFactor);
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
}
