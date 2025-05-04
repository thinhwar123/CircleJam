using System.Collections.Generic;
using TW.CustomCollider;

namespace Core.GamePlay
{
    public static class WaitingZoneContainer
    {
        private static readonly Dictionary<SectorCollider, WaitingZone> WaitingZones = new();

        public static void Register(SectorCollider sectorCollider, WaitingZone waitingZone)
        {
            WaitingZones.TryAdd(sectorCollider, waitingZone);
        }
        public static void Unregister(SectorCollider sectorCollider)
        {
            if (!WaitingZones.ContainsKey(sectorCollider)) return;
            {
                WaitingZones.Remove(sectorCollider);
            }
        }
        public static bool TryGetWaitingZone(SectorCollider sectorCollider, out WaitingZone waitingZone)
        {
            waitingZone = null;
            return WaitingZones.TryGetValue(sectorCollider, out waitingZone);
        }
        
        
    }
}