using System;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        public event Action<float> StartDodgingEvent=delegate { };
        public event Action StopDodgingEvent=delegate { };
        private CompositeParameter.ICharacterConstModifier DodgingSpeedModifier;
        public bool CanDodge_ => !IsDodging_ && !IsLockedControl_ && !this.IsInAir()&&IsMoving_;
        public bool CanStopDodge_ => IsDodging_&& CanStopDodge;
        public bool IsDodging_ { get; private set; } = false;
        private bool IsForceDodge = false;
        public float CurrentDodgingSpeedBuff_ { get=>DodgingSpeedModifier.Modifier_; }
        private bool CanStopDodge = true;
        private void InternalStopDodging()
        {
            CanChangeMovingDirection = true;
            SetDodgingAnim(false);
            IsDodging_ = false;
            StopDodgingEvent();
        }
        private void InternalStartDodging()
        {
            CanChangeMovingDirection = false;
            IsDodging_ = true;
            SetDodgingAnim(true);
            StartDodgingEvent(DodgingSpeedModifier.Modifier_);
        }

        private void AwakeAction_Dodging()
        {
            void StepOnClineAction(float angle)
            {
                if (!IsForceDodge)
                {
                    void StartForceDodging()
                    {
                        if (!IsMoving_)
                            StartMoving();
                        MovMode_TurnToForceDodgingMode();
                        InternalStartDodging();
                    }
                    if (angle > 180)
                    {
                        if (360 - angle >= GlobalConstants.Singlton.HumanCharacters_GroundForceDodgeMinAngle)
                        {
                            if (!CanChangeMovingDirection)
                                CanChangeMovingDirection = true;
                            MovingDirection_ = 1;
                            StartForceDodging();
                        }
                    }
                    else
                    {
                        if (angle >= GlobalConstants.Singlton.HumanCharacters_GroundForceDodgeMinAngle)
                        {
                            if (!CanChangeMovingDirection)
                                CanChangeMovingDirection = true;
                            MovingDirection_ = -1;
                            StartForceDodging();
                        }
                    }
                }
            }
            GroundChecker_.UpdateGroundAngleEvent += StepOnClineAction;
        }
    }
}
