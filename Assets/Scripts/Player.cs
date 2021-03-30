
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : Ship
    {
        [Header("Presets")]
        [SerializeField] private BulletArgs basicBulletArgs;

        [SerializeField] private AnimationCurve LinearThrottleCurve;

        [SerializeField] private AnimationCurve LinearBrakeCurve;

        [SerializeField] private AnimationCurve AngularThrottleCurve;

        [field: Header("Movement")]
        [field: SerializeField, Range(0f, 100f)] public float LinearThrottle { get; protected set; }

        [field: SerializeField, Range(0f, 100f)] public float LinearBrake { get; protected set; }

        [field: SerializeField, Range(0f, 360f)] public float AngularThrottle { get; protected set; }

        protected void Start()
        {
            BulletArgs = basicBulletArgs;
            BulletFactory = FindObjectOfType<BulletGenerator>().ShootBasic;
        }

        protected void Update()
        {
            if (!GameManager.IsPaused)
            {
                if (Input.GetButtonDown(Controls.ShootButton))
                {
                    print($"Shoot :)");

                    Shoot();

                    //var args = new BulletArgs
                    //{
                    //    Source = transform,
                    //    Facing = Facing,
                    //    Damage = 1.5f,
                    //    Speed = 10f,
                    //};
                    //BulletGenerator.ShootBasic(args);

                    //var bullet = Instantiate(FindObjectOfType<BulletGenerator>().basicBullet);
                    //var bullet = FindObjectOfType<BulletGenerator>().Spawn();

                    //bullet.transform.position = transform.position;
                    //bullet.transform.rotation = transform.rotation;
                    //bullet.transform.localScale = transform.localScale;

                    //var rb = bullet.GetComponent<Rigidbody2D>();
                    //rb.velocity = Facing * 10f;
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

        private void StayInBounds()
        {
            var screenSize = GameManager.HalfScreenSize;
            (float x, float y) halfSize = (0.5f, 0.5f);

            var rb = GetComponent<Rigidbody2D>();
            var newPos = Position;
            var newVel = rb.velocity;
            var force = Vector2.zero;
            const float bounce = 50f;

            bool outside = false;

            if (Position.x + halfSize.x > screenSize.x)
            {
                newPos.x = screenSize.x - halfSize.x;
                newVel.x = 0f;
                force.x = -bounce;
                outside = true;
            }
            else if (Position.x - halfSize.x < -screenSize.x)
            {
                newPos.x = -screenSize.x + halfSize.x;
                newVel.x = 0f;
                force.x = bounce;
                outside = true;
            }
            
            if (Position.y + halfSize.y > screenSize.y)
            {
                newPos.y = screenSize.y - halfSize.y;
                newVel.y = 0f;
                force.y = -bounce;
                outside = true;
            }
            else if (Position.y - halfSize.y < -screenSize.y)
            {
                newPos.y = -screenSize.y + halfSize.y;
                newVel.y = 0f;
                force.y = bounce;
                outside = true;
            }

            if (outside == false) return;

            rb.MovePosition(newPos);
            rb.velocity = newVel;
            rb.AddForce(force);
        }
    }
}