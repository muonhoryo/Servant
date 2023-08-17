
using UnityEngine;
using MuonhoryoLibrary;
using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Servant
{
    public sealed partial class MainCameraBehaviour : MonoBehaviour, ISingltone<MainCameraBehaviour>
    {
        public static MainCameraBehaviour singltone { get; private set; }
        [SerializeField]
        private Camera CameraComp;
        MainCameraBehaviour ISingltone<MainCameraBehaviour>.Singltone 
        { get => singltone;
            set => singltone=value; }
        void ISingltone<MainCameraBehaviour>.Destroy() =>
            Destroy(this);

        private static class CameraMode
        {
            private static CameraGetDepthState CurrentDepthGetter;
            private static CameraGetPositionState CurrentTargetPositionGetter;
            private static CameraMovingBehaviourState CurrentMovingBehaviourState;
            private static CameraDepthScalingBehaviourState CurrentDepthScalingBehaviourState;

            public static CameraGetDepthState CurrentDepthGetter_ => CurrentDepthGetter;
            public static CameraGetPositionState CurrentTargetPositionGetter_ => CurrentTargetPositionGetter;
            public static CameraMovingBehaviourState CurrentMovingBehaviourState_ => CurrentMovingBehaviourState;
            public static CameraDepthScalingBehaviourState CurrentDepthScalingBehaviourState_ =>
                CurrentDepthScalingBehaviourState;

            public static event Action ChangeDepthGetterEvent = delegate { };
            public static event Action ChangeTargetPositionGetterEvent = delegate { };
            public static event Action ChangeMovingBehaviourStateEvent = delegate { };
            public static event Action ChangeDepthScalingBehaviourStateEvent = delegate { };

            private static void ChangeCameraSubState<TSubStateType>(ref TSubStateType subStateField, TSubStateType newState)
                where TSubStateType : CameraSubState
            {
                if (newState == null)
                    throw ServantException.GetArgumentNullException("newState");
                subStateField?.ExitStateAction();
                subStateField = newState;
                subStateField.EnterStateAction();
            }
            public static void ChangeDepthGetter(CameraGetDepthState newState)
            {
                ChangeCameraSubState(ref CurrentDepthGetter, newState);
                ChangeDepthGetterEvent();
            }
            public static void ChangePositionGetter(CameraGetPositionState newState)
            {
                ChangeCameraSubState(ref CurrentTargetPositionGetter, newState);
                ChangeTargetPositionGetterEvent();
            }
            public static void ChangeMovingBehaviour(CameraMovingBehaviourState newState)
            {
                ChangeCameraSubState(ref CurrentMovingBehaviourState, newState);
                ChangeMovingBehaviourStateEvent();
            }
            public static void ChangeDepthScalingBehaviour(CameraDepthScalingBehaviourState newState)
            {
                ChangeCameraSubState(ref CurrentDepthScalingBehaviourState, newState);
                ChangeDepthScalingBehaviourStateEvent();
            }

            public static void ValidateMode()
            {
                if (CurrentDepthGetter == null)
                    throw ServantException.GetNullInitialization("CurrentDepthGetter");
                if (CurrentTargetPositionGetter == null)
                    throw ServantException.GetNullInitialization("CurrentTargetPositionGetter");
                if (CurrentMovingBehaviourState == null)
                    throw ServantException.GetNullInitialization("CurrentMovingBehaviourState");
                if (CurrentDepthScalingBehaviourState == null)
                    throw ServantException.GetNullInitialization("CurrentDepthScalingBehaviourState");
            }
        }

        private abstract class CameraSubState
        {
            public abstract void EnterStateAction();
            public abstract void ExitStateAction();
        }
        private abstract class CameraGetPositionState : CameraSubState
        {
            public abstract Vector2 TargetPosition_ { get; }
            public abstract GetPositionStateType StateType_ { get; }
        }
        private abstract class CameraGetDepthState: CameraSubState
        {
            public abstract float Depth_ { get; }
            public abstract GetDepthStateType StateType_ { get; }
        }
        private abstract class CameraMovingBehaviourState : CameraSubState
        {
            public abstract Vector2 GetCurrentPosition(Vector2 targetPos);
            public abstract MovingBehaviourStateType StateType_ { get; }
        }
        private abstract class CameraDepthScalingBehaviourState : CameraSubState
        {
            public abstract float GetCurrentDepth(float targetDepth);
            public abstract DepthScalingStateType StateType_ { get; }
        }

        private void UpdateCameraPosition(Vector2 newPos)
        {
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
        private void LateUpdate()
        {
            CameraMode.ValidateMode();

            Vector2 targetPos = CameraMode.CurrentTargetPositionGetter_.TargetPosition_;
            UpdateCameraPosition(CameraMode.CurrentMovingBehaviourState_.GetCurrentPosition(targetPos));

            float targetDepth = CameraMode.CurrentDepthGetter_.Depth_;
            CameraComp.depth = CameraMode.CurrentDepthScalingBehaviourState_.GetCurrentDepth(targetDepth);
        }
        public Vector2 GetCursorPos() => CameraComp.ScreenToWorldPoint
                (new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.y));
        //UnityAPI
        private void Awake()
        {
            if (!enabled)
                enabled = true;
        }
        private void OnDisable() =>
            enabled = true;
        private void Start()
        {
            this.ValidateSingltone();
            SetCameraMode_Default();
        }
    }
}

