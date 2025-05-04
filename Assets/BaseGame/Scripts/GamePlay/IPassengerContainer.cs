using UnityEngine;

namespace Core.GamePlay
{
    public interface IPassengerContainer
    {
        Transform[] PassengerSlots { get; }
        Passenger[] Passengers { get; }
        int PassengerCount { get; }
        public bool IsFull { get; }
        public bool IsEmpty { get; }
        
        Passenger DequeuePassenger();
        void EnqueuePassenger(Passenger passenger);
    }
}