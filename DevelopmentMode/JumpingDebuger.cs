using Servant.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Servant.DevelopmentOnly
{
    public sealed class JumpingDebuger : EventsSubcriber<IJumpingCharacter> 
    {
        protected override void SubscribeAction(IJumpingCharacter owner)
        {
            owner.JumpEvent += () => Debug.Log("JumpEvent");
            owner.JumpDelayStartEvent += () => Debug.Log("JumpDelayStart");
            owner.JumpDelayDoneEvent += () => Debug.Log("JumpDelayDone");
            owner.JumpHasBeenAccepted += () => Debug.Log("Jump has been accepted");
        }
    }
}
