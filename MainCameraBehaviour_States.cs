using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant
{
    public sealed partial class MainCameraBehaviour
    {
        //GetPositionStates
        [Serializable]
        public enum GetPositionStateType
        {
            GetPointPosition,
            GetTargetPosition,
            GetTargetPosition_Bounded
        }
        private sealed class CameraGetPointPositionState : CameraGetPositionState
        {
            public CameraGetPointPositionState(Vector2 GlobalPoint)
            {
                this.GlobalPoint = GlobalPoint;
            }
            private readonly Vector2 GlobalPoint;
            public override Vector2 TargetPosition_ => GlobalPoint;
            public override GetPositionStateType StateType_ => GetPositionStateType.GetPointPosition;
            public override void EnterStateAction() { }
            public override void ExitStateAction() { }
        }
        private class CameraGetTargetPositionState : CameraGetPositionState
        {
            public CameraGetTargetPositionState(Transform Target)
            {
                if (Target == null)
                    throw ServantException.GetArgumentNullException("Target");

                this.Target = Target;
            }
            protected readonly Transform Target;
            public override Vector2 TargetPosition_ => Target.position;
            public override GetPositionStateType StateType_ => GetPositionStateType.GetTargetPosition;
            public override void EnterStateAction() { }
            public override void ExitStateAction() { }
        }
        private sealed class CameraGetTargetBoundedPositionState : CameraGetTargetPositionState
        {
            public CameraGetTargetBoundedPositionState(Transform Target, Rect MoveLimit,Rect moveTrigger):base(Target)
            {
                this.MoveLimit = MoveLimit;
                CurrentMoveTrigger = moveTrigger;
            }
            private readonly Rect MoveLimit;
            private Rect CurrentMoveTrigger;
            private void UpdateCurrentMoveTrigger(Vector2 charPos)
            {
                float x = CurrentMoveTrigger.xMin;
                float y = CurrentMoveTrigger.yMin;

                if (charPos.x > CurrentMoveTrigger.xMax)
                {
                    if (charPos.x > MoveLimit.xMax)
                        x = (MoveLimit.xMax - CurrentMoveTrigger.width);
                    else
                        x += (charPos.x - CurrentMoveTrigger.xMax);
                }
                else if (charPos.x < CurrentMoveTrigger.xMin)
                {
                    if (charPos.x < MoveLimit.xMin)
                        x = MoveLimit.xMin;
                    else
                        x = charPos.x;
                }

                if (charPos.y > CurrentMoveTrigger.yMax)
                {
                    if (charPos.y > MoveLimit.yMax)
                        y = (MoveLimit.yMax - CurrentMoveTrigger.height);
                    else
                        y += (charPos.y - CurrentMoveTrigger.yMax);
                }
                else if (charPos.y < CurrentMoveTrigger.yMin)
                    if (charPos.y < MoveLimit.yMin)
                        y = MoveLimit.yMin;
                    else
                        y = charPos.y;

                CurrentMoveTrigger = new Rect(x, y, CurrentMoveTrigger.width, CurrentMoveTrigger.height);
            }
            public override Vector2 TargetPosition_ 
            {
                get
                {
                    if (!CurrentMoveTrigger.Contains(Target.position))
                    {
                        UpdateCurrentMoveTrigger(Target.position);
                        return CurrentMoveTrigger.center;
                    }
                    return CurrentMoveTrigger.center;
                }
            }
            public override GetPositionStateType StateType_ => GetPositionStateType.GetTargetPosition_Bounded;
        }
        //GetDepthStates
        [Serializable]
        public enum GetDepthStateType 
        {
            Const
        }
        private sealed class CameraGetConstDepthState : CameraGetDepthState
        {
            public CameraGetConstDepthState(float Depth)
            {
                this.Depth = Depth;
            }
            private readonly float Depth;
            public override float Depth_ => Depth;
            public override GetDepthStateType StateType_ => GetDepthStateType.Const;
            public override void EnterStateAction() { }
            public override void ExitStateAction() { }
        }
        //MovingStates
        [Serializable]
        public enum MovingBehaviourStateType
        {
            Instant
        }
        private sealed class CameraInstantMovingBehaviourState : CameraMovingBehaviourState
        {
            public override void EnterStateAction() { }
            public override void ExitStateAction() { }
            public override Vector2 GetCurrentPosition(Vector2 targetPos) =>
                targetPos;
            public override MovingBehaviourStateType StateType_ => MovingBehaviourStateType.Instant;
        }
        //DepthScalingStates
        [Serializable]
        public enum DepthScalingStateType
        {
            Instant
        }
        private sealed class CameraInsantDepthScalingBehaviourState : CameraDepthScalingBehaviourState
        {
            public override void EnterStateAction() { }
            public override void ExitStateAction() { }
            public override float GetCurrentDepth(float targetDepth) =>
                targetDepth;
            public override DepthScalingStateType StateType_ => DepthScalingStateType.Instant;
        }
    }
}
