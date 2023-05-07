
using System;
using UnityEngine;
using Servant.InteractionObjects;
using Servant.Serialization;

namespace Servant.Control
{
    public sealed partial class MainCharacterController :MonoBehaviour
    {
        private const string MoveBooleanName = "IsMove";
        private const string FallBooleanName = "Fall";
        private const string IsGarpoonAnimName = "IsGarpoon";
        private const string IsJumpAnimName = "IsJump";
        private void SetDuoEventedBoolean(ref bool booleanField, string animValueName,
        Action trueEvent, Action falseEvent, bool inputValue)
        {
            if (booleanField != inputValue)
            {
                booleanField = !booleanField;
                animator.SetBool(animValueName, booleanField);
                if (booleanField)
                {
                    trueEvent();
                }
                else
                {
                    falseEvent();
                }
            }
        }
        private new Rigidbody2D rigidbody;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private GroundChecker groundChecker;
        //StateMachine
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
        private class GarpoonPullControllerState : ControllerState
        {
            public GarpoonPullControllerState(StateName stateName, Action updateAction, Action landAction,
                Action fallAction, Action enterStateAction, Action exitStateAction) :
                base(stateName, updateAction, landAction, fallAction, enterStateAction, exitStateAction)
            { }
            public GarpoonPull Puller;
        }
        private ControllerState CurrentControllerState = WalkStayState;
        private ControllerState CurrentGarpoonState =GarpoonStates.ReadyState;
        private void ChangeControllerState(ControllerState changedState)
        {
            CurrentControllerState.ExitStateAction();
            CurrentControllerState = changedState;
            CurrentControllerState.EnterStateAction();
            ChangeControllerStateEvent?.Invoke();
        }
        private void ChangeGarpoonState(ControllerState changedState)
        {
            CurrentGarpoonState.ExitStateAction();
            CurrentGarpoonState = changedState;
            CurrentGarpoonState.EnterStateAction();
            ChangeGarpoonControllerStateEvent?.Invoke();
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
        public event Action StartMovingEvent;
        public event Action StopMovingEvent;
        public event Action SetRunModeEvent;
        public event Action SetWalkModeEvent;
        public event Action ChangeFiewDirectionEvent;
        public event Action JumpEvent;
        public event Action FallingEvent;
        public event Action LandingEvent;
        public event Action ChangeControllerStateEvent;
        public event Action InteractionEvent;
        public event Action ChangeGarpoonControllerStateEvent;
        //Moving
        private Vector2 MoveDirection = Vector2.right;
        private bool isMove = false;
        public bool IsMove_
        {
            get => isMove;
            private set
            {
                SetDuoEventedBoolean(ref isMove, MoveBooleanName,
                () => StartMovingEvent?.Invoke(),
                () => StopMovingEvent?.Invoke(), value);
            }
        }
        private bool isLeftSide = false;
        public bool IsLeftSide_
        {
            get => isLeftSide;
            private set
            {
                if (isLeftSide != value)
                {
                    isLeftSide = !isLeftSide;
                    spriteRenderer.flipX = isLeftSide;
                    ChangeFiewDirectionEvent?.Invoke();
                }
            }
        }
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
            JumpEvent?.Invoke();
        }
        private void StopMove()
        {
            rigidbody.velocity = Vector2.zero;
        }
        //Falling
        private bool isFall = false;
        private bool IsFall_
        {
            get => isFall;
            set
            {
                SetDuoEventedBoolean(ref isFall, FallBooleanName,
                () => FallingEvent?.Invoke(),
                () => LandingEvent?.Invoke(), value);
            }
        }
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
        private IInteractiveObject InteractiveTarget;
        private void Interact()
        {
            if (InteractiveTarget != null)
            {
                InteractiveTarget.Interact();
                InteractionEvent?.Invoke();
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
        private void OnEnable()
        {
            rigidbody.isKinematic = false;
        }
        private void OnDisable()
        {
            rigidbody.isKinematic = true;
        }
    }
}
