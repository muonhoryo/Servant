
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant
{
    public sealed class GarpoonBase:MonoBehaviour
    {
        [SerializeField]
        private Vector2 ProjectileStartOffset;
        [SerializeField]
        private GameObject ProjectilePrefab;
        [SerializeField]
        private SpriteRenderer RopeComp;
        public GarpoonProjectile Projectile { get; private set; }
        private void Update()
        {
            Quaternion angle = Quaternion.Euler(new Vector3(0, 0,
                Vector2.SignedAngle(Vector2.up, Projectile.transform.position-transform.position)));
            transform.rotation = angle;
            RopeComp.size = new Vector2(RopeComp.size.x, Vector2.Distance(transform.position,
                Projectile.transform.position));
        }
        public GarpoonProjectile Initialize(Vector2 Direction, float Speed, float MaxDistance,
            float MaxHookDistance)
        {
            float angleInDegress = Vector2.SignedAngle(Vector2.up, Direction);
            Quaternion angle = Quaternion.Euler(new Vector3(0, 0,angleInDegress));
            transform.rotation = angle;
            Projectile = Instantiate(ProjectilePrefab, (Vector2)transform.position + 
                ProjectileStartOffset.AngleOffset(angleInDegress),
               angle).GetComponent<GarpoonProjectile>();
            Projectile.Initialize(Direction, Speed, MaxDistance,
               MaxHookDistance);
            void OnMissAction()
            {
                Destroy(gameObject);
                Projectile.OnMiss -= OnMissAction;
                Projectile.OnTurnOff -= OnMissAction;
            }
            Projectile.OnMiss += OnMissAction;
            Projectile.OnTurnOff += OnMissAction;
            return Projectile;
        }
    }
}
