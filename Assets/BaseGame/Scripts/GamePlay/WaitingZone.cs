using LitMotion;
using Sirenix.OdinInspector;
using TW.ACacheEverything;
using TW.CustomCollider;
using TW.Utility.CustomComponent;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.GamePlay
{
    public partial class WaitingZone : ACachedMonoBehaviour, IPassengerContainer
    {
        [field: OnValueChanged(nameof(OnColorChanged))]
        [field: SerializeField] public ColorType ColorType {get; private set;}
        [field: SerializeField] public Transform MeshTransform { get; private set; }
        [field: SerializeField] public Transform[] PassengerSlots { get; private set; }
        [field: SerializeField] public Passenger[] Passengers { get; private set; }
        [field: SerializeField] public int PassengerCount { get; private set;}
        [field: SerializeField] public SectorMeshCreator SectorMeshCreator { get; private set; }
        [field: SerializeField] public SectorCollider SectorCollider { get; private set; }
        public bool IsFull => PassengerCount >= PassengerSlots.Length;
        public bool IsEmpty => PassengerCount <= 0;
        public bool PassPassenger { get; private set; }
        private Vector3 StartVector { get;set; }
        private Vector3 EndVector { get;set; }
        private MotionHandle DisappearHandle { get; set; }
        private bool IsSelected { get; set; }
        private void Awake()
        {
            WaitingZoneContainer.Register(SectorCollider, this);
            InitFullPassenger();
            SectorCollider.SetEnabled(true);
        }
        private void OnDestroy()
        {
            WaitingZoneContainer.Unregister(SectorCollider);
        }
        public void InitFullPassenger()
        {
            Passengers = new Passenger[PassengerSlots.Length];
            Passenger passengerPrefab = PrefabGlobalConfig.Instance.PassengerPrefab;
            for (int i = 0; i < PassengerCount; i++)
            {
                Passengers[i] = Instantiate(passengerPrefab, PassengerSlots[i]);
                Passengers[i].Transform.localPosition = Vector3.zero;
            }
        }
        public void SetPassPassenger(bool passPassenger)
        {
            PassPassenger = passPassenger;
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
#if UNITY_EDITOR
            EditorUtility.SetDirty(SectorMeshCreator);
#endif
            if (!ColorGlobalConfig.Instance.TryGetColorConfig(ColorType, out ColorConfig colorConfig)) return;
            SectorMeshCreator.upperMaterial = colorConfig.Material;
        }

        public void Disappear()
        {
            StartVector = Transform.position;
            EndVector = StartVector + SectorCollider.Direction * 2f;
            SectorCollider.RemoveFromRingCollider();
            DisappearHandle.TryCancel();
            DisappearHandle = LMotion.Create(0f, 1f, 0.3f)
                .WithEase(Ease.Linear)
                .WithOnComplete(OnDisappearCompleteCache)
                .Bind(OnDisappearUpdateCache);
        }
        [ACacheMethod]
        private void OnDisappearUpdate(float value)
        {
            Transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, value);
            Transform.position = Vector3.Lerp(StartVector, EndVector, value);
        }
        [ACacheMethod]
        private void OnDisappearComplete()
        {
            gameObject.SetActive(false);
        }
        public bool CanPassPassenger()
        {
            if (IsSelected) return false;
            if (IsEmpty) return false;
            if (PassPassenger) return false;
            if (!SectorCollider.IsEnabled) return false;
            if (SectorCollider.Angle is > 15f and < 345f) return false;
            return SectorCollider.IsMaxLayer;
        }

        public bool CanSelect()
        {
            return !PassPassenger;
        }
        public void SetSelected(bool selected)
        {
            IsSelected = selected;
        }
    }
}