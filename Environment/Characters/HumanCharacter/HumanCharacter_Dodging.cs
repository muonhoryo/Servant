using System;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter_OLD
    {
        public event Action<float> StartDodgingEvent=delegate { };
        public event Action StopDodgingEvent=delegate { };
        private CompositeParameter.ICharacterConstModifier DodgingSpeedModifier;
        public bool CanDodge_ => !IsDodging_ && !IsLockedControl_ && !this.IsInAir()&& 
            !this.HasWallsAtMovingDirection(WallChecker_);
        public bool CanStopDodge_ => IsDodging_&& CanStopDodge;
        public bool IsDodging_ { get; private set; } = false;
        private bool IsForceDodge = false;
        public float CurrentDodgingSpeedBuff_ { get=>DodgingSpeedModifier.Modifier_; }
        private bool CanStopDodge = true;
        private void InternalStopDodging()
        {
            ChangeLayerToCharacters();
            CanChangeMovingDirection = true;
            SetDodgingAnim(false);
            IsDodging_ = false;
            StopDodgingEvent();
        }
        private void InternalStartDodging()
        {
            if (!IsMoving_)
                InternalStartMoving();
            MovMode_TurnToDodging();
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
                            InternalStartMoving();
                        MovMode_TurnToForceDodgingMode();
                    }
                    if (angle > 180)
                    {
                        if (360 - angle >= GlobalConstants.Singlton.HumanCharacters_GroundForceDodgeMinAngle)
                        {
                            InternalSetMovingDirection(1);
                            StartForceDodging();
                        }
                    }
                    else
                    {
                        if (angle >= GlobalConstants.Singlton.HumanCharacters_GroundForceDodgeMinAngle)
                        {
                            InternalSetMovingDirection(-1);
                            StartForceDodging();
                        }
                    }
                }
            }
            GroundChecker_.UpdateGroundAngleEvent += StepOnClineAction;
        }
    }
}
