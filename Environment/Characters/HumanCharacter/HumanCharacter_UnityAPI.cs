


using System;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter_OLD
    {
        protected override void FixedUpdateAction()
        {
            if (AddedAnimationForce != Vector2.zero)
                AcceleratedMoving(MovingDirection_, AddedAnimationForce);
        }
        protected override void AwakeAction()
        {
            AwakeAction_Interaction();

            AwakeAction_LandRolling();
            AwakeAction_Animation();
            AwakeAction_Garpoon();
            AwakeAction_Climbing();
            AwakeAction_Script();
            AwakeAction_Jumping();
            AwakeAction_Dodging();
            AwakeAction_MeleeFighting();

            AfterAwakeAction_Animation();
        }
    }
}
