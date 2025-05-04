using System.Threading;
using Core.Manager;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core.GamePlay
{
    public class PickupCarWaitPassengerState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(PickupCarWaitPassengerState state, CancellationToken ct);
            UniTask OnUpdate(PickupCarWaitPassengerState state, CancellationToken ct);
            UniTask OnExit(PickupCarWaitPassengerState state, CancellationToken ct);
        }
        private IHandler Owner { get; set; }

        public PickupCarWaitPassengerState(IHandler owner)
        {
            Owner = owner;
        }

        public UniTask OnEnter(CancellationToken ct)
        {
            return Owner.OnEnter(this, ct);
        }

        public UniTask OnUpdate(CancellationToken ct)
        {
            return Owner.OnUpdate(this, ct);
        }

        public UniTask OnExit(CancellationToken ct)
        {
            return Owner.OnExit(this, ct);
        }
    }
    public partial class PickupCar : PickupCarWaitPassengerState.IHandler
    {
        private PickupCarWaitPassengerState PickupCarWaitPassengerStateCache { get; set; }
        public PickupCarWaitPassengerState PickupCarWaitPassengerState => PickupCarWaitPassengerStateCache ??= new PickupCarWaitPassengerState(this);

        public UniTask OnEnter(PickupCarWaitPassengerState state, CancellationToken ct)
        {
            GamePlayGlobalEvent.OnWaitingZoneTryMovePassenger += OnWaitingZoneTryMovePassengerHandler;
            GamePlayGlobalEvent.CheckAllWaitingZoneTryMovePassenger?.Invoke(this);
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(PickupCarWaitPassengerState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(PickupCarWaitPassengerState state, CancellationToken ct)
        {
            GamePlayGlobalEvent.OnWaitingZoneTryMovePassenger -= OnWaitingZoneTryMovePassengerHandler;
            return UniTask.CompletedTask;
        }

        private void OnWaitingZoneTryMovePassengerHandler(WaitingZone waitingZone)
        {
            if (!CanPickupPassenger(waitingZone)) return;
            LevelManager.PassPassenger(waitingZone, this, this.GetCancellationTokenOnDestroy()).Forget();
        }

        public bool CanPickupPassenger(WaitingZone waitingZone)
        {
            if (PickupPassenger) return false;
            if (waitingZone.ColorType != ColorType) return false;
            if (!waitingZone.CanPassPassenger()) return false;
            return true;
        }

    }
}