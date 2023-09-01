using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IDodgingCharacter;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class CharacterDodgingModule:Module,IDodgingModule,
        IModuleChangingScript<IMovingModule>,
        IModuleChangingScript<IFallingCheckingModule>,
        IModuleChangingScript<IWallCheckingModule>,
        IModuleChangingScript<IMovingDirectionChangingModule>,
        IModuleChangingScript<IGroundDirectionCalculatingModule>,
        IModuleChangingScript<ISpeedModule>
    {
        public event Action<float> StartDodgingEvent=delegate { };
        public event Action StopDodgingEvent = delegate { };

        public bool CanStartDodge_
        {
            get => CanStartDodge__ && !FallingChecker.IsInAir()&&
                !WallChecker.HasWallAtDirection(MovingDirModule.MovingDirection_);
            set => CanStartDodge__ = value;
        }
        private bool CanStartDodge__ = true;
        public bool CanStopDodge_
        {
            get => CanStopDodge__ && IsDodging_;
            set => CanStopDodge__ = value;
        }
        private bool CanStopDodge__ = true;
        public bool IsDodging_ { get; private set; } = false;
        public float CurrentDodgingSpeedBuff_ =>CurrentSpeedBuff.Modifier_;
        private CompositeParameter.ICharacterConstModifier CurrentSpeedBuff;

        [SerializeField]
        private Rigidbody2D Rigidbody;

        [SerializeField]
        private Component MovingModuleComponent;
        [SerializeField]
        private Component FallingCheckerComponent;
        [SerializeField]
        private Component WallCheckerComponent;
        [SerializeField]
        private Component MovingDirModuleComponent;
        [SerializeField]
        private Component GroundDirectionCalculatorComponent;
        [SerializeField]
        private Component SpeedModuleComponent;

        private IMovingModule MovingModule;
        private IFallingCheckingModule FallingChecker;
        private IWallCheckingModule WallChecker;
        private IMovingDirectionChangingModule MovingDirModule;
        private IGroundDirectionCalculatingModule GroundDirectionCalculator;
        private ISpeedModule SpeedModule;

        IMovingModule IModuleChangingScript<IMovingModule>.Module__
        { get => MovingModule; set => MovingModule = value; }
        IFallingCheckingModule IModuleChangingScript<IFallingCheckingModule>.Module__
        { get => FallingChecker; set => FallingChecker = value; }
        IWallCheckingModule IModuleChangingScript<IWallCheckingModule>.Module__
        { get => WallChecker; set => WallChecker = value; }
        IMovingDirectionChangingModule IModuleChangingScript<IMovingDirectionChangingModule>.Module__
        { get => MovingDirModule; set => MovingDirModule = value; }
        IGroundDirectionCalculatingModule IModuleChangingScript<IGroundDirectionCalculatingModule>.Module__
        { get => GroundDirectionCalculator; set => GroundDirectionCalculator = value; }
        ISpeedModule IModuleChangingScript<ISpeedModule>.Module__
        { get => SpeedModule; set => SpeedModule = value; }

        public void StartDodging()
        {
            if (CanStartDodge_) 
            {
                IsDodging_= true;
                enabled = true;
                StartDodgingAction();
                StartDodgingEvent(CurrentSpeedBuff.Modifier_);
            }
        }
        public void StopDodging()
        {
            if (CanStopDodge_)
            {
                IsDodging_ = false;
                enabled = false;
                StopDodgingAction();
                StopDodgingEvent();
            }
        }
        private void StartDodgingAction()
        {
            float speedValue;
            {
                float groundDirectVelocity =
                    MathF.Abs(Rigidbody.velocity.magnitude * GroundDirectionCalculator.GroundDirection_.x);
                if (groundDirectVelocity - (float)SpeedModule.MoveSpeed_ > DodgeSpeedMinBuff_)
                {
                    speedValue = groundDirectVelocity - (float)SpeedModule.MoveSpeed_;
                }
                else
                {
                    speedValue = DodgeSpeedMinBuff_;
                }
            }
            CurrentSpeedBuff = SpeedModule.MoveSpeed_.AddModifier_Add(speedValue);
            if (!MovingModule.IsMoving_)
                MovingModule.StartMoving();
            MovingDirModule.CanChangeMovingDirection_ = false;
            FallingChecker.StartFallingEvent += FallAction;
            FallingChecker.StartRisingEvent += RiseAction;
            MovingModule.StopMovingEvent += StopDodging;
        }
        private void StopDodgingAction()
        {
            CurrentSpeedBuff.RemoveModifier();
            MovingDirModule.CanChangeMovingDirection_ = true;
            FallingChecker.StartFallingEvent -= FallAction;
            FallingChecker.StartRisingEvent -= RiseAction;
            MovingModule.StopMovingEvent -= StopDodging;
        }
        private void FallAction(IFallingCheckingModule.FallingStartInfo i)
        {
            StopDodging();
        }
        private void RiseAction(IFallingCheckingModule.GroundFreeRisingInfo i)
        {
            StopDodging();
        }

        private void Awake()
        {
            MovingModule = MovingModuleComponent as IMovingModule;
            if (MovingModule == null)
                throw ServantException.GetNullInitialization("MovingModule");
            MovingDirModule = MovingDirModuleComponent as IMovingDirectionChangingModule;
            if (MovingDirModule == null)
                throw ServantException.GetNullInitialization("MovingDirModule");
            FallingChecker = FallingCheckerComponent as IFallingCheckingModule;
            if (FallingChecker == null)
                throw ServantException.GetNullInitialization("FallingChecker");
            WallChecker = WallCheckerComponent as IWallCheckingModule;
            if (WallChecker == null)
                throw ServantException.GetNullInitialization("WallChecker");
            GroundDirectionCalculator = GroundDirectionCalculatorComponent as IGroundDirectionCalculatingModule;
            if (GroundDirectionCalculator == null)
                throw ServantException.GetNullInitialization("GroundDirectionCalculator");
            SpeedModule = SpeedModuleComponent as ISpeedModule;
            if (SpeedModule == null)
                throw ServantException.GetNullInitialization("SpeedModule");

            DeactivateEvent += () =>
            {
                if (IsDodging_)
                    StopDodging();
            };
        }
        private void FixedUpdate()
        {
            if (CurrentSpeedBuff.Modifier_ <= DodgeSpeedModifierDescentStep_)
            {
                StopDodging();
            }
            else
                CurrentSpeedBuff.UpdateModifier(CurrentSpeedBuff.Modifier_ - DodgeSpeedModifierDescentStep_);
        }
        private void OnEnable()
        {
            if (!IsDodging_)
                enabled = false;
        }
        private void OnDisable()
        {
            if (IsDodging_)
                enabled = true;
        }

        protected abstract float DodgeSpeedMinBuff_ { get; }
        protected abstract float DodgeSpeedModifierDescentStep_ { get; }

        protected sealed override bool CanTurnActivityFromOutside_ => base.CanTurnActivityFromOutside_;
    }
}
