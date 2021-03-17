using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace MovementCore.Utility
{
    /// <summary>
    /// Represents an ordered collection of points that may be followed by a <see cref="Mover"/> character.
    /// </summary>
    public class Path
    {
        /// <summary>
        /// The ascending collection of points making up the path.
        /// </summary>
        private readonly Vector2[] points;

        /// <summary>
        /// The proportion of distance along the path that has been traversed at each point.
        /// </summary>
        private readonly float[] parameters;

        /// <summary>
        /// The ascending collection of points making up the path.
        /// </summary>
        public IReadOnlyList<Vector2> Points => Array.AsReadOnly(points);

        /// <summary>
        /// The total distance along the path.
        /// </summary>
        public float TotalLength { get; private set; }

        /// <summary>
        /// Creates a new path made up of the provided points.
        /// </summary>
        /// <param name="points">The points making up the path, in order.</param>
        public Path(params Vector2[] points)
        {
            Debug.Assert(points?.Length >= 1);

            this.points = points;
            parameters = new float[points.Length];

            // Populate each point with its proportional distance along the path
            parameters[0] = 0;
            for (int i = 0; i < points.Length - 1; i++)
            {
                var a = points[i];
                var b = points[i + 1];
                parameters[i + 1] = parameters[i] + Vector2.Distance(a, b);
            }

            // Normalize parameters such that first=0.00 and last=1.00
            TotalLength = parameters[points.Length - 1];
            for (int i = 0; i < points.Length; i++)
            {
                parameters[i] /= TotalLength;
            }
        }

        /// <summary>
        /// Calculates the position on the path corresponding to a given path parameter.
        /// </summary>
        /// <param name="parameter">The proportion of the path length that has been traveled.</param>
        /// <returns>The position that is an equal proportion along the path to <paramref name="parameter"/>.</returns>
        public Vector2 PositionAt(float parameter)
        {
            if (parameter <= 0)
            {
                return points[0];
            }
            else if (parameter >= 1)
            {
                return points[points.Length - 1];
            }

            // Set i = segment index where distance[i] > parameter
            var i = parameters.Count(d => d <= parameter);

            var a = points[i - 1];
            var b = points[i];
            float da = parameters[i - 1];
            float db = parameters[i];

            // Add proportion along segment to first point of segment
            float dp = (parameter - da) / (db - da);
            return a + ((b - a) * dp);
        }

        /// <summary>
        /// Finds the path parameter of the point on the path closest to a given position.
        /// </summary>
        /// <param name="position">The position in question, which may be on or off the path.</param>
        /// <returns>The proportion along the path of the point on the path closest to <paramref name="position"/>.</returns>
        public float ClosestParameter(Vector2 position)
        {
            Vector2 closestPoint = Vector2.zero;
            float closestDistance = float.MaxValue;
            int closestSegment = -1;
            for (int i = 0; i < points.Length - 1; i++)
            {
                // Check closest point along each segment
                var checkPoint = position.ClosestPoint(points[i], points[i + 1]);
                float checkDistance = Vector2.Distance(position, checkPoint);
                if (checkDistance < closestDistance)
                {
                    // Record new closest point
                    closestPoint = checkPoint;
                    closestDistance = checkDistance;
                    closestSegment = i;
                }
            }

            var a = points[closestSegment];
            var b = points[closestSegment + 1];
            float da = parameters[closestSegment];
            float db = parameters[closestSegment + 1];

            // Add proportion along segment to first path parameter of segment
            float dp = (closestPoint - a).magnitude / (b - a).magnitude;
            return da + ((db - da) * dp);
        }
    }
}
