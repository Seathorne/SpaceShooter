using UnityEngine;

namespace MovementCore.Base
{
    /// <summary>
    /// A state representation of a character or target that may be moving or stationary.
    /// </summary>
    public class Target
    {
        /// <summary>
        /// The current position of this character, in meters.
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.zero;

        /// <summary>
        /// The current velocity of this character, in m/s.
        /// </summary>
        public Vector2 Velocity { get; set; } = Vector2.zero;

        /// <summary>
        /// The current linear acceleration of this character, in m/s/s.
        /// </summary>
        public Vector2 Linear { get; set; } = Vector2.zero;

        /// <summary>
        /// The current orientation of this character, in radians.
        /// </summary>
        public float Orientation { get; set; } = 0f;

        /// <summary>
        /// The current rotational velocity of this character, in rad/s.
        /// </summary>
        public float Rotation { get; set; } = 0f;

        /// <summary>
        /// The current angular acceleration of this character, in rad/s/s.
        /// </summary>
        public float Angular { get; set; } = 0f;

        /// <summary>
        /// Creates a new target object whose state is identical to this one.
        /// </summary>
        /// <returns>A new target object with equivalent state property values.</returns>
        public Target Clone() => new Target()
        {
            Position = Position,
            Velocity = Velocity,
            Linear = Linear,
            Orientation = Orientation,
            Rotation = Rotation,
            Angular = Angular,
        };
    }
}
