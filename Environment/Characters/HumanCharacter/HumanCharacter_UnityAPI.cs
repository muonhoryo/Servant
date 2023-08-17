


using System;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        protected override void AwakeAction()
        {
            AwakeAction_LandRolling();
            AwakeAction_Animation();
            AwakeAction_Garpoon();
            AwakeAction_Climbing();
            AwakeAction_Script();
            AwakeAction_Jumping();
            AwakeAction_Dodging();

            AfterAwakeAction_Animation();
        }
    }
}
