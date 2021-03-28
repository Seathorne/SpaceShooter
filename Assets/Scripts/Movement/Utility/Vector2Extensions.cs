using System;

using UnityEngine;

namespace MovementCore.Utility
{
    /// <summary>
    /// Represents a two-dimensional vector as an ordered pair of two elements.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        /// Returns the angular orientation of the provided vector, in radians.
        /// </summary>
        /// <param name="a">The vector whose orientation to calculate.</param>
        /// <returns>The direction this vector points.</returns>
        public static float Angle(this Vector2 a) => Mathf.Atan2(a.y, a.x) * Mathf.Rad2Deg;

        public static float Orientation(this Quaternion a) => Mathf.Atan2(a.eulerAngles.y, a.eulerAngles.x);

        public static float Sign(this float a) => (a == 0f) ? 0f : (a > 0f) ? 1f : -1f;

        public static Vector2 RadianToVector2(this float rad) => new Vector2
        {
            x = Mathf.Cos(rad),
            y = Mathf.Sin(rad),
        };

        /// <summary>
        /// Returns the vector product of the provided vectors.
        /// </summary>
        /// <param name="a">The left-hand operand of the cross product operation.</param>
        /// <param name="b">The right-hand operand of the cross product operation.</param>
        /// <returns>The cross product of the provided vectors.</returns>
        public static double CrossProduct(this Vector2 a, Vector2 b) => a.magnitude * b.magnitude * Mathf.Sin(Angle(a) - Angle(b));

        /// <summary>
        /// Returns the minimum distance between a query point and a line segment.
        /// </summary>
        /// <param name="q">The query point.</param>
        /// <param name="a">The first point on the line segment.</param>
        /// <param name="b">The second point on the line segment.</param>
        /// <returns>The two-dimensional distance between the query point and the line segment.</returns>
        public static double DistanceToLine(Vector2 q, Vector2 a, Vector2 b)
        {
            double numerator = Math.Abs(((b.x - a.x) * (a.y - q.y)) - ((a.x - q.x) * (b.y - a.y)));
            double denominator = Math.Sqrt(Math.Pow(b.y - a.y, 2) + Math.Pow(b.y - a.y, 2));
            return numerator / denominator;
        }

        /// <summary>
        /// Returns the closest point to a query point that lies on the provided line segment.
        /// </summary>
        /// <param name="q">The query point.</param>
        /// <param name="a">The first point on the line segment.</param>
        /// <param name="b">The second point on the line segment.</param>
        /// <returns>The closest point to the query point that lies along the line segment.</returns>
        public static Vector2 ClosestPoint(this Vector2 q, Vector2 a, Vector2 b)
        {
            var line = b - a;
            float T = Vector2.Dot(q - a, line) / Vector2.Dot(line, line);
            
            if (T <= 0)
            {
                return a;
            }
            else if (T >= 1)
            {
                return b;
            }

            return a + (line * T);
        }

        public static float Sigmoid(this float value)
        {
            const float a = 6f;
            const float b = -18f;
            const float k = 1f;

            return k / (1f + Mathf.Exp(a + b * value));
        }
    }
}
