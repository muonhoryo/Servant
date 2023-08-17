


using System;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        private const string IsMovingAnimName = "IsMove";
        private const string IsFallingAnimName = "IsFall";
        private const string JumpAnimName = "Jump";
        private const string IsGroundFreeRisingAnimName = "IsGroundFreeRising";
        private const string IsDodgingAnimName = "IsDodge";
        private const string IsClimbingAnimName = "IsClimbing";
        private const string LandingAnimName = "LandingRoll";
        private const int LandingAnimDiagIndex = 1;
        private const int LandingAnimVertIndex = 2;
        private const int FreeAnimLayerIndex = 0;
        private const int GarpoonAnimLayerIndex = 1;

        [SerializeField]
        private SpriteRenderer spriteRenderer;


        public void LockingAnimationExit()
        {
            LockingAnimationExitEvent();
        }

        protected override void SetIsLeftSideAction(bool value)=>
            spriteRenderer.flipX = IsLeftSide_;
        private void SetFallingAnim(bool isFalling) => Animator_.SetBool(IsFallingAnimName, isFalling);
        private void SetGroundFreeRisingAnim(bool isGroundFreeRising) =>
            Animator_.SetBool(IsGroundFreeRisingAnimName, isGroundFreeRising);
        private void SetMovingAnim(bool isMoving) => Animator_.SetBool(IsMovingAnimName, isMoving);
        private void SetDodgingAnim(bool IsDodging) => Animator_.SetBool(IsDodgingAnimName, IsDodging);
        private void SetClimbingAnim(bool IsClimbing) => Animator_.SetBool(IsClimbingAnimName, IsClimbing);
        private void TurnAnimToFalling(IGroundCharacter.FallingStartInfo info)
        {
            if(info.WasGroundFreeRising)
                SetGroundFreeRisingAnim(false);
            SetFallingAnim(true);
        }
        private void TurnAnimToGroundFreeRising(IGroundCharacter.GroundFreeRisingInfo info)
        {
            if(info.WasFalling)
                SetFallingAnim(false);
            SetGroundFreeRisingAnim(true);
        }
        private void TurnAirAnimOff(IGroundCharacter.LandingInfo info)
        {
            SetGroundFreeRisingAnim(false);
            SetFallingAnim(false);
        }

        private void AwakeAction_Animation()
        {
            if (spriteRenderer == null)
                if (!TryGetComponent(out spriteRenderer))
                    throw ServantException.GetNullInitialization("spriteRenderer");
        }
        private void AfterAwakeAction_Animation()
        {
            StopMovingEvent += () => SetMovingAnim(false);
            StartFallingEvent += TurnAnimToFalling;
            StartGroundFreeRisingEvent += TurnAnimToGroundFreeRising;
            LandingEvent += TurnAirAnimOff;
            SetIsLeftSideAction(spriteRenderer.flipX);
        }
    }
}
