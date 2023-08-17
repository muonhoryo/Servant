
using UnityEngine;
using MuonhoryoLibrary;
using Servant.InteractionObjects;
using Servant.Characters;

namespace Servant.DevelopmentOnly
{
    public sealed class InteractionField : MonoBehaviour
    {
        private IInteractingCharacter Owner;
        private new CapsuleCollider2D collider;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IInteractiveObject obj))
            {
                Owner.AssignInteractiveTarget(obj);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out IInteractiveObject obj)&&
                Owner.RemoveInteractTarAssignment(obj))
            {
                Owner.AssignInteractiveTarget(GetNearestInteractiveObject());
            }
        }
        private void Awake()
        {
            if(!TryGetComponent(out collider))
                throw ServantException.GetNullInitialization("collider");
            Owner = GetComponentInParent<IInteractingCharacter>();
            if (Owner == null)
                throw ServantException.GetNullInitialization("Owner");

            if (!enabled)
                enabled = true;
        }
        private void OnDisable()
            => enabled = true;
        private IInteractiveObject GetNearestInteractiveObject()
        {
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
