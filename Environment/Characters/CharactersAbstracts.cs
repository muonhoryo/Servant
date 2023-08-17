using MuonhoryoLibrary.Collections;
using MuonhoryoLibrary;
using Servant.InteractionObjects;
using System;
using static Servant.Characters.IGarpoonBase;
using UnityEngine;
using MuonhoryoLibrary.Unity;
using System.Collections.Generic;

namespace Servant.Characters
{
    public interface IFallingGroundChecker
    {
        public event Action<float> UpdateGroundAngleEvent;
        public event Action<IGroundCharacter.LandingInfo> LandingEvent;
        /// <summary>
        /// Argument is true, if has start rising.
        /// </summary>
        public event Action<bool> LostGroundEvent;
        public event Action<IGroundCharacter.GroundFreeRisingInfo> StartGroundFreeRisingEvent;
        public event Action<IGroundCharacter.FallingStartInfo> StartFallingEvent;
        /// <summary>
        /// Argument is true, if has start rising.
        /// </summary>
        public event Action<bool> ChangeVerticalMovingDirection;

        public bool IsUp_ { get; }
        public bool IsOnAGround_ { get; }
        public float GroundAngle_ { get; }
    }
    public interface IClimbableGroundChecker
    {
        public event Action<GameObject, int> FoundClimbableGroundEvent;
        public event Action LostClimbableGroundEvent;
        public GameObject ClimbableGround_ { get; }
        public int ClimbableGroundDirection_ { get; }
        /// <summary>
        /// Return highest point of current climbable ground.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetClimbPosition();

        public bool HasClimbableGroundAround();
    }
    public interface IWallChecker
    {
        public event Action FoundLeftWallEvent;
        public event Action LostLeftWallEvent;
        public event Action FoundRightWallEvent;
        public event Action LostRightWallEvent;
        public bool IsThereLeftWall_ { get; }
        public bool IsThereRightWall_ { get; }
    }
    public interface IGroundCharacter
    {
        public struct LandingInfo
        {
            public LandingInfo(Vector2 LandingForce)
            {
                this.LandingForce = LandingForce;
            }
            public Vector2 LandingForce;
        }
        public struct FallingStartInfo
        {
            public FallingStartInfo(bool wasGroundFreeRising)
            {
                WasGroundFreeRising = wasGroundFreeRising;
            }
            public bool WasGroundFreeRising;
        }
        public struct GroundFreeRisingInfo
        {
            public bool WasFalling;

            public GroundFreeRisingInfo(bool wasFalling)
            {
                WasFalling = wasFalling;
            }
        }

        public enum FallingState
        {
            StandingOnGround,
            GroundFreeRising,
            Falling
        }
        public FallingState CurrentFallingState_ { get; }
        public CompositeParameter MoveSpeed_ { get; }
        public bool CanChangeMovingDirection_ { get; }
        public int MovingDirection_ { get; set; }
        public Vector2 GroundCorrectDirection_ { get; }
        public bool IsMoving_ { get; }
        public bool IsLeftSide_ { get; set; }
        public bool CanStartMoving_ { get; }
        public bool CanStopMoving_ { get; }

        public event Action<GroundFreeRisingInfo> StartGroundFreeRisingEvent;
        public event Action<FallingStartInfo> StartFallingEvent;
        public event Action<int> StartMovingEvent;
        public event Action StopMovingEvent;
        public event Action<int> ChangeMovingDirectionEvent;
        public event Action<Vector2> RecalculateCorrectDirectionEvent;
        /// <summary>
        /// Return true, if left side direction.
        /// </summary>
        public event Action<bool> ChangeHorizontalFiewDirectionEvent;
        /// <summary>
        /// Contains landing speed
        /// </summary>
        public event Action<LandingInfo> LandingEvent;
        public void StartMoving(int movingDirection);
        public void StartMoving();
        public void StopMoving();
    }
    public interface IJumpingCharacter : IGroundCharacter
    {
        public bool CanJump_ { get; }
        public event Action JumpEvent;
        public event Action JumpDelayDoneEvent;
        public event Action JumpDelayStartEvent;
        public event Action JumpHasBeenAccepted;
        public void Jump();
    }
    public interface IDodgingCharacter : IGroundCharacter
    {
        /// <summary>
        /// Return start speed buff.
        /// </summary>
        public event Action<float> StartDodgingEvent;
        public event Action StopDodgingEvent;
        public bool CanDodge_ { get; }
        public bool CanStopDodge_ { get; }
        public bool IsDodging_ { get; }
        public float CurrentDodgingSpeedBuff_ { get; }
        public void StartDodging();
        public void StopDodging();
    }
    public interface IClimbingCharacter : IJumpingCharacter
    {
        public struct ClimbingInfo
        {
            public GameObject ClimbableGround;
            public int ClimbingDirection;

