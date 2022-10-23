
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant.DevelopmentOnly
{
    public sealed class InteractionField : MonoBehaviour, ISingltone<InteractionField>
    {
        private static InteractionField singltone;
        InteractionField ISingltone<InteractionField>.Singltone 
        { get => singltone; 
            set => singltone=value; }
        private new CapsuleCollider2D collider;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IInteractiveObject obj))
            {
                Registry.CharacterController.AssignInteractiveTarget(obj);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IInteractiveObject obj)&& 
                Registry.CharacterController.RemoveInteractTarAssignment(obj))
            {
                Registry.CharacterController.AssignInteractiveTarget(GetNearestInteractiveObject());
            }
        }
        private void Awake()
        {
            if(!TryGetComponent(out collider))
                throw ServantException.NullInitialization("collider");
        }
        private IInteractiveObject GetNearestInteractiveObject()
        {
            this.ValidateSingltone();
            collider = GetComponent<CapsuleCollider2D>();
            Collider2D[] colliders = Physics2D.OverlapCapsuleAll
                ((Vector2)transform.position + collider.offset, collider.size,
                collider.direction, 0,collider.gameObject.layer);
            foreach(Collider2D collider in colliders)
            {
                if(collider.TryGetComponent(out IInteractiveObject obj))
                {
                    return obj;
                }
            }
            return null;
        }
    }
}
