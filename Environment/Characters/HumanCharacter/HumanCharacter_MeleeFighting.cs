using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter_OLD
    {
        public event Action<int> MeleeShootEvent = delegate { };
        public event Action MeleeHitBoxActivateEvent=delegate { };
        public event Action MeleeHitBoxDeactivateEvent = delegate { };
        public event Action MeleeShootDoneEvent = delegate { };
        public bool DoMeleeShoot_ { get; private set; } = false;
        public bool IsStrongShoot_ => IsStrongShoot;
        public bool CanMeleeShoot_ =>!IsLockedControl_&&!DoMeleeShoot_&& GarpoonBase_.ShootedProjectile_==null&&!IsForceDodge;
        private IMeleeFightCharacter.IMeleeHitBox MeleeHitBox;

        public void MeleeShoot(int direction)
        {
            if (CanMeleeShoot_)
            {
                if (IsMoving_)
                    StopMoving();
                InternalSetMovingDirection(direction);
                if (this.IsInAir())
                {
                    Animator_.SetTrigger(MeleeAirShootAnimName);
                }
                else
                {
                    Animator_.SetTrigger(MeleeGroundShootAnimName);
                }
                DoMeleeShoot_ = true;
                CanChangeMovingDirection = false;
                void AnimationEnterAction()
                {
                    LockingAnimationEnterEvent -= AnimationEnterAction;
                    LockingAnimationExitEvent += RunMeleeDoneEvent;
                    void MeleeShootDoneAction()
                    {
                        MeleeShootDoneEvent -= MeleeShootDoneAction;
                        DoMeleeShoot_ = false;
                        CanChangeMovingDirection = true;
                    }
                    MeleeShootDoneEvent += MeleeShootDoneAction;
                    MeleeShootEvent(MovingDirection_);
                }
                LockingAnimationEnterEvent += AnimationEnterAction;
            }
        }

        private void RunMeleeDoneEvent()
        {
            LockingAnimationExitEvent -= RunMeleeDoneEvent;
            MeleeShootDoneEvent();
        }
        public void AnimatorFighting_LockControl()
        {
            LockingAnimationExitEvent += AnimatorFighting_UnlockControl;
            LockingAnimationExitEvent -= RunMeleeDoneEvent;
            IsLockedControl_ = true;
        }
        public void AnimatorFighting_UnlockControl()
        {
            LockingAnimationExitEvent -= AnimatorFighting_UnlockControl;
            IsLockedControl_ = false;
            MeleeShootDoneEvent();
        }
        public void Animator_ActivateHitBox()
        {
            LockingAnimationExitEvent += Animator_DeactivateHitBox;
            MeleeHitBox.IsActive_ = true;
            MeleeHitBoxActivateEvent();
        }
        public void Animator_DeactivateHitBox()
        {
            LockingAnimationExitEvent -= Animator_DeactivateHitBox;
            MeleeHitBox.IsActive_ = false;
            MeleeHitBoxDeactivateEvent();
        }

        public bool IsStrongShoot;

        private void AwakeAction_MeleeFighting()
        {
            MeleeHitBox = GetComponentInChildren<MeleeHitBox>();
            if (MeleeHitBox==null)
                throw ServantException.GetNullInitialization("MeleeHitBox");
        }
    }
}
