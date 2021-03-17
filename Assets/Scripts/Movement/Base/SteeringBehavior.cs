namespace MovementCore.Base
{
    /// <summary>
    /// A code representing the steering behavior exhibited by a <see cref="Mover"/>.
    /// </summary>
    public enum SteeringBehavior
    {
        PlayerRelative = 0,

        /// <summary>
        /// The mover will reduce its velocity and rotation to zero as quickly as possible.
        /// </summary>
        Stop = 1,

        /// <summary>
        /// The mover will maintain its current velocity indefinitely.
        /// </summary>
        Continue = 2,

        /// <summary>
        /// The mover will travel toward a target at max velocity.
        /// </summary>
        Seek = 3,

        /// <summary>
        /// The mover will travel away from a target at max velocity.
        /// </summary>
        Flee = 4,

        /// <summary>
        /// The mover will exhibit <see cref="Seek"/> until entering a radius of <see cref="Mover.SlowRadius"/> away from its target.
        /// The mover will then attempt to slow its speed to zero such that it reaches a radius of <see cref="Mover.StopRadius"/> away from its target in <see cref="Mover.SlowTime"/>.
        /// The mover will then reduce its velocity to zero.
        /// </summary>
        Arrive = 5,

        /// <summary>
        /// The mover will travel toward a predicted location in front of its target, based on the target's current velocity.
        /// </summary>
        Pursue = 6,

        /// <summary>
        /// The mover will wander around at its current velocity by smoothly rotating toward a random direction.
        /// </summary>
        Wander = 7,

        /// <summary>
        /// The mover will exhibit <see cref="Seek"/> toward points along a <see cref="Path"/>.
        /// </summary>
        FollowPath = 8,

        /// <summary>
        /// The mover will attempt to create a minimum distance between it and nearby movers.
        /// </summary>
        Separate = 9,

        /// <summary>
        /// The mover will predict an inbound collision with a nearby mover and exhibit <see cref="Flee"/> from the predicted colliding point.
        /// </summary>
        AvoidCollision = 10,
    }
}
