using System.Runtime.CompilerServices;
using UnityEngine;

namespace TW.CustomCollider
{
    [System.Serializable]
    public class Arc
    {
        public float startAngle;
        public float endAngle;
        public float arcRange;
        public ArcType arcType;
        public float arcBorder;

        public Arc()
        {
            startAngle = 0;
            endAngle = 0;
            arcRange = 0;
            arcType = ArcType.Major;
        }

        public Arc(float startAngle, float endAngle)
        {
            this.startAngle = Mathf.Min(startAngle, endAngle);
            this.endAngle = Mathf.Max(startAngle, endAngle);
            arcType = ArcType.Minor;
            float range1 = Mathf.Abs(endAngle - startAngle);
            float range2 = 360 - range1;
            arcRange = arcType == ArcType.Minor ? Mathf.Min(range1, range2) : Mathf.Max(range1, range2);
        }

        public Arc(float start, float end, float containedAngle)
        {
            startAngle = Mathf.Min(start, end);
            endAngle = Mathf.Max(start, end);
            if (endAngle - startAngle < 180)
            {
                if (startAngle <= containedAngle && containedAngle <= endAngle)
                {
                    arcType = ArcType.Minor;
                }
                else
                {
                    arcType = ArcType.Major;
                }
            }
            else
            {
                if (startAngle <= containedAngle && containedAngle <= endAngle)
                {
                    arcType = ArcType.Major;
                }
                else
                {
                    arcType = ArcType.Minor;
                }
            }

            float range1 = Mathf.Abs(endAngle - startAngle);
            float range2 = 360 - range1;
            arcRange = arcType == ArcType.Minor ? Mathf.Min(range1, range2) : Mathf.Max(range1, range2);
        }

        public void SetArcWithBorder(float start, float end, float contained, float border)
        {
            arcBorder = border;
            startAngle = Mathf.Min(start, end);
            endAngle = Mathf.Max(start, end);


            if (endAngle - startAngle < 180)
            {
                arcType = IsInRange(contained, startAngle, endAngle, 0) ? ArcType.Minor : ArcType.Major;
            }
            else
            {
                arcType = IsInRange(contained, startAngle, endAngle, 0) ? ArcType.Major : ArcType.Minor;
            }

            float range1 = Mathf.Abs(endAngle - startAngle);
            float range2 = 360 - range1;
            arcRange = arcType == ArcType.Minor ? Mathf.Min(range1, range2) : Mathf.Max(range1, range2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float MiddleAngle()
        {
            int alpha = arcType == ArcType.Minor ? 1 : -1;
            int beta = (endAngle - startAngle) < 180 ? 1 : -1;
            return startAngle + alpha * beta * arcRange / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Clamp(float newAngle)
        {
            if (startAngle < 0 || endAngle < 0) return newAngle;
            if (arcRange < 2.1f + 2 * arcBorder) return MiddleAngle();
            return IsInArc(newAngle) ? newAngle : ClosestAngle(newAngle);
        }
        private bool IsInArc(float angle)
        {
            // refactor this !!!
            if ((endAngle - startAngle < 180 && arcType == ArcType.Major) ||
                (endAngle - startAngle > 180 && arcType == ArcType.Minor))
            {
                float newEndAngle = (endAngle + arcBorder);
                float offsetEnd = newEndAngle > 360 ? newEndAngle - 360 : 0;
                float newStartAngle = (startAngle - arcBorder);
                float offsetStart = newStartAngle < 0 ? newStartAngle : 0;
                return IsInRange(angle, offsetEnd, newStartAngle, 0) ||
                       IsInRange(angle, newEndAngle, 360 + offsetStart, 0);
            }

            return IsInRange(angle, startAngle, endAngle, arcBorder);
        }
        private float ClosestAngle(float angle)
        {
            float startAngle1 = (startAngle - arcBorder - 1).GetClampedAngle();
            float startAngle2 = (startAngle + arcBorder + 1).GetClampedAngle();

            float startAngleCheck = IsInArc(startAngle1) ? startAngle1 : startAngle2;
            float endAngle1 = (endAngle - arcBorder - 1).GetClampedAngle();
            float endAngle2 = (endAngle + arcBorder + 1).GetClampedAngle();
            float endAngleCheck = IsInArc(endAngle1) ? endAngle1 : endAngle2;

            float distToStart = MinDistance(angle, startAngleCheck);
            float distToEnd = MinDistance(angle, endAngleCheck);
            if (IsInArc(startAngle1) && IsInArc(startAngle2))
            {
                distToStart = MinDistance(angle, startAngle1);
                distToEnd = MinDistance(angle, startAngle2);
                return distToStart < distToEnd ? startAngle1 : startAngle2;
            }

            return distToStart < distToEnd ? startAngleCheck : endAngleCheck;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float MinDistance(float from, float to)
        {
            float dist = Mathf.Abs(from - to);
            return Mathf.Min(dist, 360 - dist);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInRange(float origin, float start, float end, float border)
        {
            return start + border <= origin && origin <= end - border;
        }
    }
}