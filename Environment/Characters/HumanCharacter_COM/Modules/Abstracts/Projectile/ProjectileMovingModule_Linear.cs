using Servant.Characters;
using Servant.Characters.COP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGarpoonBase.IProjectileShootingModule.IProjectile;

namespace Servant
{
    public abstract class ProjectileMovingModule_Linear : Module, IProjectileMovingModule
    {
        public event Action<ShootInfo> StartMovingEvent = delegate { };
        public event Action StopMovingEvent = delegate { };
        public event Action MissEvent = delegate { };

        public bool CanStartMoving_
        { get => CanStartMoving; set => CanStartMoving = value; }
        private bool CanStartMoving=true;
        public bool CanStopMoving_
        { get => false; set { } }
        public bool IsMoving_ { get; private set; }
        public float PassedDistance_ => PassedDistance;
        private float PassedDistance=0;
        private ShootInfo MovingInfo;

        [SerializeField]
        private Component SpeedModuleComponent;
        [SerializeField]
        private Component HitCheckerComponent;

        private ISpeedModule SpeedModule;
        private IHitCheckingModule HitChecker;

        private void FixedUpdate()
        {
            float remDist = GlobalConstants.Singlton.HTK_ProjectileMaxDistance - PassedDistance_;
            void Move(float stepLength)
            {
                transform.position += (Vector3)MovingInfo.Direction * stepLength;
                PassedDistance += stepLength;
            }
            if(remDist<(float)SpeedModule.MoveSpeed_)
            {
                Move(remDist);
                MissEvent();
                InternalStopMoving();
            }
            else
            {
                Move((float)SpeedModule.MoveSpeed_);
            }
        }
        public void StartMoving(ShootInfo movingInfo)
        {
            if (CanStartMoving_)
            {
                IsMoving_ = true;
                enabled = true;
                MovingInfo = movingInfo;
            }
        }
        public void StopMoving()
        {
            if (CanStopMoving_)
            {
                InternalStopMoving();
            }
        }
        private void InternalStopMoving()
        {
            IsMoving_ = false;
            enabled = false;
        }
        private void Awake()
        {
            SpeedModule = SpeedModuleComponent as ISpeedModule;
            if (SpeedModule == null)
                throw ServantException.GetNullInitialization("SpeedModule");
            HitChecker = HitCheckerComponent as IHitCheckingModule;
            if (HitChecker == null)
                throw ServantException.GetNullInitialization("HitChecker");

            HitChecker.HitObjectEvent += (i,j) => InternalStopMoving();
            HitChecker.HitObjectEvent += (obj, pos) =>
            {
                if (obj.TryGetComponent(out IGarpoonBase.IGarpoonHittableObj hitObj))
                {
                    transform.position = hitObj.HitPosition_;
                }
                else
                {
                    transform.position = pos;
                }
                if (obj.layer == MovableItemLayer_)
                {
                    gameObject.transform.parent = obj.transform;
                }
            };

            if (enabled)
                enabled = false;
        }
        private void OnEnable()
        {
            if (!IsMoving_)
                enabled = false;
        }
        private void OnDisable()
        {
            if (IsMoving_)
                enabled = true;
        }

        protected abstract int MovableItemLayer_ { get; }
    }
}
