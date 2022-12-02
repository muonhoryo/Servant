
using System;
using UnityEngine;
using MuonhoryoLibrary;
using Servant.Serialization;

namespace Servant.Control
{
    public sealed partial class MainCharacterController : SerializedObject,
        ISingltone<MainCharacterController>
    {
        private const string MoveBooleanName = "IsMove";
        private const string FallBooleanName = "Fall";
        private const string IsGarpoonAnimName = "IsGarpoon";
        private const string IsJumpAnimName = "IsJump";
        private bool isLeftSide = false;
        public bool IsLeftSide_
        {
            get=> isLeftSide;
            private set
            {
                if (isLeftSide != value)
                {
                    isLeftSide = !isLeftSide;
                    spriteRenderer.flipX = isLeftSide;
                    ChangeFiewDirectionEvent();
                }
            }
        }
        private void SetDuoEventedBoolean(ref bool boolean, string animValueName,
        Action trueEvent, Action falseEvent, bool inputValue)
        {
            if (boolean != inputValue)
            {
                boolean = !boolean;
                animator.SetBool(animValueName, boolean);
                if (boolean)
                {
                    trueEvent();
                }
                else
                {
                    falseEvent();
                }
            }
        }
        private bool isFall = false;
        private bool IsFall_
        {
            get => isFall;
            set
            {
                SetDuoEventedBoolean(ref isFall, FallBooleanName,
                () => FallingEvent(),
                () => LandingEvent(), value);
            }
        }
        private bool isMove = false;
        public bool IsMove_
        {
            get => isMove;
            private set
            {
                SetDuoEventedBoolean(ref isMove, MoveBooleanName,
                () => StartMovingEvent(),
                () => StopMovingEvent(), value);
            }
        }
        private new Rigidbody2D rigidbody;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private GroundChecker groundChecker;
        private Vector2 MoveDirection = Vector2.right;
        private IInteractiveObject InteractiveTarget;
        MainCharacterController ISingltone<MainCharacterController>.Singltone
        {
            get => Registry.CharacterController;
            set => Registry.CharacterController = value;
        }
        private class ControllerState
        {
            public ControllerState(StateName StateName,Action UpdateAction, Action LandAction,Action FallAction,
                Action EnterStateAction,Action ExitStateAction)
            {
                this.StateName = StateName;
                this.UpdateAction = UpdateAction;
                this.LandAction = LandAction;
                this.FallAction = FallAction;
                this.EnterStateAction = EnterStateAction;
                this.ExitStateAction = ExitStateAction;
            }
            public readonly StateName StateName;
            public readonly Action UpdateAction;
            public readonly Action LandAction;
            public readonly Action FallAction;
            public readonly Action EnterStateAction;
            public readonly Action ExitStateAction;
        }
        private class TempleControllerState : ControllerState
        {
            public TempleControllerState(StateName stateName, Action updateAction, Action landAction,
                Action fallAction, Action enterStateAction, Action exitStateAction) :
                base(stateName, updateAction, landAction, fallAction, enterStateAction, exitStateAction)
            { }
            public Coroutine DelayCoroutine;
        }
        private class RockingControllerState : ControllerState
        {
            public RockingControllerState(StateName stateName, Action updateAction, Action landAction,
                Action fallAction, Action enterStateAction, Action exitStateAction) :
                base(stateName, updateAction, landAction, fallAction, enterStateAction, exitStateAction)
            { }
            public DistanceJoint2D RopeJoint;
        }
        private ControllerState CurrentControllerState = WalkStayState;
        private ControllerState CurrentGarpoonState =GarpoonStates.ReadyState;
        private void ChangeControllerState(ControllerState changedState)
        {
            CurrentControllerState.ExitStateAction();
            CurrentControllerState = changedState;
            CurrentControllerState.EnterStateAction();
            ChangeControllerStateEvent();
        }
        private void ChangeGarpoonState(ControllerState changedState)
        {
            CurrentGarpoonState.ExitStateAction();
            CurrentGarpoonState = changedState;
            CurrentGarpoonState.EnterStateAction();
            ChangeGarpoonControllerStateEvent();
        }
        private void ResetControllerState()
        {
            ChangeControllerState(isFall ? FallState : WalkStayState);
        }
        public StateName GetCurrentStateName()=>CurrentControllerState.StateName;
        public void SetGarpoonAnimation(bool isGarpoon)
        {
            animator.SetBool(IsGarpoonAnimName, isGarpoon);
        }
        public void SetJumpAnimation(bool isJump)
        {
            animator.SetBool(IsJumpAnimName, isJump);
        }
        //Events
        public event Action StartMovingEvent = Registry.EmptyMethod;
        public event Action StopMovingEvent= Registry.EmptyMethod;
        public event Action SetRunModeEvent = Registry.EmptyMethod;
        public event Action SetWalkModeEvent = Registry.EmptyMethod;
        public event Action ChangeFiewDirectionEvent = Registry.EmptyMethod;
        public event Action JumpEvent = Registry.EmptyMethod;
        public event Action FallingEvent = Registry.EmptyMethod;
        public event Action LandingEvent = Registry.EmptyMethod;
        public event Action ChangeControllerStateEvent = Registry.EmptyMethod;
        public event Action InteractionEvent = Registry.EmptyMethod;
        public event Action ChangeGarpoonControllerStateEvent = Registry.EmptyMethod;
        //Moving
        private void SmoothMove(float speed)
        {
            rigidbody.velocity =  MoveDirection * speed / Time.deltaTime;
        }
        private void AddAccelForce(Vector2 forceDirection,float speed)
        {
        	rigidbody.AddForce(forceDirection * Registry.AccelMoveModifier * speed / Time.deltaTime,
                ForceMode2D.Impulse);
        }
        private void AccelMove(float speed)
        {
            AddAccelForce(MoveDirection,speed);
        }
        private void Jump()
        {
            rigidbody.AddForce( Vector2.up * Registry.MainCharacterJumpForce, ForceMode2D.Impulse);
            JumpEvent();
        }
        private void StopMove()
        {
            rigidbody.velocity = Vector2.zero;
        }
        //Falling
        public void StartFalling() => IsFall_ = true;
        //Physics
        float DefaultGravity;
        public void TurnGravity(bool gravityMode)
        {
            rigidbody.gravityScale = gravityMode ? DefaultGravity : 0;
        }
        public void ResetVelocity()
        {
            rigidbody.velocity = Vector2.zero;
        }
        //Interaction
        private void Interact()
        {
            if (InteractiveTarget != null)
            {
                InteractiveTarget.Interact();
                InteractionEvent();
            }
        }
        /// <summary>
        /// Return true if target was assigned.
        /// </summary>
        /// <param name="interactiveObject"></param>
        /// <returns></returns>
        public bool AssignInteractiveTarget(IInteractiveObject interactiveObject)
        {
            if (InteractiveTarget == null&&
                interactiveObject!=null)
            {
                interactiveObject.Show();
                InteractiveTarget = interactiveObject;
                return true;
            }
            else return false;
        }
        /// <summary>
        /// Return false if target was set null.
        /// </summary>
        /// <param name="removedObject"></param>
        /// <returns></returns>
        public bool RemoveInteractTarAssignment(IInteractiveObject removedObject)
        {
            if (InteractiveTarget!=null&&
                InteractiveTarget == removedObject)
            {
                InteractiveTarget.Hide();
                InteractiveTarget = null;
                return true;
            }
            else return false;
        } 
        //Unity API
        private void Update()
        {
            CurrentControllerState.UpdateAction();
            CurrentGarpoonState.UpdateAction();
        }
        private void Awake()
        {
            if (Registry.CharacterController != null &&
                Registry.CharacterController != this)
            {
                Destroy(this);
                throw ServantException.GetSingltoneException("CharacterController");
            }
            else
            {
                Registry.CharacterController = this;
                if (rigidbody == null) rigidbody = GetComponent<Rigidbody2D>();
                if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
                if (animator == null) animator = GetComponent<Animator>();
                if (groundChecker == null) groundChecker = GetComponentInChildren<GroundChecker>();
            }
            if (rigidbody==null)throw ServantException.NullInitialization("rigidbody");
            if(spriteRenderer==null) throw ServantException.NullInitialization("spriteRenderer");
            if (animator == null) throw ServantException.NullInitialization("animator");
            if (groundChecker == null) throw ServantException.NullInitialization("groundChecker");
            FallingEvent +=()=>CurrentControllerState.FallAction();
            LandingEvent += () => CurrentControllerState.LandAction();
            FallingEvent += () => CurrentGarpoonState.FallAction();
            LandingEvent += () => CurrentGarpoonState.LandAction();
            DefaultGravity = rigidbody.gravityScale;
        }
        private void Start()
        {
            ResetControllerState();
        }
        private void OnDestroy()
        {
            Registry.CharacterController = null;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsFall_&&!groundChecker.IsFreeStanding()) IsFall_ = false;
        }
        //Serialization 
        /*
         * Position,
         * IsLeftSide
         */
        public override string Serialize()
        {
            return this.GetSerializedId() + LocationSerializationData.SeparateSym +
                transform.position.SerializeVector2() + LocationSerializationData.SeparateSym +
                IsLeftSide_ + LocationSerializationData.SeparateSym;
        }
        public override void Deserialize(string serializedObject, int dataStart)
        {
            transform.position =LocationSerializationData.DeserializeVector2
                (LocationSerializationData.GetSubData(serializedObject, dataStart,out dataStart, 2));
            if (!bool.TryParse(LocationSerializationData.GetSubData(serializedObject, dataStart + 1,
                out dataStart), out bool isLeft)) throw ServantException.SerializationException();
            else
            {
                IsLeftSide_ = isLeft;
            }
        }
        public override void OnStartLocationLoad()
        {
            rigidbody.isKinematic = true;
        }
        public override void OnEndLocationLoad()
        {
            rigidbody.isKinematic = false;
        }
    }
}
