using MovementCore.Base;
using MovementCore.Utility;

using System;

using UnityEngine;

namespace MovementCore.Dynamic
{
    /// <summary>
    /// This class contains methods for obtaining dynamic <see cref="Steering"/> outputs for a <see cref="Mover"/> character.
    /// </summary>
    public static class DynamicSteering
    {
        /// <summary>
        /// Returns the appropriate steering output based on a mover's steering behavior and current state.
        /// </summary>
        /// <param name="mover">The mover for whom the steering output is calibrated.</param>
        /// <param name="behavior">The steering output to obtain for the mover.</param>
        /// <param name="target">The target position for the steering behavior.</param>
        /// <param name="path">The target path for the steering behavior.</param>
        /// <returns>The appropriate steering output for the mover.</returns>
        public static Steering GetSteering(this Mover mover, SteeringBehavior behavior, Target target, Path path) => behavior switch
        {
            SteeringBehavior.Stop => Stop(mover),
            SteeringBehavior.Continue => throw new NotImplementedException($"{behavior} not implemented."),
            SteeringBehavior.Seek => Seek(mover, target),
            SteeringBehavior.Flee => Flee(mover, target),
            SteeringBehavior.Arrive => Arrive(mover, target),
            SteeringBehavior.Pursue => throw new NotImplementedException($"{behavior} not implemented."),
            SteeringBehavior.Wander => throw new NotImplementedException($"{behavior} not implemented."),
            SteeringBehavior.FollowPath => FollowPath(mover, path),
            SteeringBehavior.Separate => throw new NotImplementedException($"{behavior} not implemented."),
            SteeringBehavior.AvoidCollision => throw new NotImplementedException($"{behavior} not implemented."),
            _ => Steering.Null,
        };

        /// <summary>
        /// Attempts to bring a mover to a stop.
        /// </summary>
        /// <param name="mover">The mover whose velocity and rotation will be steered against.</param>
        /// <returns>The <see cref="SteeringBehavior.Stop"/> steering output for the mover.</returns>
        public static Steering Stop(Mover mover) => new Steering
        {
            // Attempt to stop moving/rotating in one timestep
            Linear = Vector2.ClampMagnitude(-mover.Velocity, mover.MaxLinear),
            Angular = Mathf.Clamp(-mover.Rotation, -mover.MaxRotation, mover.MaxRotation),
        };

        /// <summary>
        /// Attempts to steer a mover toward a target.
        /// </summary>
        /// <param name="mover">The mover whose velocity will be steered toward the target.</param>
        /// <param name="target">The position toward which the mover will be steered.</param>
        /// <returns>The <see cref="SteeringBehavior.Seek"/> steering output for the mover.</returns>
        public static Steering Seek(Mover mover, Target target) => new Steering
        {
            // Directly toward target, acceleration capped at max value
            Linear = Vector2.ClampMagnitude(target.Position - mover.Position, mover.MaxLinear),
            Angular = 0f,
        };

        /// <summary>
        /// Attempts to steer a mover away from a target.
        /// </summary>
        /// <param name="mover">The mover whose velocity will be steered away from the target.</param>
        /// <param name="target">The position away from which the mover will be steered.</param>
        /// <returns>The <see cref="SteeringBehavior.Flee"/> steering output for the mover.</returns>
        public static Steering Flee(Mover mover, Target target) => new Steering
        {
            // Directly away from target, acceleration capped at max value
            Linear = Vector2.ClampMagnitude(mover.Position - target.Position, mover.MaxLinear),
            Angular = 0f,
        };

        /// <summary>
        /// Attempts to steer a mover toward a target and then slow to a stop as it approaches nearby.
        /// </summary>
        /// <param name="mover">The mover whose velocity will be steered toward the target.</param>
        /// <param name="target">The position toward which the mover will be steered.</param>
        /// <returns>The <see cref="SteeringBehavior.Arrive"/> steering output for the mover.</returns>
        public static Steering Arrive(Mover mover, Target target)
        {
            // Calculate direction and distance to target
            var direction = target.Position - mover.Position;
            float distance = direction.magnitude;
            direction.Normalize();

            // Set desired speed to approach target
            float idealSpeed = 0f;
            if (distance > mover.SlowRadius)
            {
                // Far away: approach at full speed
                idealSpeed = mover.MaxSpeed;
            }
            else if (distance > mover.StopRadius)
            {
                // Nearing target: linearly interpolate for a controlled approach
                idealSpeed = mover.MaxSpeed * distance / mover.SlowRadius;
            }

            // Desired acceleration based on direction, speed, and time to reach target
            var desiredAccel = (direction * idealSpeed - mover.Velocity) / mover.SlowTime;

            return new Steering
            {
                // Cap acceleration at max value
                Linear = Vector2.ClampMagnitude(desiredAccel, mover.MaxLinear),
                Angular = 0f,
            };
        }

        /// <summary>
        /// Attempts to steer a mover toward the predicted trajectory of a target.
        /// </summary>
        /// <param name="mover">The mover whose velocity will be steered toward the predicted trajectory of the target.</param>
        /// <param name="target">The target whose current velocity will be used to predict its future trajectory.</param>
        /// <returns>The <see cref="SteeringBehavior.Pursue"/> steering output for the mover.</returns>
        public static Steering Pursue(Mover mover, Target target)
        {
            // Calculate direction and distance to target
            var direction = target.Position - mover.Position;
            float distance = direction.magnitude;

            // Predict target's movement, capped at maximum prediction distance
            float speed = mover.Velocity.magnitude;
            float prediction = (speed <= distance / mover.MaxPrediction)
                ? mover.MaxPrediction
                : distance / speed;

            // Use Seek behavior on predicted target position
            var seekTarget = target.Clone();
            seekTarget.Position += target.Velocity * prediction;
            return Seek(mover, target);
        }

        /// <summary>
        /// Attempts to steer a mover along a path.
        /// </summary>
        /// <param name="mover">The mover whose velocity will be steered toward a point along the path.</param>
        /// <param name="path">The target path toward along which the mover will be steered.</param>
        /// <returns>The <see cref="SteeringBehavior.FollowPath"/> steering output for the mover.</returns>
        public static Steering FollowPath(Mover mover, Path path)
        {
            // Find parameter on path closest to current location
            float currParam = path.ClosestParameter(mover.Position);

            // Use Seek behavior on target position slightly further along path
            float targetParam = Math.Min(1, currParam + mover.MaxPathStep);
            Target target = new Target()
            {
                Position = path.PositionAt(targetParam),
            };
            return Seek(mover, target);
        }
    }
}