            public ClimbingInfo(GameObject climbableGround, int climbingDirection)
            {
                if (climbableGround == null)
                    throw ServantException.GetNullInitialization("climbableGround");

                ClimbableGround = climbableGround;
                ClimbingDirection = climbingDirection;
            }
        }
        public event Action<ClimbingInfo> StartClimbingEvent;
        public event Action ClimbingDoneEvent;
        public bool CanClimb_ { get; }
        public bool IsClimbing_ { get; }
        public void Climb();
    }
    public interface IInteractingCharacter
    {
        public event Action InteractionEvent;
        public bool CanInteract_ { get; }
        public void Interact();
        /// <summary>
        /// Return true if target was assigned.
        /// </summary>
        /// <param name="interactiveObject"></param>
        /// <returns></returns>
        public bool AssignInteractiveTarget(IInteractiveObject obj);
        /// <summary>
        /// Return false if target was set null.
        /// </summary>
        /// <param name="removedObject"></param>
        /// <returns></returns>
        public bool RemoveInteractTarAssignment(IInteractiveObject obj);
    }
    public interface IGarpoonCharacter : IGarpoonPullableObj, IJumpingCharacter
    {
        public IGarpoonBase GarpoonBase_ { get; }
        public GameObject UnpackedCharacter_ { get; }
        public bool CanStartRocking_ { get; }
        public bool CanStopRocking_ { get; }
        public bool IsRocking_ { get; }
        public bool CanStartRadialRocking_ { get; }
        public bool CanStopRadialRocking_ { get; }
        public bool IsRadialRocking_ { get; }
        public bool CanUseGarpoon_ { get; }

        public event Action StartRopeRockingEvent;
        public event Action StopRopeRockingEvent;
        public event Action<int, float> StartRadialRockingEvent;
        public event Action StopRadialRockingEvent;
        public void StartRopeRocking();
        public void StopRopeRocking();
        public void StartRadialRocking(int movingDirection, float rotationSpeed);
        public void StopRadialRocking();
        public void TurnGarpoon(bool isGarpoonActive);
    }
    public interface ILandRollingCharacter : IGroundCharacter
    {
        public event Action StartLandingRollEvent;
        public event Action LandingRollDoneEvent;
        public bool IsLandRolling_ { get; }
    }
    /// <summary>
    /// Character has any states, which lock his control.
    /// </summary>
    public interface ILockableCharacter
    {
        public event Action UnlockControlEvent;
        public event Action LockControlEvent;
        public bool IsLockedControl_ { get; }
    }
    public interface IAnimLockableCharacter
    {
        public event Action LockingAnimationExitEvent;
        public void LockingAnimationExit();
    }
    public interface IHitPointCharacter
    {
        public event Action DeathEvent;
        public void Death();
    }
    public interface IGarpoonBase
    {
        public interface IGarpoonPullableObj
        {
            public event Action<PullingTargetInfo> StartPullingEvent;
            public event Action StopPullingEvent;
            public struct PullingTargetInfo
            {
                public PullingTargetInfo(Func<Vector2> GetTargetPositionFunc, GameObject Target, float Speed,
                    Vector2 ForceOffset)
                {
                    if (GetTargetPositionFunc == null)
                        throw ServantException.GetArgumentNullException("GetTargetPositionFunc");
                    this.GetTargetPositionFunc = GetTargetPositionFunc;
                    this.Target = Target;
                    this.Speed = Speed;
                    this.ForceOffset = ForceOffset;
                }
                public Func<Vector2> GetTargetPositionFunc;
                public GameObject Target;
                public float Speed;
                public Vector2 ForceOffset;
            }
            public Vector3 Position_ { get; }
            public bool IsPulled_ { get; }
            public IPuller StartPullingToTarget(PullingTargetInfo info);
        }
        public interface IProjectile : IGarpoonPullableObj
        {
            public enum HitType
            {
                none,
                Ground,
                Item
            }

