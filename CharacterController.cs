
using System;
using UnityEngine;
using Servant.Characters;
using MuonhoryoLibrary;
using Servant.Serialization;

namespace Servant.Control
{
    public sealed partial class CharacterController : MonoBehaviour,ISingltone<CharacterController>
    {
        public static CharacterController Controller_ { get; private set; }
        CharacterController ISingltone<CharacterController>.Singltone { get => Controller_;set => Controller_ = value; }
        void ISingltone<CharacterController>.Destroy() =>
            Destroy(this);

        private IHumanCharacter ControlledCharacter;
        private static IHumanCharacter CtrlChar => Controller_.ControlledCharacter;
        //InputsNames
        private const string Input_Interaction = "Interaction";
        private const string Input_Jump = "Jump";
        private const string Input_Horizontal = "Horizontal";
        private const string Input_Shoot = "Shoot";
        private const string Input_GarpoonCatchHookOff = "CatchOff";
        private const string Input_Dodge = "Dodge";
        private const string Input_Climb = "Climb";
        //Control
        private static float GetHorizontalDirection() => Input.GetAxisRaw(Input_Horizontal);
        private static void ChangeMovingDirection(float movingDirection) =>
           CtrlChar.MovingDirection_ = movingDirection > 0 ? 1 : -1;
        private void Awake()
        {
            if (!TryGetComponent(out ControlledCharacter))
                throw ServantException.GetNullInitialization("Character");

            this.ValidateSingltone();

            MainStates.AwakeAction(this);
            GarpoonStates.AwakeAction(this);

            CtrlChar.DeathEvent += SaveLoadSystem.ResetLocation;
        }
        private void Start()
        {
            OnEnable();
        }
        private void Update()
        {
            CurrentMainState.RunUpdateAction(this);
            CurrentGarpoonState.RunUpdateAction(this);
        }
        private void OnEnable()
        {
            ResetControllerMainState();
            ResetGarpoonControllerState();
        }
        private void OnDisable()
        {
            ChangeControllerState(MainStates.NoneState);
            ChangeGarpoonControllerState(GarpoonStates.NoneState);
        }
    }
}
