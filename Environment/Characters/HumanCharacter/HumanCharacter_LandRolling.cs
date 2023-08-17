using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        private enum LandRollingType 
        {
            Diagonal,
            Vert
        }
        public event Action StartLandingRollEvent=delegate { };
        public event Action LandingRollDoneEvent=delegate { };

        public bool IsLandRolling_ { get; private set; } = false;
        private LandRollingType RollingType;
        
        public void CancelLandingRoll()
        {
            if (IsLandRolling_)
            {
                Animator_.SetInteger(LandingAnimName, 0);
            }
        }
        private void LandingRoll_Diagonal()
        {
            void MoveAction()
            {
                NoneAcceleratedGroundMoving(
                    (float)MoveSpeed_ * GlobalConstants.Singlton.HumanCharacters_LandingDiagonalRollSpeedModifier);
            }
            void ResetEvents()
            {
                UnityFixedUpdateEvent -= MoveAction;
                LandingRollDoneEvent -= ResetEvents;
            }
            UnityFixedUpdateEvent += MoveAction;
            LandingRollDoneEvent += ResetEvents;
            RollingType = LandRollingType.Diagonal;
            LandingRoll();
        }
        private void LandingRoll_Vertical()
        {
            Rigidbody_.velocity = Vector2.zero;
            RollingType = LandRollingType.Vert;
            LandingRoll();
        }
        private void LandingRoll()
        {
            if (GarpoonBase_.DidGarpoonShoot())
                GarpoonBase_.CatchHookOff();
            if (IsMoving_)
                StopMoving();
            Animator_.SetInteger(LandingAnimName,
                RollingType==LandRollingType.Diagonal? LandingAnimDiagIndex:LandingAnimVertIndex);
            IsLandRolling_ = true;
            IsLockedControl_ = true;
            void ResetEvent_Fall(IGroundCharacter.FallingStartInfo i)
            {
                CancelLandingRoll();
                StartFallingEvent -= ResetEvent_Fall;
            }
            void ResetEvent_Rising(IGroundCharacter.GroundFreeRisingInfo i)
            {
                CancelLandingRoll();
                StartGroundFreeRisingEvent -= ResetEvent_Rising;
            }
            StartFallingEvent += ResetEvent_Fall;
            StartGroundFreeRisingEvent += ResetEvent_Rising;
            LockingAnimationExitEvent += LandingRollAnimExit;
            StartLandingRollEvent();
        }
        private void LandingRollAnimExit()
        {
            LockingAnimationExitEvent-= LandingRollAnimExit;
            IsLandRolling_ = false;
            IsLockedControl_ = false;
            LandingRollDoneEvent();
        }

        private void AwakeAction_LandRolling()
        {
            void LandAction(IGroundCharacter.LandingInfo info)
            {
                if (info.LandingForce.magnitude > GlobalConstants.Singlton.HumanCharacters_LandingRollMinForce)
                {
                    if (Math.Abs(info.LandingForce.x) > info.LandingForce.y)
                        LandingRoll_Diagonal();
                    else
                        LandingRoll_Vertical();
                }
            }
            LandingEvent += LandAction;
        }
    }
}