            public event Action MissEvent;
            public event Action<GameObject> HitEvent;
            public event Action DestroyEvent;
            public float Speed_ { get; }
            public Vector2 Direction_ { get; }
            public float PassedDistance_ { get; }
            public GameObject HitObject_ { get; }
            public void Destroy();
            public void Initialize(Transform BaseTransform, Vector2 Direction);
            public void ConnectRockingRopeJoint(DistanceJoint2D rope);
        }
        public interface IRope
        {
            public void Connect(Transform Base);
        }
        public interface IPuller
        {
            public event Action PullDoneEvent;
            public void CancelPull();
        }
        public interface IHittableObj
        {
            public Vector2 HitPosition_ { get; }
        }
        public interface IRadialRockingHookCatcher : IHittableObj
        {
            /// <summary>
            /// -1 - clockwise, 1 - counter-clockwise
            /// </summary>
            public int RockingDirection_ { get; }
            public float RockingSpeed_ { get; }
        }

        public event Action<IProjectile> ShootEvent;
        public event Action CatchOffProjectileEvent;
        public event Action StartPullingEvent;
        public event Action EndPullingEvent;
        public event Action<float> SetRotationEvent;
        public event Action ShowingTheBaseEvent;
        public event Action HidingTheBaseEvent;
        public event Action CancelPullingEvent;

        public IProjectile ShootedProjectile_ { get; }
        public float CurrentRotation_ { get; set; }
        public bool CanBeManualRotated_ { get; }
        public bool CanShoot_ { get;}
        public bool CanCatchHookOff_ { get; }
        public bool CanPulling_ { get; }
        public bool CanCancelPulling_ { get; }
        public bool IsPull_ { get; }
        public void RotateToGlobalPoint(Vector2 point);
        public void ShootProjectile(Vector2 Direction);
        /// <summary>
        /// Shoot projectile to current direction.
        /// </summary>
        public void ShootProjectile();
        public void StartPulling();
        public void CancelPulling();
        public void CatchHookOff();
    }
    public interface IHumanCharacter : IInteractingCharacter, IDodgingCharacter, IClimbingCharacter, ILandRollingCharacter,
        IGarpoonCharacter, IHittableObj, IHitPointCharacter, ILockableCharacter
    { }
    /// <summary>
    /// Default value changed by add's and multiply's modifiers
    /// </summary>
    public sealed class CompositeParameter
    {
        public interface ICharacterConstModifier
        {
            public event Action<float> UpdateValueEvent;
            public void RemoveModifier();
            public float Modifier_ { get; }
            public void UpdateModifier(float newValue);
        }
        private sealed class ModifierHandler : ICharacterConstModifier
        {
            public ModifierHandler(float Modifier, SingleLinkedList<ModifierHandler> list, CompositeParameter owner)
            {
                void RemoveFromListAction()
                {
                    list.Remove(this);
                    owner.RecalculateSpeed();
                }
                RemoveHandlerAction = RemoveFromListAction;
                UpdateValueEvent += (i) => owner.RecalculateSpeed();
                this.Modifier = Modifier;
            }
            private float Modifier;
            public event Action<float> UpdateValueEvent = delegate { };
            private readonly Action RemoveHandlerAction;
            public float Modifier_ => Modifier;
            void ICharacterConstModifier.RemoveModifier() => RemoveHandlerAction();
            public void UpdateModifier(float newValue)
            {
                Modifier = newValue;
                UpdateValueEvent(Modifier);
            }
        }
        private CompositeParameter() { }
        public CompositeParameter(float DefaultValue)
        {
            this.DefaultValue = DefaultValue;
            RecalculateSpeed();
        }
        public readonly float DefaultValue;
        public float CurrentValue_ { get; private set; }
        public event Action<ICharacterConstModifier> AddingAddModifierEvent;
        public event Action<ICharacterConstModifier> AddingMultiplyModifierEvent;
        public event Action<float> ValueHasBeenRecalculatedEvent;
        private readonly SingleLinkedList<ModifierHandler> AddersList = new();
        private readonly SingleLinkedList<ModifierHandler> MultipliesList = new();
        private void RecalculateSpeed()
        {
            CurrentValue_ = DefaultValue;
            foreach (var item in MultipliesList)
            {
                CurrentValue_ *= item.Modifier_;
            }
            foreach (var item in AddersList)
            {
                CurrentValue_ += item.Modifier_;
            }
            ValueHasBeenRecalculatedEvent?.Invoke(CurrentValue_);
        }
        private ICharacterConstModifier AddModifier
            (float modifierValue, SingleLinkedList<ModifierHandler> list,
            Action<ICharacterConstModifier> runningEventAction)
        {
            ModifierHandler modifier = new(modifierValue, AddersList, this);
            list.AddLast(modifier);
            runningEventAction(modifier);
            RecalculateSpeed();
            return modifier;
        }
        public ICharacterConstModifier AddModifier_Add(float speed)
        {
            return AddModifier(speed, AddersList, (item) => AddingAddModifierEvent?.Invoke(item));
        }
        public ICharacterConstModifier AddModifier_Multiply(float speed)
        {
            return AddModifier(speed, MultipliesList, (item) => AddingMultiplyModifierEvent?.Invoke(item));
        }
        public static explicit operator float(CompositeParameter i) => i.CurrentValue_;
    }
    public abstract class GroundMovableCharacter : MonoBehaviour,IGroundCharacter
    {
        [SerializeField]
        private Rigidbody2D Rigidbody;
        [SerializeField]
        private Animator Animator;
        protected Animator Animator_ => Animator;
        protected Rigidbody2D Rigidbody_ => Rigidbody;
        protected IFallingGroundChecker GroundChecker_ { get; private set; }
        protected IWallChecker WallChecker_ { get; private set; }
        //Moving
        public event Action<int> StartMovingEvent=delegate { };
        protected void RunStartMovingEvent(int movingDirection) =>
            StartMovingEvent(movingDirection);
        public event Action<int> ChangeMovingDirectionEvent=delegate { };
        public event Action StopMovingEvent=delegate { };

