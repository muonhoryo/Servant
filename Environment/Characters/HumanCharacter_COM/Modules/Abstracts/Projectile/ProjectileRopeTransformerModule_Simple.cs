using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuonhoryoLibrary.Unity;
using Servant.Characters;
using Servant.Characters.COP;
using UnityEngine;
using static Servant.Characters.IGarpoonBase.IProjectileShootingModule.IProjectile;

namespace Servant
{
    public sealed class ProjectileRopeTransformerModule_Simple:Module,IRope
    {
        [SerializeField]
        private SpriteRenderer RopeComp;
        private Transform Base;
        [SerializeField]
        private Transform ProjectileTransform;

        [SerializeField]
        private Component ProjectileComponent;

        private IGarpoonBase.IProjectileShootingModule.IProjectile Projectile;

        private void Update()
        {
            RopeComp.size = new Vector2(RopeComp.size.x, Vector2.Distance(Base.position,
                ProjectileTransform.position));
            transform.eulerAngles = transform.eulerAngles.GetEulerAngleOfImage
                ((Base.position - ProjectileTransform.position).AngleFromDirection() - 90);
        }
        void IRope.Connect(Transform Base)
        {
            this.Base = Base;
        }
        private void Awake()
        {
            Projectile = ProjectileTransform as IGarpoonBase.IProjectileShootingModule.IProjectile;
            if (Projectile == null)
                throw ServantException.GetNullInitialization("Projectile");

            if (!enabled)
                enabled = true;
        }
        private void OnDisable() => enabled = true;
    }
}
