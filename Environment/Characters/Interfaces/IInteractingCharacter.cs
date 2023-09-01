using Servant.Characters.COP;
using Servant.InteractionObjects;
using System;
using static Servant.Characters.IInteractingCharacter;

namespace Servant.Characters
{
    public interface IInteractingCharacter
    {
        public interface IInteractionModule:IModule
        {
            public event Action InteractionEvent;
            /// <summary>
            /// Setter sets locker, getter can return false even locker is inactive.
            /// </summary>
            public bool CanInteract_ { get; set; }
            public void Interact();
            /// <summary>
            /// Return true if target was assigned.
            /// </summary>
            /// <param name="interactiveObject"></param>
            /// <returns></returns>
            public bool AssignInteractiveTarget(IInteractiveObject obj);
            /// <summary>
            /// Return false if target was set null.
            /// </summary>
            /// <param name="removedObject"></param>
            /// <returns></returns>
            public bool RemoveInteractTarAssignment(IInteractiveObject obj);
        }
        public event Action InteractionEvent;
        public bool CanInteract_ 
        { get => InteractionModule_.CanInteract_ && CanInteract__; set => CanInteract__ = value; }
        protected bool CanInteract__ { get; set; }
        public void Interact()
        {
            if(CanInteract_)
            {
                InteractionModule_.Interact();
                RunInteractionEvent();
            }
        }
        protected void RunInteractionEvent();

        protected IInteractionModule InteractionModule_ { get; }
    }
    public interface IInteractionCharacter_ModuleChanging:
        IModuleChangingScript<IInteractionModule>
    {
        IInteractionModule IModuleChangingScript<IInteractionModule>.Module__ 
        { get => InteractionModule_; set => InteractionModule_ = value; }
        protected IInteractionModule InteractionModule_ { get; set; }
    }
}
