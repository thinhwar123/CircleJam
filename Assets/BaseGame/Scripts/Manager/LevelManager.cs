using System.Threading;
using Core.GamePlay;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Manager
{
    public partial class LevelManager : MonoBehaviour
    {
        [field: SerializeField] public PickupCar[] PickupCarArray { get; private set; }
        [field: SerializeField] public WaitingZone[] WaitingZoneArray { get; private set; }
        [field: SerializeField] public Transform StartPoint { get; private set; }
        [field: SerializeField] private int CarOut {get; set;}
        private void Awake()
        {
            Init();
            GamePlayGlobalEvent.CheckAllWaitingZoneTryMovePassenger += CheckAllWaitingZoneTryMovePassenger;
        }
        private void OnDestroy()
        {
            GamePlayGlobalEvent.CheckAllWaitingZoneTryMovePassenger -= CheckAllWaitingZoneTryMovePassenger;
        }

        private void CheckAllWaitingZoneTryMovePassenger(PickupCar pickupCar)
        {
            if (pickupCar.IsFull) return;
            foreach (WaitingZone waitingZone in WaitingZoneArray)
            {
                if (!pickupCar.CanPickupPassenger(waitingZone)) continue;
                GamePlayGlobalEvent.OnWaitingZoneTryMovePassenger?.Invoke(waitingZone);
                break;
            }
            
        }
        private void Init()
        {
            CarOut = 0;
            foreach (PickupCar car in PickupCarArray)
            {
                car.Init();
            }
        }
        
        public static async UniTask PassPassenger(WaitingZone waitingZone, PickupCar car, CancellationToken ct)
        {
            car.SetPickupPassenger(true);
            waitingZone.SetPassPassenger(true);
            while (!waitingZone.IsEmpty && !car.IsFull)
            {
                Passenger passenger = waitingZone.DequeuePassenger();
                car.EnqueuePassenger(passenger);
                passenger.MoveToCurrentSlot();
                await UniTask.Delay(10, cancellationToken: ct);
            }
            await UniTask.Delay(300, cancellationToken: ct);
            car.SetPickupPassenger(false);
            waitingZone.SetPassPassenger(false);
            if (waitingZone.IsEmpty)
            {
                waitingZone.Disappear();
            }
            if (car.IsFull)
            {
                car.MoveOut();
            }
        }
        

    }

#if UNITY_EDITOR
    public partial class LevelManager
    {
        [Button]
        private void EditorSetup()
        {
            Vector3 startPosition = StartPoint.position;
            Quaternion startRotation = StartPoint.rotation;
            for (int i = 0; i < PickupCarArray.Length; i++)
            {
                EditorUtility.SetDirty(PickupCarArray[i]);
                PickupCarArray[i]
                    .SetStartPosition(i, StartPoint);
            }
        }
    }
#endif
}