using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Servant.Characters.COP;
using static Servant.Characters.IGarpoonBase.IProjectileShootingModule.IProjectile;

namespace Servant
{
    public abstract class ProjectileHitCheckingModule_Simple:Module,IHitCheckingModule
    {
        public event Action<GameObject, Vector2> HitObjectEvent;
        public GameObject HitObject_ { get; private set; }
        private bool IsCheckCollision = false;
        private ShootInfo MovingInfo;

        [SerializeField]
        private Component MovingModuleComponent;
        [SerializeField]
        private Component SpeedModuleComponent;

        private IProjectileMovingModule MovingModule;
        private ISpeedModule SpeedModule;

        private void StartCollisiongChecking(ShootInfo movingInfo)
        {
            MovingInfo = movingInfo;
            IsCheckCollision = true;
            enabled = true;
        }
        private void StopCollisionChecking()
        {
            IsCheckCollision = false;
            enabled = false;
        }
        private void FixedUpdate()
        {
            RaycastHit2D hit = Physics2D.Raycast
                (transform.position,MovingInfo.Direction,(float)SpeedModule.MoveSpeed_,CollisionLayerMask_);
            if (hit.collider != null)
            {
                StopCollisionChecking();
                HitObject_ = hit.collider.gameObject;
                HitObjectEvent(HitObject_, hit.point);
            }
        }
        private void Awake()
        {
            MovingModule = MovingModuleComponent as IProjectileMovingModule;
            if (MovingModule == null)
                throw ServantException.GetNullInitialization("MovingModule");
            SpeedModule = SpeedModuleComponent as ISpeedModule;
            if (SpeedModule == null)
                throw ServantException.GetNullInitialization("SpeedModule");

            MovingModule.StartMovingEvent += StartCollisiongChecking;
            MovingModule.StopMovingEvent += StopCollisionChecking;
        }
        private void OnEnable()
        {
            if (!IsCheckCollision)
                enabled = false;
        }
        private void OnDisable()
        {
            if (IsCheckCollision)
                enabled = true;
        }

        protected abstract int CollisionLayerMask_ { get; }
        protected sealed override bool CanTurnActivityFromOutside_ => false;
    }
}
