
using UnityEngine;

namespace Servant.Control
{
    public sealed class GroundChecker:MonoBehaviour
    {
        public uint CollisionCount { get; private set; } = 0;
        private MainCharacterController owner;
        private new CircleCollider2D collider;
        private void Awake()
        {
            if (owner == null) owner = GetComponentInParent<MainCharacterController>();
            if (collider == null) collider = GetComponent<CircleCollider2D>();
            if (owner == null) throw ServantException.NullInitialization("owner");
            if (collider == null) throw ServantException.NullInitialization("collider");
        }
        private void Start()
        {
            if (IsFall())
            {
                owner.StartFalling();
            }
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer==Registry.GroundLayer) CollisionCount++;
        }
        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.layer == Registry.GroundLayer)
            {
                if (CollisionCount == 0) throw new ServantException("Collision calculating error.");
                CollisionCount--;
                if (CollisionCount == 0)
                {
                    owner.StartFalling();
                }
            }
        } 
        public bool IsFall()
        {
            return CollisionCount == 0;
            /*return Physics2D.OverlapCircle((Vector2)transform.position + collider.offset,
                collider.radius, Registry.GroundLayer) == null;*/
        }
    }
}
