using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TW.CustomCollider
{
    [SelectionBase]
    public partial class SectorCollider : MonoBehaviour
    {
        private Transform cacheTransform;
        public Transform Transform => cacheTransform = cacheTransform != null ? cacheTransform : transform;

        [SerializeField] private SectorConfig sectorConfig;
        [SerializeField] private SectorData sectorData;
        [SerializeField] private SectorMesh sectorMesh;
        private RingCollider ringCollider;
        public Vector3 Center => sectorConfig.Center;
        public float MinDistanceFromCenter => sectorConfig.MinDistanceFromCenter;
        public float DistanceFromCenter => Vector3.Distance(Transform.position, Center);
        public float AngleRange => sectorConfig.Length * 360 / (2 * Mathf.PI * DistanceFromCenter);
        public float Depth => sectorConfig.Depth;
        public Vector3 Direction => Transform.position - Center;
        public int Layer => sectorData.Layer;
        public float Angle => sectorData.Angle;
        public float StartAngle => sectorData.StartAngle;
        public float EndAngle => sectorData.EndAngle;
        public bool IsMaxLayer => sectorData.Layer >= ringCollider.MaxLayer - 1;
        private bool CanUpper { get; set; }
        private bool CanLower { get; set; }
        private bool CanChangeLayer { get; set; }
        private float TargetDistance { get; set; }
        private float CurrentDistance { get; set; }
        public bool IsEnabled { get; private set; }
        [ShowInInspector] private Arc moveArc = new Arc();

        public void SetEnabled(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public void WithRingCollider(RingCollider col)
        {
            ringCollider = col;
        }

        [Button]
        public void FixedPosition()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
            sectorData.Angle = Vector3.SignedAngle(Vector3.forward, Direction, Vector3.up).GetClampedAngle();

            sectorData.Layer = Mathf.RoundToInt((DistanceFromCenter - MinDistanceFromCenter) / Depth);
            sectorData.StartAngle = (sectorData.Angle - AngleRange / 2).GetClampedAngle();
            sectorData.EndAngle = (sectorData.Angle + AngleRange / 2).GetClampedAngle();
            transform.position = Center + Direction.normalized * (Depth * sectorData.Layer + MinDistanceFromCenter);
            CurrentDistance = DistanceFromCenter;
            TargetDistance = DistanceFromCenter;
            sectorMesh.UpdateMesh();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInSector(Vector3 position)
        {
            Vector3 direction = position - Center;
            float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up).GetClampedAngle();
            if (!AAngleExtension.IsInRange(angle, StartAngle, EndAngle)) return false;
            float distance = direction.magnitude;
            return distance >= DistanceFromCenter - Depth / 2 && distance <= DistanceFromCenter + Depth / 2;
        }

        public void TryMoveToPosition(Vector3 movePoint)
        {
            Vector3 newDirection = movePoint - Center;
            float newDistance = newDirection.magnitude;
            UpdateNewAngle(movePoint);
            CanUpper = ringCollider.CanUpperLayer(this);
            CanLower = ringCollider.CanLowerLayer(this);
            CanChangeLayer = false;
            if (DistanceFromCenter - newDistance > Depth / 2)
            {
                CanChangeLayer = ringCollider.CanLowerLayer(this);
            }

            if (newDistance - DistanceFromCenter > Depth / 2)
            {
                CanChangeLayer = ringCollider.CanUpperLayer(this);
            }

            if (CanChangeLayer)
            {
                int newLayer = Mathf.Clamp(Mathf.RoundToInt((newDistance - MinDistanceFromCenter) / Depth), Layer - 1,
                    Layer + 1);
                ringCollider.ChangeLayer(this, Layer, newLayer);
                sectorData.Layer = newLayer;
                UpdateNewAngle(movePoint);
            }

            TargetDistance = Depth * sectorData.Layer + MinDistanceFromCenter;
            CurrentDistance = Mathf.Lerp(CurrentDistance, TargetDistance, 0.1f);
            Transform.position = Center + Quaternion.Euler(0, sectorData.Angle, 0) * Vector3.forward * CurrentDistance;
            sectorMesh.UpdateMesh();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateNewAngle(Vector3 movePoint)
        {
            Vector3 newDirection = movePoint - Center;
            float newAngle = Vector3.SignedAngle(Vector3.forward, newDirection, Vector3.up).GetClampedAngle();
            ringCollider.TryGetArcCanMove(this, ref moveArc);
            sectorData.Angle = moveArc.Clamp(newAngle);
            sectorData.StartAngle = (sectorData.Angle - AngleRange / 2).GetClampedAngle();
            sectorData.EndAngle = (sectorData.Angle + AngleRange / 2).GetClampedAngle();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetOutSector(out float minAngle, out float maxAngle)
        {
            int currentLayer = Mathf.RoundToInt((DistanceFromCenter - MinDistanceFromCenter) / Depth);
            int outLayer = currentLayer + 1;
            float layerDistance = Depth * outLayer + MinDistanceFromCenter;
            float layerAngle = sectorConfig.Length * 360 / (2 * Mathf.PI * layerDistance);
            minAngle = (Angle - layerAngle / 2).GetClampedAngle();
            maxAngle = (Angle + layerAngle / 2).GetClampedAngle();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetInSector(out float minAngle, out float maxAngle)
        {
            int currentLayer = Mathf.RoundToInt((DistanceFromCenter - MinDistanceFromCenter) / Depth);
            int outLayer = currentLayer - 1;
            float layerDistance = Depth * outLayer + MinDistanceFromCenter;
            float layerAngle = sectorConfig.Length * 360 / (2 * Mathf.PI * layerDistance);
            minAngle = (Angle - layerAngle / 2).GetClampedAngle();
            maxAngle = (Angle + layerAngle / 2).GetClampedAngle();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFromRingCollider()
        {
            ringCollider.RemoveSector(this);
        }
    }
#if UNITY_EDITOR
    public partial class SectorCollider
    {
        private void OnDrawGizmos()
        {
            return;
            DrawCurrentSector();
            DrawOutSector();
            DrawInSector();
        }

        private void DrawCurrentSector()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Center, transform.position);
            Gizmos.color = Color.blue;
            Vector3 startDir = Quaternion.Euler(0, -AngleRange / 2, 0) * Direction.normalized;
            Gizmos.DrawLine(Center + startDir * (DistanceFromCenter - Depth / 2),
                Center + startDir * (DistanceFromCenter + Depth / 2));

            Vector3 endDir = Quaternion.Euler(0, AngleRange / 2, 0) * Direction.normalized;
            Gizmos.DrawLine(Center + endDir * (DistanceFromCenter - Depth / 2),
                Center + endDir * (DistanceFromCenter + Depth / 2));
        }

        private void DrawOutSector()
        {
            int currentLayer = Mathf.RoundToInt((DistanceFromCenter - MinDistanceFromCenter) / Depth);
            int outLayer = currentLayer + 1;
            float layerDistance = Depth * outLayer + MinDistanceFromCenter;
            float layerAngle = sectorConfig.Length * 360 / (2 * Mathf.PI * layerDistance);
            ;
            Gizmos.color = Color.red;
            Vector3 startDir = Quaternion.Euler(0, -layerAngle / 2, 0) * Direction.normalized;
            Gizmos.DrawLine(Center + startDir * (layerDistance - Depth / 2),
                Center + startDir * (layerDistance + Depth / 2));

            Vector3 endDir = Quaternion.Euler(0, layerAngle / 2, 0) * Direction.normalized;
            Gizmos.DrawLine(Center + endDir * (layerDistance - Depth / 2),
                Center + endDir * (layerDistance + Depth / 2));
        }

        private void DrawInSector()
        {
            int currentLayer = Mathf.RoundToInt((DistanceFromCenter - MinDistanceFromCenter) / Depth);
            int outLayer = currentLayer - 1;
            float layerDistance = Depth * outLayer + MinDistanceFromCenter;
            float layerAngle = sectorConfig.Length * 360 / (2 * Mathf.PI * layerDistance);
            ;
            Gizmos.color = Color.red;
            Vector3 startDir = Quaternion.Euler(0, -layerAngle / 2, 0) * Direction.normalized;
            Gizmos.DrawLine(Center + startDir * (layerDistance - Depth / 2),
                Center + startDir * (layerDistance + Depth / 2));

            Vector3 endDir = Quaternion.Euler(0, layerAngle / 2, 0) * Direction.normalized;
            Gizmos.DrawLine(Center + endDir * (layerDistance - Depth / 2),
                Center + endDir * (layerDistance + Depth / 2));
        }
    }
#endif
}