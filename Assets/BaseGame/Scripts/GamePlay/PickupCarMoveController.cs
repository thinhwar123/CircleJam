using System;
using LitMotion;
using TW.ACacheEverything;
using TW.Utility.CustomComponent;
using UnityEngine;

namespace Core.GamePlay
{
    public partial class PickupCarMoveController : ACachedMonoBehaviour
    {
        private MotionHandle MovingHandle { get; set; }
        private Vector3 StartPosition { get; set; }
        private Vector3 TargetPosition { get; set; }
        private Quaternion StartRotation { get; set; }
        private Quaternion TargetRotation { get; set; }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            Transform.SetPositionAndRotation(position, rotation);
        }
        
        public void MoveToPosition(Vector3 position, Action onMoveComplete)
        {
            MovingHandle.TryCancel();
            StartPosition = Transform.position;
            TargetPosition = position;
            StartRotation = Transform.rotation;
            TargetRotation = Quaternion.LookRotation(TargetPosition - StartPosition);
            MovingHandle = LMotion.Create(0f, 1f, 0.5f)
                .WithOnComplete(onMoveComplete)
                .WithEase(Ease.Linear)
                .Bind(OnMoveUpdateHandleCache);
        }
        [ACacheMethod]
        private void OnMoveUpdateHandle(float progress)
        {
            Vector3 position = Vector3.Lerp(StartPosition, TargetPosition, progress);
            Quaternion rotation = Quaternion.Lerp(StartRotation, TargetRotation, Mathf.Clamp01(progress*5));
            Transform.SetPositionAndRotation(position, rotation);
        }
    }
}