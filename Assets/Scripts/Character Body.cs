using UnityEngine;

public class CharacterBody : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private LayerMask _solidLayers;

    [Min(0)][SerializeField] private float _maxSpeed = 30;
    [Min(0)][SerializeField] private float _surfaceAnchor = 0.01f;
    [Range(0, 90)][SerializeField] private float _maxSlope = 45f;

    private float _sqrMaxSpeed;
    private float _minGroundVertical;

    private Rigidbody2D.SlideMovement _slideMovement;

    [SerializeField] private Vector2 _velocity;
    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value.sqrMagnitude > _sqrMaxSpeed
            ? value.normalized * _maxSpeed
            : value;
    }

    public CharacterState State { get; private set; }
    [Min(0)][field: SerializeField]public float GravityFactor { get; private set; } = 1f;

    private void Awake()
    {
        _sqrMaxSpeed = _maxSpeed * _maxSpeed;
        _minGroundVertical = Mathf.Cos(_maxSlope * Mathf.Deg2Rad);

        _slideMovement = new Rigidbody2D.SlideMovement
        {
            maxIterations = 3,
            surfaceSlideAngle = 90,
            gravitySlipAngle = 90,
            surfaceUp = Vector2.up,
            surfaceAnchor = Vector2.down * _surfaceAnchor,
            gravity = Vector2.zero,
            layerMask = _solidLayers,
            useLayerMask = true
        };
    }

    private void FixedUpdate()
    {
        Velocity += Time.fixedDeltaTime * Physics2D.gravity;

        var slideResults = _rigidbody.Slide(
            _velocity,
            Time.fixedDeltaTime,
            _slideMovement);

        if (slideResults.slideHit)
        {
            _velocity -= Vector2.Dot(_velocity, slideResults.slideHit.normal)
                         * slideResults.slideHit.normal;
        }

        if (_velocity.y <= 0 && slideResults.surfaceHit)
        {
            _velocity -= Vector2.Dot(_velocity, slideResults.surfaceHit.normal)
                         * slideResults.surfaceHit.normal;

            State = slideResults.surfaceHit.normal.y >= _minGroundVertical
                ? CharacterState.Grounded
                : CharacterState.Airborne;
        }
        else
        {
            State = CharacterState.Airborne;
        }
    }

    public void SetLocomotionVelocity(float locomotionVelocity)
    {
        Velocity = new Vector2(locomotionVelocity, _velocity.y);
    }

    public void Jump(float jumpSpeed)
    {
        Velocity = new Vector2(_velocity.x, jumpSpeed);
        State = CharacterState.Airborne;
    }
}