        public event Action<bool> ChangeHorizontalFiewDirectionEvent=delegate { };
        protected abstract float DefaultMoveSpeed_ { get; }
        public virtual bool CanChangeMovingDirection_ => true;
        private int MovingDirection = 0;
        public int MovingDirection_
        {
            get => MovingDirection;
            set
            {
                if (CanChangeMovingDirection_)
                {
                    if (value == 0)
                        throw new ServantException("MovingDirection cannot be zero.");

                    value = value.Sign();
                    if (MovingDirection != value)
                    {
                        MovingDirection = value;
                        ChangeMovingDirectionEvent(value);
                        IsLeftSide_ = value < 0;
                    }
                }
            }
        }
        public bool IsMoving_ { get; protected set; }
        private bool IsLeftSide = false;
        public bool IsLeftSide_
        {
            get => IsLeftSide;
            set
            {
                if (IsLeftSide != value)
                {
                    IsLeftSide = !IsLeftSide;
                    SetIsLeftSideAction(value);
                    ChangeHorizontalFiewDirectionEvent(IsLeftSide);
                }
            }
        }
        public virtual bool CanStartMoving_ => !IsMoving_&& !this.IsInAir()&& !this.HasWallsAtMovingDirection(WallChecker_);
        public virtual bool CanStopMoving_ => IsMoving_;
        protected abstract void SetIsLeftSideAction(bool value);
        public CompositeParameter MoveSpeed_ { get; private set; }
        protected void RecalculateGroundCorrectDirection(float angle)
        {
            GroundCorrectDirection_ = angle.DirectionOfAngle();
            RecalculateCorrectDirectionEvent(GroundCorrectDirection_);
        }
        protected virtual void InternalStartMoving()
        {
            void MoveAction()
            {
                NoneAcceleratedGroundMoving((float)MoveSpeed_);
            }
            UnityFixedUpdateEvent += MoveAction;
            void StopMovingAction()
            {
                UnityFixedUpdateEvent -= MoveAction;
                StopMovingEvent -= StopMovingAction;
                StartGroundFreeRisingEvent -= GroundFreeRisingAction;
                StartFallingEvent -= FallAction;
                if (!this.IsInAir())
                    Rigidbody.velocity = Vector2.zero;
            }
            void GroundFreeRisingAction(IGroundCharacter.GroundFreeRisingInfo i) => StopMoving();
            void FallAction(IGroundCharacter.FallingStartInfo i) => StopMoving();

            StopMovingEvent += StopMovingAction;
            StartGroundFreeRisingEvent += GroundFreeRisingAction;
            StartFallingEvent += FallAction;
            IsMoving_ = true;
            StartMovingEvent(MovingDirection_);
        }
        protected void InternalStopMoving()
        {
            IsMoving_ = false;
            StopMovingEvent();
        }
        private void FoundWallAction(int direction)
        {
            if (IsMoving_ && MovingDirection_ == direction)
            {
                StopMoving();
            }
        }
        //GroundCorrectDirection
        public event Action<Vector2> RecalculateCorrectDirectionEvent=delegate { };
        public Vector2 GroundCorrectDirection_ { get; private set; } = Vector2.right;

