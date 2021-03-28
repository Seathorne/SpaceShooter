using MovementCore.Base;
using MovementCore.Dynamic;
using MovementCore.Utility;

using UnityEngine;

public class Player : MonoBehaviour
{
    public float MaxSpeed;
    public float MaxRotation;

    public float LinearThrottle;
    public float LinearBrake;
    public float LinearFriction;

    public float AngularThrottle;
    public float AngularFriction;

    public Mover Mover { get; private set; }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    protected void Start()
    {
        Mover = new Mover
        {
            Behavior = SteeringBehavior.PlayerRelative,
            MaxSpeed = MaxSpeed,
            MaxRotation = MaxRotation,
            MaxLinear = Mathf.Max(LinearThrottle, LinearBrake),
            MaxAngular = AngularThrottle,
        };
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    protected void Update()
    {
        if (GameManager.IsPaused)
        {
            return;
        }

        //UpdateMover();
        //Utility.Print($"{Mover.Position},{Mover.Velocity},{Mover.Linear}, {Mover.Orientation},{Mover.Rotation},{Mover.Angular}");
    }

    private Steering PlayerRelative()
    {
        // Shorthand for keys that are held
        bool right = VirtualKey.RotateRight.IsHeld();
        bool left = VirtualKey.RotateLeft.IsHeld();
        bool up = VirtualKey.Accelerate.IsHeld();
        bool down = VirtualKey.Brake.IsHeld();

        Steering result = Steering.Null;
        if (right ^ left)
        {
            // If right/left held, update angular
            result.Angular = left ? AngularThrottle : -AngularThrottle;
        }
        else if (Mover.Angular != 0f)
        {
            result.Angular = Mathf.Sign(-Mover.Velocity.x) * AngularFriction;
        }

        if (up ^ down)
        {
            // If up/down held, update linear
            result.Linear = Mover.Orientation.RadianToVector2() * (up ? LinearThrottle : -LinearBrake);
        }
        else if (Mover.Velocity.magnitude != 0f)
        {
            result.Linear = new Vector2
            {
                x = Mathf.Sign(-Mover.Velocity.x) * LinearFriction,
                y = Mathf.Sign(-Mover.Velocity.y) * LinearFriction,
            };
        }

        return result;
    }

    private void UpdateMover()
    {
        var steering = PlayerRelative();
        Utility.Print($"asldkjsalkd{steering.Linear} {steering.Angular}");
        Mover.Update(steering, Time.deltaTime);

        var rb = GetComponent<Rigidbody2D>();
        rb.MoveRotation(Mover.Orientation * Mathf.Rad2Deg);
        rb.MovePosition(Mover.Position);
    }
}
