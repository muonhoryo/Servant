using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter_OLD : GroundMovableCharacter, IHumanCharacter
    {
        public event Action LockControlEvent = delegate { };
        public event Action UnlockControlEvent = delegate { };
        private bool IsLockedControl = false;
        public bool IsLockedControl_
        {
            get => IsLockedControl;
            private set
            {
                if (IsLockedControl != value)
                {
                    IsLockedControl = value;
                    if (IsLockedControl)
                        LockControlEvent();
                    else
                        UnlockControlEvent();
                }
            }
        }

        [SerializeField]
        private CapsuleCollider2D HitBox;
        public void Climb()
        {
            if (CanClimb_)
            {
                InternalClimb();
            }
        }
        public void StartDodging()
        {
            if (CanDodge_)
            {
                InternalStartDodging();
            }
        }
        public void StopDodging()
        {
            if (CanStopDodge_)
            {
                MovMode_TurnToGround();
                InternalStopDodging();
            }
        }
        public void Jump()
        {
            if (CanJump_)
            {
                InternalJump();
            }
        }


        private void AwakeAction_Script()
        {
            if (HitBox == null)
                if (!TryGetComponent(out HitBox))
                    throw ServantException.GetNullInitialization("HitBox");
        }
    }
}
