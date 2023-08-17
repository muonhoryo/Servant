using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant
{
    public sealed partial class MainCameraBehaviour
    {
        //SetPositionGetter
        public void SetPositionGetter_Point(Vector2 point)
        {
            CameraMode.ChangePositionGetter(new CameraGetPointPositionState(point));
        }
        public void SetPositionGetter_Target(Transform target)
        {
            CameraMode.ChangePositionGetter(new CameraGetTargetPositionState(target));
        }
        public void SetPositionGetter_Target(Transform target,Rect moveLimit,Rect moveTrigger)
        {
            CameraMode.ChangePositionGetter
                (new CameraGetTargetBoundedPositionState(target, moveLimit, moveTrigger));
        }
        //SetDepthGetter
        public void SetDepthGetter_Const(float depth)
        {
            CameraMode.ChangeDepthGetter(new CameraGetConstDepthState(depth));
        }
        //SetMovingBehaviour
        public void SetMovingBehaviour_Instant()
        {
            CameraMode.ChangeMovingBehaviour(new CameraInstantMovingBehaviourState());
        }
        //SetDepthScalingBehaviour
        public void SetDepthScalingBehaviour_Instant()
        {
            CameraMode.ChangeDepthScalingBehaviour(new CameraInsantDepthScalingBehaviourState());
        }
        //SetCameraMode
        public void SetCameraMode_Default()
        {
            SetPositionGetter_Point(Vector2.zero);
            SetDepthGetter_Const(CameraComp.depth);
            SetMovingBehaviour_Instant();
            SetDepthScalingBehaviour_Instant();
        }
        public void SetCameraMode_CharacterTracking(Transform character,Rect moveLimit,Rect moveTrigger,float depth)
        {
            SetPositionGetter_Target(character,moveLimit, moveTrigger);
            SetDepthGetter_Const(depth);
            SetMovingBehaviour_Instant();
            SetDepthScalingBehaviour_Instant();
        }
    }
}
