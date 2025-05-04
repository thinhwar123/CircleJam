using System;
using LitMotion;
using Sirenix.OdinInspector;
using TW.Utility.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Core.GamePlay
{
    public partial class PickupCar : ACachedMonoBehaviour, IPassengerContainer
    {
        private readonly float slotDistance = 1.5f;
        [field: OnValueChanged(nameof(OnColorChanged))]
        [field: SerializeField] public ColorType ColorType {get; private set;}
        [field: SerializeField] private PickupCarGraphic Graphic {get; set;}
        [field: SerializeField] public Transform[] PassengerSlots { get; private set; }
        [field: SerializeField] public Passenger[] Passengers { get; private set; }
        [field: SerializeField] public int PassengerCount { get; private set;}
        [field: SerializeField] private int CarIndex { get; set; }
        [field: SerializeField] private PickupCarMoveController PickupCarMoveController { get; set; }
        [field: SerializeField] private Transform PickPassengerPosition { get; set; }
        public bool IsFull => PassengerCount >= PassengerSlots.Length;
        public bool IsEmpty => PassengerCount <= 0;
        private StateMachine StateMachine { get; set; }
        private bool PickupPassenger { get; set; }

        public void SetPickupPassenger(bool pickupPassenger)
        {
            PickupPassenger = pickupPassenger;
        }
        private void Awake()
        {
            Passengers = new Passenger[PassengerSlots.Length];
        }
        private void OnDestroy()
        {
            StateMachine.Stop();
        }

        public PickupCar SetStartPosition(int index, Transform pickPassengerPosition)
        {
            CarIndex = index;
            PickPassengerPosition = pickPassengerPosition;
            Vector3 startPosition = PickPassengerPosition.position + Vector3.forward * slotDistance * CarIndex;
            Quaternion rotation = PickPassengerPosition.rotation;
            PickupCarMoveController.SetPositionAndRotation(startPosition, rotation);
            return this;
        }
        public PickupCar Init()
        {
            StateMachine = new StateMachine();
            StateMachine.RegisterState(PickupCarSleepState);
            StateMachine.Run();
            StateMachine.RequestTransition(CarIndex != 0 ? PickupCarWaitNextCarState : PickupCarWaitPassengerState);
            return this;
        }

        public Passenger DequeuePassenger()
        {
            if (IsEmpty) return null;
            PassengerCount--;
            Passenger passenger = Passengers[PassengerCount];
            passenger.Transform.SetParent(null);
            return passenger;
        }

        public void EnqueuePassenger(Passenger passenger)
        {
            if (IsFull) return;
            Passengers[PassengerCount] = passenger;
            passenger.Transform.SetParent(PassengerSlots[PassengerCount]);
            PassengerCount++;
        }

        private void OnColorChanged()
        {
            Graphic.OnColorChange(ColorType);
        }

        public void MoveOut()
        {
            StateMachine.RequestTransition(PickupCarMovingOutState);
        }
    }
}