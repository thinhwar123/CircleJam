using System;

namespace Core.GamePlay
{
    public class GamePlayGlobalEvent
    {
        public static Action<WaitingZone> OnWaitingZoneTryMovePassenger { get; set; }
        public static Action On1CarMoveOut { get; set; }

        public static Action<PickupCar> CheckAllWaitingZoneTryMovePassenger { get; set; }
    }
}