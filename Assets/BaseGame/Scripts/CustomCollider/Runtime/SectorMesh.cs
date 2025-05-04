using System;
using System.Runtime.CompilerServices;
using PathCreation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TW.CustomCollider
{
    public class SectorMesh : MonoBehaviour
    {
        private Transform cacheTransform;
        public Transform Transform => cacheTransform = cacheTransform != null ? cacheTransform : transform;
        
        [field: SerializeField] private SectorCollider SectorCollider { get; set; }
        [field: SerializeField] private PathCreator PathCreator { get; set; }
        [field: SerializeField] private Transform MeshTransform { get; set; }
        [field: SerializeField] private Vector3[] Points { get; set; } = new Vector3[13];
        private BezierPath BezierPath { get; set; }

        private void Awake()
        {
            BezierPath = new BezierPath(Points, false, PathSpace.xz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateMesh()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(PathCreator);
#endif
            if (PathCreator == null) return;
            if (BezierPath is null || Points.Length != 13)
            {
                Points = new Vector3[13];
                BezierPath = new BezierPath(Points, false, PathSpace.xz)
                {
                    ControlPointMode = BezierPath.ControlMode.Free
                };
            }

            for (int i = 0; i < 13; i++)
            {
                UpdatePoint(i);
                UpdatePointLeft(i);
                UpdatePointRight(i);
            }

            PathCreator.bezierPath = BezierPath;
            MeshTransform.rotation = Quaternion.LookRotation(SectorCollider.Direction);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePoint(int index)
        {
            if (index % 3 != 0) return;
            float angle = -SectorCollider.AngleRange / 2 + index * SectorCollider.AngleRange / 12;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * SectorCollider.Direction;
            Vector3 point = direction - Transform.position;
            BezierPath.SetPoint(index, point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePointLeft(int index)
        {
            if (index % 3 != 1) return;
            float angle = -SectorCollider.AngleRange / 2 + (index - 1) * SectorCollider.AngleRange / 12;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * SectorCollider.Direction;
            Vector3 point = direction - Transform.position;
            Vector3 left = Quaternion.Euler(0, 90, 0) * direction * 0.075f;
            BezierPath.SetPoint(index, left + point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdatePointRight(int index)
        {
            if (index % 3 != 2) return;
            float angle = -SectorCollider.AngleRange / 2 + (index + 1) * SectorCollider.AngleRange / 12;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * SectorCollider.Direction;
            Vector3 point = direction - Transform.position;
            Vector3 right = Quaternion.Euler(0, -90, 0) * direction * 0.075f;
            BezierPath.SetPoint(index, right + point);
        }
    }
}