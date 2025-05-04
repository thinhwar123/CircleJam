using Core.GamePlay;
using R3;
using R3.Triggers;
using TW.CustomCollider;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Manager
{
    public class InputManager : Singleton<InputManager>
    {
        [field: SerializeField] private Camera MainCamera { get; set; }
        [field: SerializeField] private RingCollider RingCollider { get; set; }
        [field: SerializeField] private SectorCollider SelectSector { get; set; }
        [field: SerializeField] private WaitingZone SelectWaitingZone { get; set; }
        private Plane DefaultPlane { get; set; } = new Plane(Vector3.up, Vector3.zero);
        private Vector3 Offset { get; set; }

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Mouse0))
                .Subscribe(OnMouseDownCallback)
                .AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(KeyCode.Mouse0))
                .Subscribe(OnMouseCallback)
                .AddTo(this);
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyUp(KeyCode.Mouse0))
                .Subscribe(OnMouseUpCallback)
                .AddTo(this);
        }

        private void OnMouseDownCallback(Unit _)
        {
            if (IsMouseOverUI()) return;
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            DefaultPlane.Raycast(ray, out float enter);
            Vector3 hitPoint = ray.GetPoint(enter);
            if (!RingCollider.TryGetSectorCollider(hitPoint, out SectorCollider sector)) return;
            if (!WaitingZoneContainer.TryGetWaitingZone(sector, out WaitingZone zone)) return;
            if (!zone.CanSelect()) return; 
            SelectSector = sector;
            SelectWaitingZone = zone;
            Offset = hitPoint - sector.Transform.position;
            SelectWaitingZone.SetSelected(true);
        }

        private void OnMouseCallback(Unit _)
        {
            if (SelectSector is null) return;
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            DefaultPlane.Raycast(ray, out float enter);
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 movePoint = hitPoint - Offset;
            SelectSector.TryMoveToPosition(movePoint);
        }

        private void OnMouseUpCallback(Unit _)
        {
            if (SelectSector is null) return;
            SelectSector.FixedPosition();
            SelectSector = null;
            SelectWaitingZone.SetSelected(false);
            GamePlayGlobalEvent.OnWaitingZoneTryMovePassenger?.Invoke(SelectWaitingZone);
            SelectWaitingZone = null;
        }

        private bool IsMouseOverUI()
        {
            return Input.touchCount > 0
                ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
                : EventSystem.current.IsPointerOverGameObject();
        }
    }
}