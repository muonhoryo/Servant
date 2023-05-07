
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant.Control
{
    public sealed class GroundChecker:MonoBehaviour
    {
        public uint CollisionCount { get; private set; } = 0;
        private MainCharacterController owner;
        private float PrevHeight=0;
        private void Awake()
        {
            if (owner == null) owner = GetComponentInParent<MainCharacterController>();
            if (owner == null) throw ServantException.GetNullInitialization("owner");
        }
        private void Start()
        {
            if (IsFreeStanding())
            {
                owner.StartFalling();
            }
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer.IsInLayerMask(Registry.GroundLayerMask)) CollisionCount++;
        }
        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.layer.IsInLayerMask(Registry.GroundLayerMask))
            {
                if (CollisionCount == 0) throw new ServantException("Collision calculating error.");
                CollisionCount--;
                if (CollisionCount == 0)
                {
                    enabled = true;
                }
            }
        }
        private void FixedUpdate()
        {
            if (transform.position.y < PrevHeight)
            {
                owner.StartFalling();
                enabled = false;
            }
            else PrevHeight = transform.position.y;
        }
        private void OnEnable()
        {
            PrevHeight = transform.position.y;
        }
        public bool IsFreeStanding()
        {
            return CollisionCount == 0;
        }
    }
}
