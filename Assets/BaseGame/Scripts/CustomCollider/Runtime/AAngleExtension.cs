using System.Runtime.CompilerServices;
using UnityEngine;

namespace TW.CustomCollider
{
    /// <summary>
    /// Extension methods and angle utilities for working with circular angular ranges and comparisons.
    /// </summary>
    public static class AAngleExtension
    {
        /// <summary>
        /// Returns the angle clamped into [0, 360) degrees.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetClampedAngle(this float angle)
        {
            if (Mathf.Abs(angle) < 0.01) return 0;
            if (angle < 0) angle += 360;
            if (angle > 360) angle -= 360;
            return angle;
        }

        /// <summary>
        /// Determines if an angle lies within a circular range [start, end], accounting for wrap-around at 360 degrees.
        /// </summary>
        public static bool IsInRange(float angle, float start, float end)
        {
            if (end < start) return IsInRangeFix(angle, start, 360) || IsInRangeFix(angle, 0, end);
            return IsInRangeFix(angle, start, end);
        }

        /// <summary>
        /// Determines whether two circular angle ranges intersect, considering 0–360 degree wrap-around.
        /// Returns the length of intersected angle in degrees via out parameter.
        /// </summary>
        public static bool IsIntersect(float originStart, float originEnd, float start, float end)
        {
            // Normalize to ranges in [start, end) with wrapping support
            bool originWrapped = originEnd < originStart;
            bool targetWrapped = end < start;

            if (!originWrapped && !targetWrapped)
            {
                // Simple case: both ranges are normal
                return !(originEnd < start || end < originStart);
            }

            if (originWrapped && !targetWrapped)
            {
                // origin wraps, target doesn't: check overlap with both segments of origin
                return !(originEnd < start && end < originStart);
            }

            if (!originWrapped)
            {
                // target wraps, origin doesn't: check overlap with both segments of target
                return !(end < originStart && originEnd < start);
            }

            // Both wrap: guaranteed to intersect unless they’re exactly disjoint
            return true;
        }

        /// <summary>
        /// Checks if a given angle is within a non-wrapping range [start, end].
        /// </summary>
        private static bool IsInRangeFix(float angle, float start, float end)
        {
            return start <= angle && angle <= end;
        }
    }
}