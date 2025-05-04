using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Core.GamePlay
{
    public class PickupCarWaitNextCarState : IState
    {
        public interface IHandler
        {
            UniTask OnEnter(PickupCarWaitNextCarState state, CancellationToken ct);
            UniTask OnUpdate(PickupCarWaitNextCarState state, CancellationToken ct);
            UniTask OnExit(PickupCarWaitNextCarState state, CancellationToken ct);
        }
        private IHandler Owner { get; set; }

        public PickupCarWaitNextCarState(IHandler owner)
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
    public partial class PickupCar : PickupCarWaitNextCarState.IHandler
    {
        private PickupCarWaitNextCarState PickupCarWaitNextCarStateCache { get; set; }
        public PickupCarWaitNextCarState PickupCarWaitNextCarState => PickupCarWaitNextCarStateCache ??= new PickupCarWaitNextCarState(this);
        

        public UniTask OnEnter(PickupCarWaitNextCarState state, CancellationToken ct)
        {
            GamePlayGlobalEvent.On1CarMoveOut += On1CarMoveOutHandler;
            return UniTask.CompletedTask;
        }

        public UniTask OnUpdate(PickupCarWaitNextCarState state, CancellationToken ct)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnExit(PickupCarWaitNextCarState state, CancellationToken ct)
        {
            GamePlayGlobalEvent.On1CarMoveOut -= On1CarMoveOutHandler;
            return UniTask.CompletedTask;
        }
        private void On1CarMoveOutHandler()
        {
            if (CarIndex == 0) return;
            CarIndex--;
            Vector3 newPosition = PickPassengerPosition.position + Vector3.forward * slotDistance * CarIndex;
            PickupCarMoveController.MoveToPosition(newPosition, OnMoveToNextSlotCompleteCache);
        }

        [ACacheMethod]
        private void OnMoveToNextSlotComplete()
        {
            if(CarIndex != 0) return;
            StateMachine.RequestTransition(PickupCarWaitPassengerState);
        }       
    }
}