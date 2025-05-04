using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Core.GamePlay
{
    public class PickupCarMovingOutState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(PickupCarMovingOutState state, CancellationToken ct);
            UniTask OnUpdate(PickupCarMovingOutState state, CancellationToken ct);
            UniTask OnExit(PickupCarMovingOutState state, CancellationToken ct);
        }
        private IHandler Owner { get; set; }

        public PickupCarMovingOutState(IHandler owner)
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
    public partial class PickupCar : PickupCarMovingOutState.IHandler
    {
        private PickupCarMovingOutState PickupCarMovingOutStateCache { get; set; }
        public PickupCarMovingOutState PickupCarMovingOutState => PickupCarMovingOutStateCache ??= new PickupCarMovingOutState(this);

        public UniTask OnEnter(PickupCarMovingOutState state, CancellationToken ct)
        {
            GamePlayGlobalEvent.On1CarMoveOut?.Invoke();
            Vector3 newPosition = PickPassengerPosition.position + Vector3.left * 5;
            PickupCarMoveController.MoveToPosition(newPosition, OnMoveOutCompleteCache);
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(PickupCarMovingOutState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(PickupCarMovingOutState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        [ACacheMethod]
        private void OnMoveOutComplete()
        {
            Destroy(gameObject);
        }
    }
}