


using Servant.InteractionObjects;
using System;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        public bool CanInteract_ => !IsLockedControl_ && InteractiveTarget != null;
        public event Action InteractionEvent=delegate { };
        private IInteractiveObject InteractiveTarget;
        bool IInteractingCharacter.AssignInteractiveTarget(IInteractiveObject obj)
        {
            if (InteractiveTarget == null &&
                obj != null)
            {
                obj.Show();
                InteractiveTarget = obj;
                return true;
            }
            else return false;
        }
        bool IInteractingCharacter.RemoveInteractTarAssignment(IInteractiveObject removedObject)
        {
            if (InteractiveTarget != null &&
                InteractiveTarget == removedObject)
            {
                InteractiveTarget.Hide();
                InteractiveTarget = null;
                return true;
            }
            else return false;
        }

        private void InternalInteract()
        {
            InteractiveTarget.Interact();
            InteractionEvent();
        }
    }
}
