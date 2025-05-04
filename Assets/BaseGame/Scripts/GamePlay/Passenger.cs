using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomComponent;
using UnityEngine;

namespace Core.GamePlay
{
    public partial class Passenger : ACachedMonoBehaviour
    {
        private MotionHandle MoveHandle { get; set; }
        public void MoveToCurrentSlot()
        {
            MoveHandle.TryCancel();
            MoveHandle = LMotion.Create(Transform.localPosition, Vector3.zero, 0.3f)
                .WithEase(Ease.Linear)
                .Bind(OnMoveToCurrentSlotUpdateCache);
        }
        [ACacheMethod]
        private void OnMoveToCurrentSlotUpdate(Vector3 position)
        {
            Transform.localPosition = position;
        }
    }
}