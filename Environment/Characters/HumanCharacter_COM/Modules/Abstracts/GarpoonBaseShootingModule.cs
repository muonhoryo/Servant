using MuonhoryoLibrary.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGarpoonBase;
using static Servant.Characters.IGarpoonBase.IProjectileShootingModule;

namespace Servant.Characters.COP
{
    public sealed class GarpoonBaseShootingModule : Module, IProjectileShootingModule
    {
        public event Action<IProjectile, IProjectile.ShootInfo> ShootEvent = delegate{};
        public event Action MissEvent=delegate { };
        public event Action<GameObject> HitEvent=delegate { };
        public event Action DestroyProjectileEvent=delegate { };
        public bool CanShootProjectile_
        { get => CanShootProjectile&&ShootedProjectile_==null; set =>CanShootProjectile=value; }
        private bool CanShootProjectile=true;
        public bool CanDestroyProjectile_
        { get => CanDestroyProjectile&&ShootedProjectile_!=null; set =>CanDestroyProjectile=value; }
        private bool CanDestroyProjectile=true;
        public IProjectile ShootedProjectile_ { get; private set; } = null;

        [SerializeField]
        private GameObject ProjectilePrefab;
        [SerializeField]
        private Vector2 ProjectileStartOffset;

        public void DestroyProjectile()
        {
            if (CanDestroyProjectile_)
            {
                ShootedProjectile_.Destroy();
            }
        }
        public void ShootProjectile(Vector2 direction)
        {
            if (CanShootProjectile_)
            {
                GameObject projObj = Instantiate(ProjectilePrefab,
                    (Vector2)transform.position + ProjectileStartOffset.AngleOffset(transform.eulerAngles.z),
                    Quaternion.Euler(transform.eulerAngles));
                ShootedProjectile_ = projObj.GetComponent<IProjectile>();
                ShootedProjectile_.Initialize(transform, (transform.eulerAngles.z + 90).DirectionOfAngle());

                ShootedProjectile_.MissEvent += MissEvent;
                ShootedProjectile_.HitEvent += HitEvent;
                ShootedProjectile_.DestroyEvent += DestroyProjectileEvent;
                ShootedProjectile_.DestroyEvent += () => ShootedProjectile_ = null;
                ShootEvent(ShootedProjectile_,ShootedProjectile_.Info_);
            }
        }
    }
}