        //Falling
        public event Action<IGroundCharacter.FallingStartInfo> StartFallingEvent=delegate { };
        public event Action<IGroundCharacter.GroundFreeRisingInfo> StartGroundFreeRisingEvent=delegate { };
        public event Action<IGroundCharacter.LandingInfo> LandingEvent=delegate { };
        public IGroundCharacter.FallingState CurrentFallingState_ { get; private set; }
        private void StartGroundFreeRising(IGroundCharacter.GroundFreeRisingInfo info)
        {
            CurrentFallingState_ = IGroundCharacter.FallingState.GroundFreeRising;
            StartGroundFreeRisingEvent(info);
        }
        private void StartFalling(IGroundCharacter.FallingStartInfo info)
        {
            CurrentFallingState_ = IGroundCharacter.FallingState.Falling;
            StartFallingEvent(info);
        }
        private void Land(IGroundCharacter.LandingInfo info)
        {
            CurrentFallingState_ = IGroundCharacter.FallingState.StandingOnGround;
            LandingEvent(info);
        }
        //Physics
        protected float DefaultGravity_ { get; private set; }
        //Control
        public void StartMoving(int movingDirection)
        {
            MovingDirection_ = movingDirection;
            StartMoving();
        }
        public void StartMoving()
        {
            if (CanStartMoving_)
                InternalStartMoving();
        }
        public void StopMoving()
        {
            if (CanStopMoving_)
            {
                InternalStopMoving();
            }
        }
        //UnityAPI
        protected event Action UnityFixedUpdateEvent=delegate { };
        private void FixedUpdate()
        {
            UnityFixedUpdateEvent();
            FixedUpdateAction();
        }
        private void Awake()
        {
            if (Rigidbody == null) Rigidbody = GetComponent<Rigidbody2D>();
            if (Animator == null) Animator = GetComponent<Animator>();
            GroundChecker_ = GetComponentInChildren<IFallingGroundChecker>();
            WallChecker_=GetComponentInChildren<IWallChecker>();

            if (Rigidbody == null) throw ServantException.GetNullInitialization("Rigidbody");
            if (Animator == null) throw ServantException.GetNullInitialization("Animator");
            if (GroundChecker_ == null) throw ServantException.GetNullInitialization("GroundChecker");
            if (WallChecker_ == null) throw ServantException.GetNullInitialization("WallChecker_");

            DefaultGravity_ = Rigidbody.gravityScale;

            MoveSpeed_ = new CompositeParameter(DefaultMoveSpeed_);

            GroundChecker_.LandingEvent += Land;
            GroundChecker_.UpdateGroundAngleEvent += RecalculateGroundCorrectDirection;
            GroundChecker_.StartFallingEvent += StartFalling;
            GroundChecker_.StartGroundFreeRisingEvent += StartGroundFreeRising;
            WallChecker_.FoundRightWallEvent += () => FoundWallAction(1);
            WallChecker_.FoundLeftWallEvent += () => FoundWallAction(-1);

            AwakeAction();

            MovingDirection_ = IsLeftSide ? -1 : 1;

            if (!enabled)
                enabled = true;
        }
        private void OnEnable()
        {
            Rigidbody.isKinematic = false;
            OnEnableAction();
        }
        private void OnDisable()
        {
            Rigidbody.isKinematic = true;
            OnDisableAction();
        }
        protected virtual void AwakeAction() { }
        protected virtual void FixedUpdateAction() { }
        protected virtual void OnEnableAction() { }
        protected virtual void OnDisableAction() { }


        protected void NoneAcceleratedGroundMoving(float speed)
        {
            NoneAcceleratedMoving(speed, GroundCorrectDirection_);
        }
        protected void NoneAcceleratedMoving(float speed, Vector2 direction)
        {
            Rigidbody_.velocity = speed * MovingDirection_ * direction;
        }
        protected void AcceleratedGroundMoving(float speed)
        {
            AcceleratedMoving(speed, GroundCorrectDirection_);
        }
        protected void AcceleratedMoving(float speed, Vector2 direction)
        {
            Rigidbody_.AddForce(speed * direction,ForceMode2D.Force);
        }
        protected void AcceleratedRockingMoving(IGarpoonCharacter owner,float speed)
        {
            Vector2 forceDirection;
            if (transform.position.x > owner.GarpoonBase_.ShootedProjectile_.Position_.x != MovingDirection_ > 0)
            {
                forceDirection = new Vector2(MovingDirection_, 0);
            }
            else
            {
                Vector2 dir = (owner.GarpoonBase_.ShootedProjectile_.Position_ - transform.position).normalized;
                forceDirection = dir.GetRadialForceDirection(MovingDirection_);
            }
            Rigidbody_.AddForce(speed  * forceDirection,ForceMode2D.Force);
            IsLeftSide_ = transform.position.x > owner.GarpoonBase_.ShootedProjectile_.Position_.x;
        }
    }
}
