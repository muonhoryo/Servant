


using System;
using static Servant.Characters.IInteractingCharacter;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        public event Action InteractionEvent
        {
            add { InteractionModule.InteractionEvent += value; }
            remove { InteractionModule.InteractionEvent -= value;}
        }
        public bool CanInteract_ => !IsLockedControl_ && InteractionModule.CanInteract_;

        private IInteractionModule InteractionModule;

        public void Interact()
        {
            if (CanInteract_)
            {
                InteractionModule.Interact();
            }
        }

        private void AwakeAction_Interaction()
        {
            if (!TryGetComponent(out InteractionModule))
                throw ServantException.GetNullInitialization("InteractionModule");
        }
    }
}
