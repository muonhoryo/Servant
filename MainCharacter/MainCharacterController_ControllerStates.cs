
using System;
using System.Collections;
using UnityEngine;

namespace Servant.Control
{
    public sealed partial class MainCharacterController
    {
        static MainCharacterController Controller => Registry.CharacterController;
        private static readonly Action EmptyAction=()=> { };
        //InputsNames
        private const string Input_Interaction = "Interaction";
        private const string Input_Jump = "Jump";
        private const string Input_Horizontal = "Horizontal";
        private const string Input_Shoot = "Shoot";
        private const string Input_GarpoonPull = "Pull";
        //UpdateAction
        private static void WalkStayUpdateAction()
        { WalkStayCheck(); JumpAction(); Interaction();}

        private static void WalkMoveUpdateAction()
        { WalkMoveAction(); JumpAction(); Interaction(); }
        private static void FallUpdateAction()
        { FallMoveAction(); Interaction(); }
        private static void RockingUpdateAction()
        { RockingMoveAction();Interaction(); }
        private static void PullUpdateAction()
        {
            Interaction(); 
        }
        //Interaction
        private static void Interaction()
        {
            if(Input.GetButtonDown(Input_Interaction))
            {
                Controller.Interact();
            }
        }
        //JumpAction
        private static void JumpAction()
        {
        	if(Input.GetButtonDown(Input_Jump))
        	{
        		Controller.Jump();
                Controller.ChangeControllerState(JumpState);
        	}
        }
        //MoveAction
        private static void SetCharacterDirection(float direction)
        {
            Controller.IsLeftSide_ = direction < 0;
        }
        private static float GetHorizontalDirection() => Input.GetAxisRaw(Input_Horizontal);
        private static void MoveAction(float direction, float speedConst)
        {
            SetCharacterDirection(direction);
            Controller.SmoothMove(direction * speedConst);
        }
        private static void MoveWithStayTransitAction( float speedConst,ControllerState stayState)
        {
            float direction = GetHorizontalDirection();
            if(direction == 0)
            {
                Controller.ChangeControllerState(stayState);
                Controller.StopMove();
            }
            else
            {
                MoveAction(direction,speedConst);
            }
        }
        private static void WalkMoveAction()=>
            MoveWithStayTransitAction(Registry.MainCharacterWalkSpeed, WalkStayState);
        private static void FallMoveAction()
        {
            float direction = GetHorizontalDirection();
            if (direction != 0)
            {
                SetCharacterDirection(direction);
                Controller.AccelMove(Registry.MainCharacterJumpMoveSpeed*direction);
            }
        }
        private static void RockingMoveAction()
        {
            float direction = GetHorizontalDirection();
            if (direction != 0)
            {
                Vector2 force = Vector2.Perpendicular
                    (Vector3.Normalize
                        (Controller.Projectile.transform.position - Controller.transform.position)) * -direction;
                Controller.AddAccelForce(force, Registry.GarpoonRockingMoveSpeed);
                SetCharacterDirection(direction);
            }
            else
            {
                Controller.IsLeftSide_ = Controller.transform.position.x > Controller.Projectile.transform.position.x;
            }
        }
        //MoveAction(stay)
        private static void WalkStayCheck()
        {
            if (GetHorizontalDirection() != 0)
            {
                Controller.ChangeControllerState(WalkState);
                Controller.IsMove_ = true;
            }
        }
        //LandAction
        private static void FallLandAction()
        {
            Controller.ChangeControllerState(Controller.IsMove_ ? WalkState : WalkStayState);
            Controller.StopMove();
        }
        //FallAction
        private static void FallAction()
        {
            Controller.ChangeControllerState(FallState);
        }
        //EnterStateAction
        private static void EnterWalkStayState()
        {
            Controller.IsMove_ = false;
        }
        private static void EnterWalkState()
        {
            Controller.IsMove_ = true;
        }
        private static IEnumerator JumpStateTransitDelay()
        {
            yield return new WaitForSeconds(Registry.JumpStateTransitDelay);
            Controller.ChangeControllerState(Controller.IsMove_ ? WalkState : WalkStayState);
            yield break;
        }
        private static void EnterJumpStateAction()
        {
            JumpState.DelayCoroutine=Controller.StartCoroutine(JumpStateTransitDelay());
            Controller.SetJumpAnimation(true);
        }
        private static void EnterRockingStateAction()
        {
            RockingState.RopeJoint = Controller.gameObject.AddComponent<DistanceJoint2D>();
            RockingState.RopeJoint.connectedBody = Controller.Projectile.GetComponent<Rigidbody2D>();
        }
        private static void EnterPullStateAction()
        {
            Controller.ResetVelocity();
            Controller.TurnGravity(false);
            Controller.IsFall_ = true;
        }
        //ExitStateAction
        private static void ExitJumpStateAction()
        {
            Controller.StopCoroutine(JumpState.DelayCoroutine);
            Controller.SetJumpAnimation(false);
        }
        private static void ExitRockingStateAction()
        {
            Destroy(RockingState.RopeJoint);
        }
        private static void ExitPullStateAction()
        {
            Controller.TurnGravity(true);
        }

        private static readonly ControllerState WalkStayState =new ControllerState
            (StateName.WalkStayState,
             UpdateAction: WalkStayUpdateAction,
             LandAction: EmptyAction,
             FallAction: FallAction,
             EnterStateAction: EnterWalkStayState,
             ExitStateAction: EmptyAction);
        private static readonly ControllerState WalkState =new ControllerState
            (StateName.WalkState,
             UpdateAction: WalkMoveUpdateAction,
             LandAction: EmptyAction,
             FallAction: FallAction,
             EnterStateAction:EnterWalkState,
             ExitStateAction: EmptyAction);
        private static readonly ControllerState FallState = new ControllerState
            (StateName.FallState,
             UpdateAction: FallUpdateAction,
             LandAction: FallLandAction,
             FallAction: EmptyAction,
             EnterStateAction: EmptyAction,
             ExitStateAction: EmptyAction);
        private static readonly TempleControllerState JumpState = new TempleControllerState
            (StateName.JumpState,
            updateAction: FallUpdateAction,
            landAction: EmptyAction,
            fallAction: FallAction,
            enterStateAction: EnterJumpStateAction,
             exitStateAction: ExitJumpStateAction);
        private static readonly RockingControllerState RockingState = new RockingControllerState
            (StateName.RockingState,
             updateAction: RockingUpdateAction,
             landAction: FallLandAction,
             fallAction: EmptyAction,
             enterStateAction: EnterRockingStateAction,
             exitStateAction: ExitRockingStateAction);
        private static readonly ControllerState PullState =new ControllerState
            (StateName.PullState,
             UpdateAction: PullUpdateAction,
             LandAction: EmptyAction,
             FallAction: EmptyAction,
             EnterStateAction: EnterPullStateAction,
             ExitStateAction: ExitPullStateAction);
        private static readonly ControllerState NoneState = new ControllerState
                (StateName.NoneState,
                UpdateAction: EmptyAction,
                LandAction: EmptyAction,
                FallAction: EmptyAction,
                EnterStateAction: EmptyAction,
                ExitStateAction: EmptyAction);
        public enum StateName
        {
            WalkStayState,
            WalkState,
            FallState,
            JumpState,
            RockingState,
            PullState,
            NoneState,
            Garpoon_ReadyState,
            Garpoon_ShootState,
            Garpoon_HookState,
            Garpoon_PullState
        }
    }
}
