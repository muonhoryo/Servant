


using System;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter_OLD
    {
        public override bool CanSetMovingDirection_ => CanChangeMovingDirection&&!IsLockedControl_;
        protected override void InternalStartMoving()
        {
            IsMoving_ = true;
            SetMovingAnim(true);
            if (!this.IsInAir())
                if (IsDodging_)
                {
                    MovMode_TurnToDodging();
                    StartDodgingEvent(DodgingSpeedModifier.Modifier_);
                }
                else
                    MovMode_TurnToGround();
            else if (IsRocking_)
                MovMode_TurnToRocking();
            else
                MovMode_TurnToFalling();

            void RunMoving()
            {
                CurrentMovingMode.MovingAction();
            }
            void StopMovingAction()
            {
                CurrentMovingMode.ExitAction();
                StopMovingEvent -= StopMovingAction;
                UnityFixedUpdateEvent -= RunMoving;
            }
            UnityFixedUpdateEvent += RunMoving;
            StopMovingEvent += StopMovingAction;
            RunStartMovingEvent(MovingDirection_);
        }

        private MovingMode CurrentMovingMode = new(null, null, null);
        private event Action ChangeMovingModeEvent=delegate { };

        //MovingMode
        private void ChangeMovingMode(MovingMode mode)
        {
            if (mode != CurrentMovingMode)
            {
                CurrentMovingMode.ExitAction?.Invoke();
                CurrentMovingMode = mode;
                CurrentMovingMode.EnterAction?.Invoke();
                ChangeMovingModeEvent();
            }
        }
        //Moving
        private void MovMode_GroundMovingAction()
        {
            NoneAcceleratedGroundMoving((float)MoveSpeed_);
        }
        private void MovMode_AirMovingAction()
        {
            AcceleratedMoving((float)MoveSpeed_ * GlobalConstants.Singlton.HumanCharacters_AirMovingSpeedModifier,
                new Vector2(MovingDirection_, 0));
        }
        private void MovMode_RockingMoving()
        {
            AcceleratedRockingMoving(this, GlobalConstants.Singlton.HumanCharacters_RockingMoveSpeed);
        }
        private void MovMode_DodgingMoving()
        {
            MovMode_GroundMovingAction();
            if (DodgingSpeedModifier.Modifier_ <= 0)
            {
                MovMode_TurnToGround();
            }
            else
            {
                float newBuff = DodgingSpeedModifier.Modifier_ - GlobalConstants.Singlton.HumanCharacters_DodgingSpeedDescentStep;
                DodgingSpeedModifier.UpdateModifier(newBuff);
            }
        }
        //Enter
        private void MovMode_GroundEnterAction()
        {
            StartFallingEvent += MovMode_TurnToAirMode_Fall;
            StartGroundFreeRisingEvent += MovMode_TurnToAirMode_Rising;
            JumpEvent += MovMode_JumpAction;
            StopMovingEvent += MovMode_GroundStop;
        }
        private void MovMode_AirMovingEnter()
        {
            LandingEvent += MovMode_Air_TurnToGround;
            StartRopeRockingEvent += MovMode_TurnToRocking;
        }
        private void MovMode_RockingEnter()
        {
            LandingEvent += MovMode_Air_TurnToGround;
            StopRopeRockingEvent += MovMode_TurnToFalling;
        }
        private void MovMode_DodgingEnter()
        {
            MovMode_GroundEnterAction();
            float speedValue;
            float groundDirectVelocity = MathF.Abs(Rigidbody_.velocity.magnitude * GroundCorrectDirection_.x);
            if (groundDirectVelocity-(float)MoveSpeed_ > GlobalConstants.Singlton.HumanCharacters_DodgingSpeedMinBuff)
            {
                speedValue = groundDirectVelocity - (float)MoveSpeed_;
            }
            else
            {
                speedValue = GlobalConstants.Singlton.HumanCharacters_DodgingSpeedMinBuff;
            }
            DodgingSpeedModifier=MoveSpeed_.AddModifier_Add(speedValue);
            CanChangeMovingDirection = false;
            IsDodging_ = true;
            SetDodgingAnim(true);
            ChangeLayerToBackGround();
            StartDodgingEvent(DodgingSpeedModifier.Modifier_);
        }
        private void MovMode_ForceDodgingEnter()
        {
            MovMode_DodgingEnter();
            CanStopMoving = false;
            CanStopDodge = false;
            IsForceDodge = true;
            GroundChecker_.UpdateGroundAngleEvent += MovMode_ForceDodgingGroundAngleChange;
        }
        //Exit
        private void MovMode_GroundExit()
        {
            StartFallingEvent -= MovMode_TurnToAirMode_Fall;
            StartGroundFreeRisingEvent -= MovMode_TurnToAirMode_Rising;
            JumpEvent -= MovMode_JumpAction;
            StopMovingEvent -= MovMode_GroundStop;
        }
        private void MovMode_AirExit()
        {
            LandingEvent -= MovMode_Air_TurnToGround;
            StartRopeRockingEvent -= MovMode_TurnToRocking;
        }
        private void MovMode_RockingExit()
        {
            LandingEvent -= MovMode_Air_TurnToGround;
            StopRopeRockingEvent -= MovMode_TurnToFalling;
            IsLeftSide_ = Rigidbody_.velocity.x < 0;
        }
        private void MovMode_DodgingExit()
        {
            MovMode_GroundExit();
            InternalStopDodging();
            DodgingSpeedModifier?.RemoveModifier();
            DodgingSpeedModifier = null;
        }
        private void MovMode_ForceDodgingExit()
        {
            MovMode_DodgingExit();
            CanStopMoving = true;
            CanStopDodge = true;
            IsForceDodge = false;
            GroundChecker_.UpdateGroundAngleEvent -= MovMode_ForceDodgingGroundAngleChange;
        }
        //Other
        private void MovMode_GroundStop()
        {
            Rigidbody_.velocity = Vector2.zero;
        }
        private void MovMode_JumpAction()
        {
            void JumpDelayAction()
            {
                JumpDelayDoneEvent -= JumpDelayAction;
                if (!this.IsInAir())
                    MovMode_TurnToGround();
            }
            MovMode_TurnToAirMode();
            JumpDelayDoneEvent += JumpDelayAction;
        }
        private void MovMode_ForceDodgingGroundAngleChange(float angle)
        {
            if (angle > 180)
                angle = 360 - angle;

            if (angle < GlobalConstants.Singlton.HumanCharacters_GroundForceDodgeMinAngle)
            {
                InternalStopDodging();
                InternalStopMoving();
            }
        }

        private void MovMode_TurnToGround() =>
            ChangeMovingMode( GetGroundMode());
        private void MovMode_TurnToFalling() =>
            ChangeMovingMode(GetFallingMode());
        private void MovMode_TurnToRocking() =>
            ChangeMovingMode(GetRockingMode());
        private void MovMode_TurnToDodging() =>
            ChangeMovingMode(GetDodgingMode());
        private void MovMode_Air_TurnToGround(IGroundCharacter.LandingInfo i) =>
            MovMode_TurnToGround();
        private void MovMode_TurnToAirMode_Fall(IGroundCharacter.FallingStartInfo i) =>
            MovMode_TurnToAirMode();
        private void MovMode_TurnToAirMode_Rising(IGroundCharacter.GroundFreeRisingInfo i) =>
            MovMode_TurnToAirMode();
        private void MovMode_TurnToAirMode() =>
            ChangeMovingMode(IsRocking_ ? GetRockingMode() : GetFallingMode());
        private void MovMode_TurnToForceDodgingMode() =>
            ChangeMovingMode(GetForceDodgingMode());

        private MovingMode GetGroundMode() => new MovingMode
            (MovingAction: MovMode_GroundMovingAction,
             EnterAction: MovMode_GroundEnterAction,
             ExitAction: MovMode_GroundExit);
        private MovingMode GetFallingMode() => new MovingMode
            (MovingAction: MovMode_AirMovingAction,
             EnterAction: MovMode_AirMovingEnter,
             ExitAction: MovMode_AirExit);
        private MovingMode GetRockingMode() => new MovingMode
            (MovingAction: MovMode_RockingMoving,
             EnterAction: MovMode_RockingEnter,
             ExitAction: MovMode_RockingExit);
        private MovingMode GetDodgingMode() => new MovingMode
            (MovingAction: MovMode_DodgingMoving,
             EnterAction: MovMode_DodgingEnter,
             ExitAction: MovMode_DodgingExit);
        private MovingMode GetForceDodgingMode() => new MovingMode
            (MovingAction: MovMode_GroundMovingAction,
             EnterAction: MovMode_ForceDodgingEnter,
             ExitAction: MovMode_ForceDodgingExit);

        private class MovingMode
        {
            public MovingMode(Action MovingAction, Action EnterAction, Action ExitAction)
            {
                this.MovingAction = MovingAction;
                this.EnterAction = EnterAction;
                this.ExitAction = ExitAction;
            }
            public readonly Action MovingAction;
            public readonly Action EnterAction;
            public readonly Action ExitAction;
        }
    }
}
