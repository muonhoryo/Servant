
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant.DevelopmentOnly
{
    public sealed class ControllerEventsDebuger : MonoBehaviour, ISingltone<ControllerEventsDebuger>
    {
        private static ControllerEventsDebuger singltone;
        ControllerEventsDebuger ISingltone<ControllerEventsDebuger>.Singltone
        { get => singltone; set => singltone = value; }
        private void OnValidate()
        {
            this.ValidateSingltone();
        }
        private void Start()
        {
            Registry.CharacterController.StartMovingEvent +=()=>ShowEventInfo("StartMovingEvent");
            Registry.CharacterController.StopMovingEvent += () => ShowEventInfo("StopMovingEvent");
            Registry.CharacterController.SetRunModeEvent += () => ShowEventInfo("SetRunModeEvent");
            Registry.CharacterController.SetWalkModeEvent += () => ShowEventInfo("SetWalkModeEvent");
            Registry.CharacterController.ChangeFiewDirectionEvent += () => ShowEventInfo("ChangeFiewDirectionEvent");
            Registry.CharacterController.JumpEvent += () => ShowEventInfo("JumpEvent");
            Registry.CharacterController.FallingEvent += () => ShowEventInfo("FallingEvent");
            Registry.CharacterController.LandingEvent += () => ShowEventInfo("LandingEvent");
            Registry.CharacterController.ChangeControllerStateEvent += 
                () => ShowEventInfo("ChangeControllerStateEvent");
            Registry.CharacterController.InteractionEvent += () => ShowEventInfo("InteractionEvent");
        }
        private void ShowEventInfo(string eventName)
        {
            Debug.Log(eventName);
        }
    }
}
