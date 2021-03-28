using MovementCore.Base;
using MovementCore.Dynamic;
using MovementCore.Utility;

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Bullets")]
    [SerializeField] private Bullet BulletPrefab;

    [Header("Linear")]
    [SerializeField, Range(0f, 100f)] private float LinearThrottle;

    [SerializeField, Range(0f, 100f)] private float LinearBrake;

    [SerializeField, Range(0f, 100f)] private float MaxSpeed;

    [SerializeField] private AnimationCurve LinearThrottleCurve;

    [SerializeField] private AnimationCurve LinearBrakeCurve;

    [Header("Angular Acceleration")]
    [SerializeField, Range(0f, 360f)] private float AngularThrottle;

    [SerializeField, Range(0f, 360f)] private float MaxRotation;

    [SerializeField] private AnimationCurve AngularThrottleCurve;

    public Mover Mover { get; private set; }

    public Vector2 Position => transform.position;

    public Vector2 Facing => transform.up;

    // Start is called before the first frame update
    protected void Start()
    {
        Mover = new Mover
        {
            Behavior = SteeringBehavior.PlayerRelative,
            Position = transform.position,
            Rotation = transform.rotation.Orientation(),
            MaxSpeed = MaxSpeed,
            MaxRotation = MaxRotation,
            MaxLinear = Mathf.Max(LinearThrottle, LinearBrake),
            MaxAngular = AngularThrottle,
        };
    }

    protected void Update()
    {
        if (!GameManager.IsPaused)
        {
            if (Input.GetButtonDown(Controls.ShootButton))
            {
                Shoot();
            }
        }
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (!GameManager.IsPaused)
        {
            UpdateMove();
            UpdateRotate();
        }
    }

    private void UpdateMove()
    {
        var rb = GetComponent<Rigidbody2D>();

        float input = Input.GetAxis(Controls.VerticalAxis);
        if (input > 0f)
        {
            // Apply forward throttle
            rb.AddForce(Facing * LinearThrottle * LinearThrottleCurve.Evaluate(input));
        }
        else if (input < 0f && rb.velocity.magnitude > 0f)
        {
            // Apply brake against movement direction
            rb.AddForce(-rb.velocity.normalized * LinearBrake * LinearBrakeCurve.Evaluate(-input));
        }

        // Clamp actual velocity
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, MaxSpeed);
    }

    private void UpdateRotate()
    {
        var rb = GetComponent<Rigidbody2D>();

        float input = Input.GetAxis(Controls.HorizontalAxis);
        float inputMag = Mathf.Abs(input);
        if (inputMag > 0f)
        {
            // Apply cw/ccw throttle
            rb.AddTorque(Mathf.Sign(-input) * AngularThrottle * AngularThrottleCurve.Evaluate(inputMag));
        }

        // Clamp actual rotation
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -MaxRotation, MaxRotation);
    }

    private void Shoot()
    {
        var bullet = Instantiate(BulletPrefab);
        bullet.Shoot(transform, Facing);
    }
}
