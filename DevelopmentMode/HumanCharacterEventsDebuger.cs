
using UnityEngine;
using MuonhoryoLibrary;
using Servant.Characters;
using UnityEngine.TextCore.Text;

namespace Servant.DevelopmentOnly
{
    public sealed class HumanCharacterEventsDebuger : EventsSubcriber<IHumanCharacter>
    {
        protected override void SubscribeAction(IHumanCharacter owner)
        {
            owner.StartMovingEvent += (i) => Debug.Log("StartMovingEvent");
            owner.StopMovingEvent += () => Debug.Log("StopMovingEvent");
            owner.JumpEvent += () => Debug.Log("JumpEvent");
            owner.StartFallingEvent += (i) => Debug.Log("FallingEvent");
            owner.LandingEvent += (i) => Debug.Log("LandingEvent");
            owner.InteractionEvent += () => Debug.Log("InteractionEvent");
        }
    }
}
