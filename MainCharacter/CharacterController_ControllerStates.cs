
using System.Collections;
using UnityEngine;

namespace Servant.Control
{
    public sealed partial class MainCharacterController
    {
        static MainCharacterController Controller => Registry.CharacterController;
        //UpdateAction
        private static void WalkStayUpdateAction()
        { WalkStayAction();JumpAction(); WalkStayChangeRunModeAction();
            Interaction();}
        private static void WalkMoveUpdateAction()
        { WalkMoveAction();JumpAction();WalkChangeRunModeAction();
            Interaction();}
        private static void RunStayUpdateAction()
        { RunStayAction();JumpAction();RunStayChangeRunModeAction();
            Interaction();}
        private static void RunMoveUpdateAction()
        { RunMoveAction();JumpAction();RunChangeRunModeAction();
            Interaction();}
        //Interaction
        private static void Interaction()
        {
            if(Input.GetButtonDown("Interaction"))
            {
                Controller.Interact();
            }
        }
        //JumpAction
        private static void JumpAction()
        {
        	if(Input.GetButtonDown("Jump"))
        	{
        		Controller.Jump();
                Controller.ChangeState(JumpState);
        	}
        }
        //MoveAction
        private static void SetCharacterDirection(float direction)
        {
            Controller.IsLeftSide_ = direction < 0;
        }
        private static float GetHorizontalDirection() => Input.GetAxisRaw("Horizontal");
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
                Controller.ChangeState(stayState);
                Controller.StopMove();
            }
            else
            {
                MoveAction(direction,speedConst);
            }
        }
        private static void WalkMoveAction()=>
            MoveWithStayTransitAction(Registry.MainCharacterWalkSpeed, WalkStayState);
        private static void RunMoveAction()=>
            MoveWithStayTransitAction(Registry.MainCharacterRunSpeed, RunStayState);
        private static void FallMoveAction()
        {
            float direction = GetHorizontalDirection();
            if (direction != 0)
            {
                SetCharacterDirection(direction);
                Controller.AccelMove(Registry.MainCharacterJumpMoveSpeed*direction);
            }
        }
        //MoveAction(stay)
        private static void StayAction(float direction,ControllerState moveState)
        {
            if (direction != 0)
            {
                Controller.ChangeState(moveState);
                Controller.IsMove_ = true;
            }
        }
        private static void StayAction(ControllerState moveState)
        {
            StayAction(GetHorizontalDirection(), moveState);
        }
        private static void WalkStayAction() => StayAction( WalkState);
        private static void RunStayAction() => StayAction( RunState);
        //LandAction
        private static void FallLandAction()
        {
            if (Controller.IsRun_)
            {
                Controller.ChangeState(Controller.IsMove_ ? RunState : RunStayState);
            }
            else
            {
                Controller.ChangeState(Controller.IsMove_ ? WalkState : WalkStayState);
            }
            Controller.StopMove();
        }
        //FallAction
        private static void FallAction()
        {
            Controller.ChangeState(FallState);
        }
        //EnterStateAction
        private static void EnterWalkStayState()
        {
            Controller.IsMove_ = false;
            Controller.IsRun_ = false;
        }
        private static void EnterWalkState()
        {
            Controller.IsMove_ = true;
            Controller.IsRun_ = false;
        }
        private static void EnterRunStayState()
        {
            Controller.IsMove_ = false;
            Controller.IsRun_ = true;
        }
        private static void EnterRunState()
        {
            Controller.IsMove_ = true;
            Controller.IsRun_ = true;
        }
        private static IEnumerator JumpStateTransitDelay()
        {
            yield return new WaitForSeconds(Registry.JumpStateTransitDelay);
            if (!Controller.IsRun_) Controller.ChangeState(Controller.IsMove_ ? WalkState : WalkStayState);
            else Controller.ChangeState(Controller.IsMove_ ? RunState : RunStayState);
            yield break;
        }
        private static void EnterJumpStateAction()
        {
            (Controller.CurrentState as TempleControllerState).DelayCoroutine=
                Controller.StartCoroutine(JumpStateTransitDelay());
        }
        //ExitStateAction
        private static void ExitJumpStateAction()
        {
            Controller.StopCoroutine((Controller.CurrentState as TempleControllerState).DelayCoroutine);
        }
        //ChangeRunMoveAction
        private static void ChangeRunModeAction(ControllerState changedState)
        {
            if (Input.GetButtonDown("ChangeRunMode"))
            {
                Controller.IsRun_ = !Controller.IsRun_;
                Controller.ChangeState(changedState);
            }
        }
        private static void WalkStayChangeRunModeAction() => ChangeRunModeAction(RunStayState);
        private static void WalkChangeRunModeAction() => ChangeRunModeAction(RunState);
        private static void RunStayChangeRunModeAction() => ChangeRunModeAction(WalkStayState);
        private static void RunChangeRunModeAction()=> ChangeRunModeAction(WalkState);

        private static ControllerState WalkStayState =new ControllerState
            (StateName.WalkStayState,
             UpdateAction: WalkStayUpdateAction,
             LandAction: Registry.EmptyMethod,
             FallAction: FallAction,
             EnterStateAction: EnterWalkStayState,
             ExitStateAction: Registry.EmptyMethod);
        private static readonly ControllerState WalkState =new ControllerState
            (StateName.WalkState,
             UpdateAction: WalkMoveUpdateAction,
             LandAction: Registry.EmptyMethod,
             FallAction: FallAction,
             EnterStateAction:EnterWalkState,
             ExitStateAction: Registry.EmptyMethod);
        private static readonly ControllerState RunStayState =new ControllerState
            (StateName.RunStayState,
             UpdateAction: RunStayUpdateAction,
             LandAction: Registry.EmptyMethod,
             FallAction: FallAction,
             EnterStateAction: EnterRunStayState,
             ExitStateAction: Registry.EmptyMethod);
        private static readonly ControllerState RunState =new ControllerState
            (StateName.RunState,
             UpdateAction: RunMoveUpdateAction,
             LandAction: Registry.EmptyMethod,
             FallAction: FallAction,
             EnterStateAction: EnterRunState,
             ExitStateAction: Registry.EmptyMethod);
        private static readonly ControllerState FallState = new ControllerState
            (StateName.FallState,
             UpdateAction: FallMoveAction,
             LandAction: FallLandAction,
             FallAction: Registry.EmptyMethod,
             EnterStateAction: Registry.EmptyMethod,
             ExitStateAction: Registry.EmptyMethod);
        private static readonly ControllerState JumpState = new TempleControllerState
            (StateName.JumpState,
            updateAction: FallMoveAction,
            landAction: Registry.EmptyMethod,
            fallAction: FallAction,
            enterStateAction: EnterJumpStateAction,
             exitStateAction: ExitJumpStateAction);
        public enum StateName
        {
            WalkStayState,
            WalkState,
            RunStayState,
            RunState,
            FallState,
            JumpState
        }
    }
}
