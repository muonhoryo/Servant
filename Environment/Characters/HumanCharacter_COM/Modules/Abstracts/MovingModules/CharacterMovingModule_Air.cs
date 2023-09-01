using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class CharacterMovingModule_Air : CharacterMovingModule,
        IModuleChangingScript<IFallingCheckingModule>,
        IModuleChangingScript<IWallCheckingModule>
    {
        IFallingCheckingModule IModuleChangingScript<IFallingCheckingModule>.Module__ 
        {
            get => FallingChecker;
            set
            {
                FallingChecker = value;
                if (value != null&&IsMoving_)
                    SubscribeOn_FallingChecker();
            }
        }
        IWallCheckingModule IModuleChangingScript<IWallCheckingModule>.Module__
        {
            get => WallChecker;
            set
            { 
                WallChecker = value;
                if(value!=null&&IsMoving_)
                    SubscribeOn_WallChecker(MovingDirModule_.MovingDirection_);
            }
        }

        [SerializeField]
        private Component FallingCheckerComponent;
        [SerializeField]
        private Component WallCheckerComponent;

        private IFallingCheckingModule FallingChecker;
        private IWallCheckingModule WallChecker;
        protected sealed override Vector2 GetMovingDirection() => Vector2.right;
        protected sealed override void MovingAction(Vector2 direction, int horizontalDirection, float speed,float speedModifier)
        {
            Rigidbody_.AddForce(speed * direction* speedModifier * horizontalDirection* MovingSpeedModifier_, ForceMode2D.Force);
        }
        protected sealed override bool CanStartMoving_AdditionalConditions =>
            !IsLockedControl_&& !WallChecker.HasWallAtDirection(MovingDirModule_.MovingDirection_) 
            && FallingChecker.IsInAir()
            && !DoMeleeShoot_;
        protected sealed override void AwakeAction()
        {
            FallingChecker = FallingCheckerComponent as IFallingCheckingModule;
            if (FallingChecker == null)
                throw ServantException.GetNullInitialization("FallingChecker");
            WallChecker = WallCheckerComponent as IWallCheckingModule;
            if (WallChecker == null)
                throw ServantException.GetNullInitialization("WallChecker");
        }
        private void Start()
        {
            StopMovingEvent += () =>
            {
                WallChecker.FoundWallAtRightSideEvent -= StopMoving;
                WallChecker.FoundWallAtLeftSideEvent -= StopMoving;
                FallingChecker.LandingEvent -= StopMovingAction_Land;
                MovingDirModule_.ChangeMovingDirectionEvent -= ChangeMovingDirectionAction;
            };
            StartMovingEvent += (direction) =>
            {
                SubscribeOn_WallChecker(direction);
                SubscribeOn_FallingChecker();
                SubscribeOn_MovingDirModule();
            };
            ChangeMovingDirModuleEvent += () =>
            { if (MovingDirModule_ != null && IsMoving_) SubscribeOn_MovingDirModule(); };
        }
        private void SubscribeOn_FallingChecker()
        {
            FallingChecker.LandingEvent += StopMovingAction_Land;
        }
        private void StopMovingAction_Land(IFallingCheckingModule.LandingInfo i) =>
            StopMoving();
        private void SubscribeOn_WallChecker(int direction)
        {
            if (direction > 0)
            {
                WallChecker.FoundWallAtRightSideEvent += StopMoving;
                WallChecker.FoundWallAtLeftSideEvent -= StopMoving;
            }
            else
            {
                WallChecker.FoundWallAtLeftSideEvent += StopMoving;
                WallChecker.FoundWallAtRightSideEvent -= StopMoving;
            }
        }
        private void SubscribeOn_MovingDirModule()
        {
            MovingDirModule_.ChangeMovingDirectionEvent += ChangeMovingDirectionAction;
        }
        private void ChangeMovingDirectionAction(int direction)
        {
            if (WallChecker.HasWallAtDirection(direction))
            {
                StopMoving();
            }
            else
            {
                SubscribeOn_WallChecker(direction);
            }
        }
    }
}
