using Servant.Characters.COP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters
{

    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class CharacterMovingModule_SimpleGround : CharacterMovingModule,
        IModuleChangingScript<IGroundDirectionCalculatingModule>,
        IModuleChangingScript<IFallingCheckingModule>,
        IModuleChangingScript<IWallCheckingModule>
    {
        IGroundDirectionCalculatingModule IModuleChangingScript<IGroundDirectionCalculatingModule>.Module__
        { get => GroundDirectionCalculator; set => GroundDirectionCalculator = value; }
        IFallingCheckingModule IModuleChangingScript<IFallingCheckingModule>.Module__ 
        {
            get => FallingCheckingModule;
            set 
            { 
                FallingCheckingModule = value;
                if (value != null && IsMoving_)
                    SubscribeOn_FallingChecker();
            }
        }
        IWallCheckingModule IModuleChangingScript<IWallCheckingModule>.Module__ 
        {
            get => WallChecker;
            set 
            {
                WallChecker = value;
                SubscribeOn_WallChecker(MovingDirModule_.MovingDirection_);
            }
        }

        [SerializeField]
        private Component GroundDirectionCalculatorComponent;
        [SerializeField]
        private Component FallingCheckingModuleComponent;
        [SerializeField]
        private Component WallCheckerComponent;

        private IGroundDirectionCalculatingModule GroundDirectionCalculator;
        private IFallingCheckingModule FallingCheckingModule;
        private IWallCheckingModule WallChecker;

        protected sealed override Vector2 GetMovingDirection() =>
            GroundDirectionCalculator.GroundDirection_;
        protected sealed override void MovingAction(Vector2 direction, int horizontalDirection, float speed,float speedModifier)
        {
            Rigidbody_.velocity = speed * horizontalDirection * speedModifier*direction;
        }
        protected sealed override bool CanStartMoving_AdditionalConditions =>
            !FallingCheckingModule.IsInAir() &&
                !WallChecker.HasWallAtDirection(MovingDirModule_.MovingDirection_);
        protected sealed override void AwakeAction()
        {
            GroundDirectionCalculator = GroundDirectionCalculatorComponent as IGroundDirectionCalculatingModule;
            if (GroundDirectionCalculator == null)
                throw ServantException.GetNullInitialization("GroundDirectionCalculator");
            FallingCheckingModule = FallingCheckingModuleComponent as IFallingCheckingModule;
            if (FallingCheckingModule == null)
                throw ServantException.GetNullInitialization("FallingCheckingModule");
            WallChecker = WallCheckerComponent as IWallCheckingModule;
            if (WallChecker == null)
                throw ServantException.GetNullInitialization("WallChecker");
        }
        private void Start()
        {
            StopMovingEvent += () =>
            {
                FallingCheckingModule.StartFallingEvent -= StopMovingAction_Falling;
                FallingCheckingModule.StartRisingEvent -= StopMovingAction_Rising;
                WallChecker.FoundWallAtLeftSideEvent -= StopMoving;
                WallChecker.FoundWallAtRightSideEvent -= StopMoving;
                MovingDirModule_.ChangeMovingDirectionEvent -= ChangeMovingDirectionAction;
                if (IsActive_)
                    Rigidbody_.velocity = Vector2.zero;
            };
            StartMovingEvent += (int i) =>
            {
                SubscribeOn_FallingChecker();
                SubscribeOn_WallChecker(MovingDirModule_.MovingDirection_);
                SubscribeOn_MovingDirModule();
            };
            ChangeMovingDirModuleEvent += () =>
            { if (MovingDirModule_ != null && IsMoving_) SubscribeOn_MovingDirModule(); };
        }
        private void SubscribeOn_FallingChecker()
        {
            FallingCheckingModule.StartFallingEvent += StopMovingAction_Falling;
            FallingCheckingModule.StartRisingEvent += StopMovingAction_Rising;
        }
        private void StopMovingAction_Falling(IFallingCheckingModule.FallingStartInfo i) =>
            StopMoving();
        private void StopMovingAction_Rising(IFallingCheckingModule.GroundFreeRisingInfo i) =>
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
