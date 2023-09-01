
using Servant.InteractionObjects;
using System;
using UnityEngine;
using static Servant.Characters.IInteractingCharacter;

namespace Servant.Characters.COP
{
    public sealed class CharacterInteractionModule:Module,IInteractionModule
    {
        private bool CanInteract = true;
        public bool CanInteract_ 
        { 
            get => CanInteract && InteractiveTarget != null;
            set => CanInteract = value;
        }
        public event Action InteractionEvent = delegate { };

        private IInteractiveObject InteractiveTarget;

        bool IInteractionModule.AssignInteractiveTarget(IInteractiveObject obj)
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
        bool IInteractionModule.RemoveInteractTarAssignment(IInteractiveObject removedObject)
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
        void IInteractionModule.Interact()
        {
            if (CanInteract_)
            {
                InteractiveTarget.Interact();
                InteractionEvent();
            }
        }

        protected override bool CanTurnActivityFromOutside_ => false;
    }
}
