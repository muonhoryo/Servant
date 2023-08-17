


using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        private bool IsAllowedJump = true;
        public bool CanJump_ => IsAllowedJump && !IsLockedControl_&&!this.IsInAir();

        public event Action JumpEvent=delegate { };
        public event Action JumpDelayStartEvent=delegate { };
        public event Action JumpDelayDoneEvent=delegate { };
        public event Action JumpHasBeenAccepted=delegate { };

        private Coroutine CurrentJumpDelay;
        private IEnumerator JumpDelay()
        {
            IsAllowedJump = false;
            JumpDelayStartEvent();
            yield return new WaitForSeconds(GlobalConstants.Singlton.HumanCharacters_JumpDelay);
            RequestJumpAccepting();
            JumpDelayDoneEvent();
        }
        private void RequestJumpAccepting()
        {
            bool canJump = true;
            if (this.IsInAir())
            {
                void OnLandRequest(IGroundCharacter.LandingInfo i)
                {
                    LandingEvent -= OnLandRequest;
                    RequestJumpAccepting();
                }
                LandingEvent += OnLandRequest;
                canJump = false;
            }

            if (canJump)
                AcceptJump();
        }
        private void AcceptJump()
        {
            IsAllowedJump = true;
            JumpHasBeenAccepted();
        }
        private void StartJumpDelay()
        {
            if (IsAllowedJump)
                CurrentJumpDelay=StartCoroutine(JumpDelay());
            else if (CurrentJumpDelay != null)
            {
                StopCoroutine(CurrentJumpDelay);
                CurrentJumpDelay = StartCoroutine(JumpDelay());
            }
        }

        private void InternalJump()
        {
            JumpEvent();
            StartCoroutine(JumpDelay());
            Rigidbody_.AddForce(GlobalConstants.Singlton.HumanCharacters_JumpForce *
                Vector2.up, ForceMode2D.Impulse);
            Animator_.SetTrigger(JumpAnimName);
        }

        private void AwakeAction_Jumping()
        {
            StartFallingEvent += (i) => StartJumpDelay();
            StartGroundFreeRisingEvent += (i) => StartJumpDelay();
        }
    }
}
