using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter_OLD
    {
        public event Action<IClimbingCharacter.ClimbingInfo> StartClimbingEvent=delegate { };
        public event Action ClimbingDoneEvent=delegate { };

        private IClimbableGroundChecker ClimbableGroundChecker;
        public  bool CanClimb_
            =>!IsLockedControl_&& !IsRocking_ && !IsClimbing_ && ClimbableGroundChecker.HasClimbableGroundAround()&&
            !DoMeleeShoot_;
        public bool IsClimbing_ { get; private set; }

        private void ClimbingAnimationExit()
        {
            SetClimbingAnim(false);
            LockingAnimationExitEvent -= ClimbingAnimationExit;
            IsLockedControl_ = false;
            IsClimbing_ = false;
            Rigidbody_.isKinematic = false;
            ClimbingDoneEvent();
        }
        public void CancelClimb()
        {
            if (IsClimbing_)
            {
                SetClimbingAnim(false);
            }
        }

        private void InternalClimb()
        {
            Vector2 pos = ClimbableGroundChecker.GetClimbPosition();
            pos = new Vector2(pos.x, pos.y + HitBox.size.y / 2);
            transform.position = pos;
            if (IsMoving_)
                StopMoving();
            if (GarpoonBase_.DidGarpoonShoot())
                GarpoonBase_.CatchHookOff();
            IsClimbing_ = true;
            SetClimbingAnim(true);
            IsLeftSide_ = ClimbableGroundChecker.ClimbableGroundDirection_ < 0;
            Rigidbody_.isKinematic = true;
            Rigidbody_.velocity = Vector2.zero;
            IsLockedControl_ = true;
            LockingAnimationExitEvent += ClimbingAnimationExit;
            StartClimbingEvent(new IClimbingCharacter.ClimbingInfo
                (ClimbableGroundChecker.ClimbableGround_, ClimbableGroundChecker.ClimbableGroundDirection_));
        }

        private void AwakeAction_Climbing()
        {
            ClimbableGroundChecker = GetComponentInChildren<IClimbableGroundChecker>(true);
            if (ClimbableGroundChecker == null)
                throw ServantException.GetNullInitialization("ClimbableGroundChecker");
        }
    }
}
