
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : Ship
    {
        [SerializeField] private AnimationCurve LinearThrottleCurve;

        [SerializeField] private AnimationCurve LinearBrakeCurve;

        [SerializeField] private AnimationCurve AngularThrottleCurve;

        [field: Header("Movement")]
        [field: SerializeField, Range(0f, 100f)] public float LinearThrottle { get; protected set; }

        [field: SerializeField, Range(0f, 100f)] public float LinearBrake { get; protected set; }

        [field: SerializeField, Range(0f, 360f)] public float AngularThrottle { get; protected set; }

        protected new void Start()
        {
            base.Start();

            var generator = GameManager.Instance.bulletGenerator;
            SetWeapon(generator.ShootBasic, generator.BasicBulletArgs);

            Died += () => GameManager.Restart();
        }

        protected new void Update()
        {
            base.Update();
            if (!GameManager.IsPaused)
            {
                if (Input.GetButton(Controls.ShootButton))
                {
                    TryShoot();
                }
            }
        }

        protected override void UpdateMove()
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

            StayInBounds();
        }

        protected override void UpdateRotate()
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
    }
}