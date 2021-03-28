using MovementCore.Base;
using MovementCore.Utility;

using UnityEngine;

namespace MovementCore.Dynamic
{
    /// <summary>
    /// A character whose state (position, velocity, orientation, rotation, and linear and angular acceleration)
    /// are updated as a function of the character's current state, intrinsic characteristics, and steering behavior.
    /// </summary>
    public class Mover : Target
    {
        /// <summary>
        /// The minimum threshold speed for all movers, in m/s.
        /// Speeds below this values are considered to be 0 in order to reduce jitter.
        /// </summary>
        public const float ThresholdSpeed = 0.01f;

        /// <summary>
        /// The minimum threshold linear acceleration magnitude for all movers, in m/s/s.
        /// Linear accelerations below this value are considered to be 0 in order to reduce jitter.
        /// </summary>
        public const float ThresholdAcceleration = 0f;

        /// <summary>
        /// The minimum threshold rotational speed for all movers, in rad/s.
        /// Rotational speeds below this value are considered to be 0 in order to reduce jitter.
        /// </summary>
        public const float ThresholdRotation = 0.01f;

        /// <summary>
        /// The minimum threshold angular acceleration magnitude for all movers, in rad/s/s.
        /// Angular accelerations below this value are considered to be 0 in order to reduce jitter.
        /// </summary>
        public const float ThresholdAngularAcceleration = 0f;
        
        /// <summary>
        /// The steering behavior that is used to update this character's acceleration based on its current state and target.
        /// </summary>
        public SteeringBehavior Behavior { get; set; }

        /// <summary>
        /// The maximum speed at which this character's position can change, in m/s.
        /// </summary>
        public float MaxSpeed { get; set; } = float.MaxValue;

        /// <summary>
        /// The maximum linear acceleration at which this character's velocity can change, in m/s/s.
        /// </summary>
        public float MaxLinear { get; set; } = float.MaxValue;

        /// <summary>
        /// The maximum rotational velocity at which this character's orientation can change, in rad/s.
        /// </summary>
        public float MaxRotation { get; set; } = float.MaxValue;

        /// <summary>
        /// The maximum angular acceleration at which this character's rotational velocity can change, in rad/s/s.
        /// </summary>
        public float MaxAngular { get; set; } = float.MaxValue;

        /// <summary>
        /// The target of this character's steering function.
        /// </summary>
        public Target Target { get; set; }

        /// <summary>
        /// The target path for this character's steering function.
        /// </summary>
        public Path Path { get; set; }

        /// <summary>
        /// Whether this character should immediately align its orientation with its current velocity.
        /// </summary>
        public bool Align { get; set; }

        /// <summary>
        /// The radius, in meters, from this character's target within which
        /// the character will begin to slow down as it nears the target.
        /// </summary>
        public float SlowRadius { get; set; } = 0f;

        /// <summary>
        /// The radius, in meters, from this character's target within which
        /// the character is considered to have reached the target.
        /// </summary>
        public float StopRadius { get; set; } = 0f;

        /// <summary>
        /// The bounding radius, in meters, extending outward from this character's center.
        /// </summary>
        public float CollideRadius { get; set; } = 0f;

        /// <summary>
        /// The radius, in meters, extending outward from this character's center
        /// which the character will try to keep clear of other characters.
        /// </summary>
        public float AvoidRadius { get; set; } = 0f;

        /// <summary>
        /// The desired time, in seconds, between slowing down at the <see cref="SlowRadius"/>
        /// and stopping at the <see cref="StopRadius"/>.
        /// </summary>
        public float SlowTime { get; set; } = 1f;

        /// <summary>
        /// The maximum distance, in meters, that a moving target's future
        /// trajectory may be predicted.
        /// </summary>
        public float MaxPrediction { get; set; } = float.MaxValue;

        /// <summary>
        /// The maximum proportion (normalized path parameter) that this
        /// character may advance its current target along a path.
        /// </summary>
        public float MaxPathStep { get; set; }

        /// <summary>
        /// Updates this character's state (linear and angular positions, velocities, and accelerations)
        /// using the provided steering output, delta time, and physics specification.
        /// </summary>
        /// <param name="acceleration">The dynamic steering output with which to steer this character.</param>
        /// <param name="dt">The delta time since the last update, in seconds.</param>
        /// <param name="precision">The precision with which to perform physics calculations.</param>
        public void Update(Steering acceleration, float dt, Precision precision = Precision.NewtonEuler1)
        {
            //acceleration.Linear = Vector2.ClampMagnitude(acceleration.Linear, MaxLinear);
            //acceleration.Angular = Mathf.Clamp(acceleration.Angular, -MaxAngular, MaxAngular);

            switch (precision)
            {
                case Precision.Precise:
                    // Highschool physics update
                    float halfDtSq = 0.5f * dt * dt;
                    Position += (Velocity * dt) + (acceleration.Linear * halfDtSq);
                    Orientation += (Rotation * dt) + (acceleration.Angular * halfDtSq);
                    break;
                case Precision.NewtonEuler1:
                    // Newton-Euler 1 approximation
                    Position += Velocity * dt;
                    Orientation += Rotation * dt;
                    break;
            }

            // Update velocity/rotation
            Velocity += acceleration.Linear * dt;
            Rotation += acceleration.Angular * dt;

            // Zero-out if below threshold, cap at max speed otherwise
            Velocity = (Velocity.magnitude < ThresholdSpeed)
                ? Vector2.zero
                : Vector2.ClampMagnitude(Velocity, MaxSpeed);

            // Zero-out if below threshold, cap at max speed otherwise
            Rotation = (Mathf.Abs(Rotation) < ThresholdRotation)
                ? 0f
                : Mathf.Clamp(Rotation, -MaxRotation, MaxRotation);

            if (Align)
            {
                // Align orientation to direction of velocity
                Orientation = Velocity.Angle();
            }

            // Update linear/angular acceleration
            Linear = acceleration.Linear;
            Angular = acceleration.Angular;
        }
    }
}
