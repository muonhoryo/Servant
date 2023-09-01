using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COM
{
    [RequireComponent(typeof(ModuleInitializer<IMovingDirectionChangingModule>),
        typeof(ModuleInitializer<ISpeedModule>))]
    [RequireComponent(typeof(ModuleInitializer<IFallingCheckingModule>),
        typeof(ModuleInitializer<IWallCheckingModule>))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class CharacterMovingModule_Air : CharacterMovingModule
    {
        private IFallingCheckingModule FallingChecker;
        private IWallCheckingModule WallChecker;
        [SerializeField]
        private Rigidbody2D Rigidbody;
        [SerializeField]
        protected float MovingSpeedModifier;
        protected override Vector2 GetMovingDirection() => Vector2.right;
        protected override void MovingAction(Vector2 direction, int horizontalDirection, float speed)
        {
            Rigidbody.AddForce(speed * direction* horizontalDirection* MovingSpeedModifier, ForceMode2D.Force);
        }
        protected override bool CanStartMoving_AdditionalConditions =>
            !IsLockedControl_&& !WallChecker.HasWallAtDirection(MovingDirModule_.MovingDirection_) 
            && FallingChecker.IsInAir()
            && !DoMeleeShoot_;
        private void Awake()
        {
            if (Rigidbody == null)
                if (!TryGetComponent(out Rigidbody))
                    throw ServantException.GetNullInitialization("Rigidbody");

            StopMovingEvent += StopMovingAction;
            StartMovingEvent += (i) =>
            {
                FallingChecker.LandingEvent += StopMovingAction_Land;
            };
        }
        private void StopMovingAction()
        {
            FallingChecker.LandingEvent -= StopMovingAction_Land;
        }
        private void StopMovingAction_Land(IFallingCheckingModule.LandingInfo i) =>
            StopMoving();
        private void Start()
        {
            void ChangeDirectionAction(int direction)
            {
                if (WallChecker.HasWallAtDirection(direction))
                {
                    StopMoving();
                }
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
            MovingDirModule_.ChangeMovingDirectionEvent += ChangeDirectionAction;
            ChangeDirectionAction(MovingDirModule_.MovingDirection_);
        }
    }
}
