using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    public abstract class CharacterMovingModule:Module,IMovingModule,
        IModuleChangingScript<IMovingDirectionChangingModule>,
        IModuleChangingScript<ISpeedModule>
    {
        public event Action<int> StartMovingEvent = delegate { };
        public event Action StopMovingEvent = delegate { };
        public float MoveSpeed_ => SpeedModule_.MoveSpeed_.CurrentValue_;
        public bool IsMoving_ { get; private set; } = false;
        public bool CanStartMoving_
        {
            get => CanStartMoving && !IsMoving_ && CanStartMoving_AdditionalConditions;
            set => CanStartMoving = value;
        }
        protected virtual bool CanStartMoving_AdditionalConditions { get=>true; }
        public bool CanStopMoving_
        {
            get => CanStopMoving && IsMoving_;
            set => CanStopMoving = value;
        }


        protected event Action ChangeMovingDirModuleEvent = delegate { };
        protected event Action ChangeSpeedModuleEvent = delegate { };
        IMovingDirectionChangingModule IModuleChangingScript<IMovingDirectionChangingModule>.Module__
            { get => MovingDirModule_;set { MovingDirModule_ = value; ChangeMovingDirModuleEvent(); } }
        ISpeedModule IModuleChangingScript<ISpeedModule>.Module__
            { get => SpeedModule_; set { SpeedModule_ = value; ChangeSpeedModuleEvent(); } }

        [SerializeField]
        private Component MovingDirModuleComponent;
        [SerializeField]
        private Component SpeedModuleComponent;
        [SerializeField]
        private Rigidbody2D Rigidbody;

        protected IMovingDirectionChangingModule MovingDirModule_ { get; private set; }
        protected ISpeedModule SpeedModule_ { get; private set; }
        protected Rigidbody2D Rigidbody_ => Rigidbody;
        private bool CanStartMoving = true;
        private bool CanStopMoving = true;

        public void StartMoving()
        {
            if (CanStartMoving_)
            {
                InternalStartMoving();
            }
        }
        public void StopMoving()
        {
            if (CanStopMoving_)
            {
                InternalStopMoving();
            }
        }
        private void InternalStartMoving()
        {
            IsMoving_ = true;
            enabled = true;
        }
        private void InternalStopMoving()
        {
            IsMoving_ = false;
            enabled = false;
        }

        private void Awake()
        {
            MovingDirModule_ = MovingDirModuleComponent as IMovingDirectionChangingModule;
            if (MovingDirModule_ == null)
                throw ServantException.GetNullInitialization("MovingDirModule_");
            SpeedModule_ = SpeedModuleComponent as ISpeedModule;
            if (SpeedModule_ == null)
                throw ServantException.GetNullInitialization("SpeedModule");
            if (Rigidbody == null)
                if (!TryGetComponent(out Rigidbody))
                    throw ServantException.GetNullInitialization("Rigidbody");

            DeactivateEvent += StopMoving;

            AwakeAction();
        }
        protected virtual void AwakeAction() { }
        private void FixedUpdate()
        {
            MovingAction(GetMovingDirection(), MovingDirModule_.MovingDirection_, SpeedModule_.MoveSpeed_.CurrentValue_,
                MovingSpeedModifier_);
        }
        protected abstract void MovingAction(Vector2 direction,int horizontalDirection, float speed,float speedModifier);
        protected abstract Vector2 GetMovingDirection();
        protected abstract float MovingSpeedModifier_ { get; }
        private void OnEnable()
        {
            if (IsMoving_)
                StartMovingEvent(MovingDirModule_.MovingDirection_);
            else
                enabled = false;
        }
        private void OnDisable()
        {
            if (!IsMoving_)
                StopMovingEvent();
            else
                enabled = true;
        }

        protected sealed override bool CanTurnActivityFromOutside_ => base.CanTurnActivityFromOutside_;
    }
}
