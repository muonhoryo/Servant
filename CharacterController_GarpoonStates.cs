
using UnityEngine;
using Servant.Characters;
using System;

namespace Servant.Control
{
    public sealed partial class CharacterController
    {
        private void ResetGarpoonControllerState()
        {
            GarpoonControllerState state;
            if (CtrlChar.GarpoonBase_.ShootedProjectile_ == null)
                state = GarpoonStates.ReadyState;
            else if (CtrlChar.GarpoonBase_.IsPull_)
                state = GarpoonStates.PullState;
            else if (CtrlChar.GarpoonBase_.ShootedProjectile_.HitObject_ == null)
                state = GarpoonStates.ShootState;
            else
                state = GarpoonStates.HookState;
            ChangeGarpoonControllerState(state);
        }
        private void ChangeGarpoonControllerState(GarpoonControllerState newState)
        {
            CurrentGarpoonState.RunExitAction(this);
            CurrentGarpoonState= newState;
            CurrentGarpoonState.RunEnterAction(this);
        }
        private GarpoonControllerState CurrentGarpoonState=GarpoonStates.NoneState;
        private class GarpoonControllerState : ControllerState
        {
            public GarpoonControllerState(string StateName, Action<CharacterController> UpdateAction,
                Action<CharacterController> EnterAction, Action<CharacterController> ExitAction) :
                base(StateName, UpdateAction, EnterAction, ExitAction)
            { }
        }
        private static class GarpoonStates
        {
            private static IGarpoonBase GarpoonBase => Controller_.ControlledCharacter.GarpoonBase_;
            //UpdateActions
            private static void Ready_Update(CharacterController owner)
            {
                if (Input.GetButtonDown(Input_Shoot))
                {
                    GarpoonBase.RotateToGlobalPoint(MainCameraBehaviour.singltone.GetCursorPos());
                    GarpoonBase.ShootProjectile();
                }
            }
            private static void Shoot_Update(CharacterController owner)
            {
                if (Input.GetButtonUp(Input_Shoot))
                {
                    GarpoonBase.StartPulling();
                }
            }
            private static void Hook_Update(CharacterController owner)
            {
                if (Input.GetButtonDown(Input_GarpoonCatchHookOff))
                {
                    owner.ChangeGarpoonControllerState(ShootState);
                    GarpoonBase.CatchHookOff();
                }
                else if (Input.GetButtonUp(Input_Shoot))
                {
                    GarpoonBase.StartPulling();
                } 
            }
            private static void Pull_Update(CharacterController owner)
            {
                if (Input.GetButtonDown(Input_GarpoonCatchHookOff))
                {
                    owner.ChangeGarpoonControllerState(ShootState);
                    GarpoonBase.CatchHookOff();
                }
            }
            //Enter
            private static void Shoot_Enter(CharacterController owner)
            {
                GarpoonBase.ShootedProjectile_.HitEvent += (i) => owner.ChangeGarpoonControllerState(HookState);
                GarpoonBase.ShootedProjectile_.DestroyEvent += () => owner.ChangeGarpoonControllerState(ReadyState);
            }


            public static readonly GarpoonControllerState ReadyState = new GarpoonControllerState(
                StateName: "Ready",
                UpdateAction: Ready_Update,
                EnterAction:null,
                ExitAction: null);
            public static readonly GarpoonControllerState ShootState = new GarpoonControllerState(
                StateName: "Shoot",
                UpdateAction: Shoot_Update,
                EnterAction: Shoot_Enter,
                ExitAction: null);
            public static readonly GarpoonControllerState HookState = new GarpoonControllerState(
                StateName: "Hook",
                UpdateAction: Hook_Update,
                EnterAction: null,
                ExitAction: null);
            public static readonly GarpoonControllerState PullState =  new GarpoonControllerState(
                StateName: "Pull",
                UpdateAction: Pull_Update,
                EnterAction: null,
                ExitAction: null);
            public static readonly GarpoonControllerState NoneState = new GarpoonControllerState
                (StateName: "None", null, null, null);

            public static void AwakeAction(CharacterController owner)
            {
                GarpoonBase.ShootEvent += (i) => owner.ChangeGarpoonControllerState(ShootState);

                GarpoonBase.StartPullingEvent += () =>  owner.ChangeGarpoonControllerState(PullState);
            }
        }
    }
}
