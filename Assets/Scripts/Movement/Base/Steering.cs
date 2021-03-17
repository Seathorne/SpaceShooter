using UnityEngine;

namespace MovementCore.Base
{
    /// <summary>
    /// Represents a combination of linear and angular acceleration as the output of a steering function.
    /// </summary>
    public struct Steering
    {
        public static Steering Null = new Steering()
        {
            Linear = Vector2.zero,
            Angular = 0f,
        };

        /// <summary>
        /// The linear acceleration output by the steering function.
        /// </summary>
        public Vector2 Linear;

        /// <summary>
        /// The angular acceleration output by the steering function.
        /// </summary>
        public float Angular;
    }
}
