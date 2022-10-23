
using UnityEngine;
using Servant.Serialization;

namespace Servant
{
    public abstract class DefaultInteractiveObject: SerializedObject,IInteractiveObject
    {
        [SerializeField]
        private SpriteRenderer sprite;
        private void Awake()
        {
            if (sprite == null) throw ServantException.NullInitialization("sprite");
            sprite.enabled = false;
        }
        void IInteractiveObject.Hide()
        {
            sprite.enabled = false;
        }
        void IInteractiveObject.Interact() => Interact();
        protected abstract void Interact();
        void IInteractiveObject.Show()
        {
            sprite.enabled = true;
        }
    }
}
