
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant.DevelopmentOnly
{
    public sealed class ControllerStatesDebuger : MonoBehaviour, ISingltone<ControllerStatesDebuger>
    {
        private static ControllerStatesDebuger singltone;
        ControllerStatesDebuger ISingltone<ControllerStatesDebuger>.Singltone 
        { get => singltone; set => singltone=value; }
        private void OnValidate()
        {
            this.ValidateSingltone();
        }
        private void Start()
        {
            Registry.CharacterController.ChangeControllerStateEvent += ShowChangedStateInfo;
        }
        private void ShowChangedStateInfo()
        {
            Debug.Log(Registry.CharacterController.GetCurrentStateName());
        }
        private void OnDestroy()
        {
            if(Registry.CharacterController!=null)
                Registry.CharacterController.ChangeControllerStateEvent -= ShowChangedStateInfo;
        }
    }
}
