using Servant.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Control
{
    public sealed partial class CharacterController
    {
        private ControllerState CurrentMainState=MainStates.NoneState;
        private void ChangeControllerState(ControllerState newState)
        {
            CurrentMainState.RunExitAction(this);
            CurrentMainState= newState;
            CurrentMainState.RunEnterAction(this);
        }
        private void ResetControllerMainState()
        {
            ControllerState state;
            if (CtrlChar.IsLockedControl_)
                state = MainStates.NoneState;
            else if (CtrlChar.IsInAir())
                state = MainStates.AirState;
            else if (CtrlChar.IsDodging_)
                state = MainStates.DodgeState;
            else
                state= MainStates.GroundState;
            ChangeControllerState(state);
        }
        private class ControllerState
        {
            private ControllerState() { }
            public ControllerState(string StateName,Action<CharacterController> UpdateAction,
                Action<CharacterController> EnterAction,
                Action<CharacterController> ExitAction)
            {
                this.StateName = StateName;
                this.UpdateAction = UpdateAction??delegate { };
                this.EnterAction = EnterAction??delegate { };
                this.ExitAction = ExitAction ?? delegate { };
            }

            public readonly string StateName;
            public virtual void RunUpdateAction(CharacterController owner) =>
                UpdateAction(owner);
            public virtual void RunEnterAction(CharacterController owner) =>
                EnterAction(owner);
            public virtual void RunExitAction(CharacterController owner) =>
                ExitAction(owner);
            private readonly Action<CharacterController> UpdateAction;
            private readonly Action<CharacterController> EnterAction;
            private readonly Action<CharacterController> ExitAction;
        }
        private static class MainStates
        {
            //Update
            private static void Ground_UpdateAction(CharacterController owner)
            {
                MovingAction();

                InteractionAction();

                if (Input.GetButtonDown(Input_Jump)) CtrlChar.Jump();
                else if (Input.GetButton(Input_Dodge)) CtrlChar.StartDodging();
                else if (Input.GetButton(Input_Climb)) CtrlChar.Climb();
            }
            private static void Air_UpdateAction(CharacterController owner)
            {
                MovingAction();

                InteractionAction();

                if (Input.GetButtonDown(Input_Climb)) CtrlChar.Climb();
            }
            private static void Dodge_UpdateAction(CharacterController owner)
            {
                if (!Input.GetButton(Input_Dodge)) CtrlChar.StopDodging();
                else if (Input.GetButtonDown(Input_Jump)) CtrlChar.Jump();

                InteractionAction();
            }
            //Other
            private static void MovingAction()
            {
                float dir = GetHorizontalDirection();
                if (dir != 0)
                {
                    ChangeMovingDirection(dir);
                    CtrlChar.StartMoving();
                }
                else
                {
                    if (!Input.GetButton(Input_Dodge))
                        CtrlChar.StopMoving();
                }
            }
            private static void InteractionAction()
            {
                if (Input.GetButtonDown(Input_Interaction)) CtrlChar.Interact();
            }

            public static readonly ControllerState GroundState = new ControllerState(
                StateName: "Ground",
                UpdateAction: Ground_UpdateAction,
                EnterAction: null,
                ExitAction: null);
            public static readonly ControllerState AirState = new ControllerState(
                StateName: "Air",
                UpdateAction: Air_UpdateAction,
                EnterAction: null,
                ExitAction: null);
            public static readonly ControllerState DodgeState = new ControllerState(
                StateName: "Dodge",
                UpdateAction: Dodge_UpdateAction,
                EnterAction: null,
                ExitAction: null);
            public static readonly ControllerState NoneState = new ControllerState(
                StateName: "None",
                UpdateAction: null,
                EnterAction: null,
                ExitAction: null);

            public static void AwakeAction(CharacterController owner)
            {
                CtrlChar.StartDodgingEvent += (i) => owner.ChangeControllerState(DodgeState);

                CtrlChar.StartFallingEvent += (i) => owner.ChangeControllerState(AirState);
                CtrlChar.StartGroundFreeRisingEvent += (i) => owner.ChangeControllerState(AirState);

                CtrlChar.LandingEvent += (i) => owner.ChangeControllerState(GroundState);
                CtrlChar.StopDodgingEvent += () =>
                {
                    if (!CtrlChar.IsInAir()) owner.ChangeControllerState(GroundState);
                };

                CtrlChar.LockControlEvent += () => owner.ChangeControllerState(NoneState);

                CtrlChar.UnlockControlEvent += () => owner.ResetControllerMainState();
            }
        }
     }
}
