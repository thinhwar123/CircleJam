using UnityEngine;

namespace TW.CustomCollider
{
    public class RingCollider : MonoBehaviour
    {
        [field: SerializeField] public int MaxLayer { get; private set; }
        [field: SerializeField] private float MinDistanceFromCenter { get; set; }
        [field: SerializeField] public SectorCollider[] SectorColliders { get; private set; }
        public SectorCollider[,] SectorInLayer { get; private set; }
        public int[] SectorInLayerCount { get; private set; }
        private readonly Arc tempArc = new Arc();

        private void Awake()
        {
            foreach (SectorCollider sector in SectorColliders)
            {
                sector.FixedPosition();
                sector.WithRingCollider(this);
            }

            SectorInLayer = new SectorCollider[MaxLayer, SectorColliders.Length];
            SectorInLayerCount = new int[MaxLayer];
            foreach (SectorCollider sector in SectorColliders)
            {
                int layer = sector.Layer;
                SectorInLayer[layer, SectorInLayerCount[layer]] = sector;
                SectorInLayerCount[layer]++;
            }
        }

        public bool TryGetSectorCollider(Vector3 position, out SectorCollider sector)
        {
            sector = null;
            foreach (SectorCollider col in SectorColliders)
            {
                if (!col.IsEnabled) continue;
                if (!col.IsInSector(position)) continue;
                sector = col;
                return true;
            }

            return false;
        }

        public void TryGetArcCanMove(SectorCollider sector, ref Arc arc)
        {
            float minAngle = GetMinAngleCanMove(sector);
            float maxAngle = GetMaxAngleCanMove(sector);
            float containedAngle = sector.Angle;
            arc.SetArcWithBorder(minAngle, maxAngle, containedAngle, sector.AngleRange);
        }

        public bool CanUpperLayer(SectorCollider sector)
        {
            if (sector.Layer == MaxLayer - 1) return false;
            sector.GetOutSector(out float minAngle, out float maxAngle);
            int layer = sector.Layer + 1;
            int sectorInLayer = SectorInLayerCount[layer];
            for (int i = 0; i < sectorInLayer; i++)
            {
                SectorCollider currentSector = SectorInLayer[layer, i];
                if (!currentSector.IsEnabled) continue;
                if (currentSector == sector) continue;
                if (!AAngleExtension.IsIntersect(currentSector.StartAngle, currentSector.EndAngle, minAngle, maxAngle))
                    continue;
                return false;

            }

            return true;
        }

        public bool CanLowerLayer(SectorCollider sector)
        {
            if (sector.Layer == 0) return false;
            sector.GetInSector(out float minAngle, out float maxAngle);
            int layer = sector.Layer - 1;
            int sectorInLayer = SectorInLayerCount[layer];
            for (int i = 0; i < sectorInLayer; i++)
            {
                SectorCollider currentSector = SectorInLayer[layer, i];
                if (!currentSector.IsEnabled) continue;
                if (currentSector == sector) continue;
                if (!AAngleExtension.IsIntersect(currentSector.StartAngle, currentSector.EndAngle, minAngle, maxAngle))
                    continue;
                return false;
            }

            return true;
        }

        private float GetMinAngleCanMove(SectorCollider sector)
        {
            int layer = sector.Layer;
            int sectorInLayer = SectorInLayerCount[layer];
            float diff = 360;
            float angle = -1;
            if (sectorInLayer <= 1) return angle;
            for (int i = 0; i < sectorInLayer; i++)
            {
                SectorCollider currentSector = SectorInLayer[layer, i];
                if (!currentSector.IsEnabled) continue;
                if (currentSector == sector) continue;
                tempArc.SetArcWithBorder(sector.EndAngle, currentSector.StartAngle, sector.Angle, sector.AngleRange);
                float newDiff = tempArc.arcRange;
                if (newDiff > diff) continue;
                diff = newDiff;
                angle = currentSector.Angle;
            }
            return angle;
        }

        private float GetMaxAngleCanMove(SectorCollider sector)
        {
            int layer = sector.Layer;
            int sectorInLayer = SectorInLayerCount[layer];
            float diff = 360;
            float angle = -1;
            if (sectorInLayer <= 1) return angle;
            for (int i = 0; i < sectorInLayer; i++)
            {
                SectorCollider currentSector = SectorInLayer[layer, i];
                if (!currentSector.IsEnabled) continue;
                if (currentSector == sector) continue;
                tempArc.SetArcWithBorder(sector.StartAngle, currentSector.EndAngle, sector.Angle, sector.AngleRange);
                float newDiff = tempArc.arcRange;
                if (newDiff > diff) continue;
                diff = newDiff;
                angle = currentSector.Angle;
            }
            return angle;
        }

        public void ChangeLayer(SectorCollider sectorCollider, int lastLayer, int newLayer)
        {
            RemoveSectorInLayer(sectorCollider, lastLayer);
            AddSectorInLayer(sectorCollider, newLayer);
        }
        public void RemoveSector(SectorCollider sectorCollider)
        {
            int layer = sectorCollider.Layer;
            RemoveSectorInLayer(sectorCollider, layer);
            sectorCollider.SetEnabled(false);
        }

        private void RemoveSectorInLayer(SectorCollider sectorCollider, int layer)
        {
            int sectorInLayer = SectorInLayerCount[layer];
            SectorCollider lastSector = SectorInLayer[layer, sectorInLayer - 1];
            for (int i = 0; i < sectorInLayer; i++)
            {
                if (SectorInLayer[layer, i] != sectorCollider) continue;
                SectorInLayer[layer, i] = lastSector;
                SectorInLayerCount[layer]--;
                break;
            }
        }

        private void AddSectorInLayer(SectorCollider sectorCollider, int layer)
        {
            int sectorInLayer = SectorInLayerCount[layer];
            SectorInLayer[layer, sectorInLayer] = sectorCollider;
            SectorInLayerCount[layer]++;
        }
    }
}