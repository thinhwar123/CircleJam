using System.Threading;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace Core.GamePlay
{
    public class PickupCarSleepState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(PickupCarSleepState state, CancellationToken ct);
            UniTask OnUpdate(PickupCarSleepState state, CancellationToken ct);
            UniTask OnExit(PickupCarSleepState state, CancellationToken ct);
        }
        private IHandler Owner { get; set; }

        public PickupCarSleepState(IHandler owner)
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
    public partial class PickupCar : PickupCarSleepState.IHandler
    {
        private PickupCarSleepState PickupCarPickupCar { get; set; }
        public PickupCarSleepState PickupCarSleepState => PickupCarPickupCar ??= new PickupCarSleepState(this);

        public UniTask OnEnter(PickupCarSleepState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(PickupCarSleepState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(PickupCarSleepState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }
    }
}