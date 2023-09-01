using Servant.Characters.COP;
using System;
using UnityEngine;
using static Servant.Characters.IGarpoonBase.IProjectileShootingModule;
using static Servant.Characters.IGarpoonBase;
using static Servant.Characters.IGarpoonBase.IProjectileShootingModule.IProjectile;

namespace Servant.Characters
{
    public interface IGarpoonBase:IModule
    {
        public interface IGarpoonHittableObj
        {
            public Vector2 HitPosition_ { get; }
        }
        public interface IRadialRockingHookCatcher : IGarpoonHittableObj
        {
            /// <summary>
            /// -1 - clockwise, 1 - counter-clockwise
            /// </summary>
            public int RockingDirection_ { get; }
            public float RockingSpeedMultiplier_ { get; }
            public float RockingSpeedAdder_ { get; }
        }

        public interface IRockingModule : IModule
        {
            public event Action StartRopeRockingEvent;
            public event Action StopRopeRockingEvent;
            public bool CanStopRocking_ { get; set; }
            public bool IsRocking_ { get; }
            public void StopRopeRocking();
        }
        public interface IProjectileShootingModule : IModule
        {
            public interface IProjectile : IGarpoonPullableObj
            {
                public interface IRope
                {
                    public void Connect(Transform Base);
                }

                public interface IProjectileMovingModule:IModule
                {
                    public event Action<ShootInfo> StartMovingEvent;
                    public event Action StopMovingEvent;
                    public event Action MissEvent;
                    public bool CanStartMoving_{ get; set; }
                    public bool CanStopMoving_ { get; set; }
                    public bool IsMoving_ { get; }
                    public float PassedDistance_ { get; }
                    public void StartMoving(ShootInfo movingInfo);
                    public void StopMoving();
                }
                public interface IHitCheckingModule:IModule
                {
                    public event Action<GameObject,Vector2> HitObjectEvent;
                    public GameObject HitObject_ { get; }
                }
                public struct ShootInfo
                {
                    public Vector2 Direction;
                    public Vector2 StartPosition;
                    public float MaxDistance;
                    public ShootInfo(Vector2 direction, Vector2 startPosition, float maxDistance)
                    {
                        Direction = direction;
                        StartPosition = startPosition;
                        MaxDistance = maxDistance;
                    }
                }
                public event Action MissEvent;
                public event Action<GameObject> HitEvent;
                public event Action DestroyEvent;
                public ShootInfo Info_ { get; }
                public float PassedDistance_ => MovingModule_.PassedDistance_;
                public GameObject HitObject_ => HitChecker_.HitObject_;
                public Transform BaseTransform_ { get; }
                public void Destroy();
                public void Initialize(Transform BaseTransform, Vector2 Direction);

                protected IProjectileMovingModule MovingModule_ { get; }
                protected IHitCheckingModule HitChecker_ { get; }
                protected ISpeedModule SpeedModule_ { get; }
            }
            public interface IProjectile_ModuleChanging :
                IModuleChangingScript<IProjectileMovingModule>,
                IModuleChangingScript<IHitCheckingModule>,
                IModuleChangingScript<ISpeedModule>
            {
                IProjectileMovingModule IModuleChangingScript<IProjectileMovingModule>.Module__
                { get => MovingModule_; set => MovingModule_ = value; }
                IHitCheckingModule IModuleChangingScript<IHitCheckingModule>.Module__
                { get => HitChecker_; set => HitChecker_ = value; }
                ISpeedModule IModuleChangingScript<ISpeedModule>.Module__
                { get => SpeedModule_; set => SpeedModule_ = value; }
                protected IProjectileMovingModule MovingModule_ { get; set; }
                protected IHitCheckingModule HitChecker_ { get; set; }
                protected ISpeedModule SpeedModule_ { get; set; }
            }
            public interface IRockingProjectile : IProjectile
            {
                public void ConnectRockingRopeJoint(DistanceJoint2D rope);
            }

            public event Action<IProjectile, ShootInfo> ShootEvent;
            public event Action MissEvent;
            public event Action<GameObject> HitEvent;
            public event Action DestroyProjectileEvent;
            public ShootInfo Info_ => ShootedProjectile_.Info_;
            public GameObject HitObjest_ => ShootedProjectile_.HitObject_;
            public bool CanShootProjectile_ { get; set; }
            public bool CanDestroyProjectile_ { get; set; }
            public void ShootProjectile(Vector2 direction);
            public void DestroyProjectile();

            public IProjectile ShootedProjectile_ { get; }
        }
        public interface IRotatingModule:IModule
        {
            public event Action<float> RotatingBaseEvent;
            public bool CanRotate_ { get; set; }
            public float CurrentRotation_ { get; set; }
        }
        public interface ICatchingOffModule : IModule
        {
            public event Action CatchHookOffEvent;
            public bool CanCatchHookOff_ { get; set; }
            public void CatchHookOff();
        }
        public interface IPullingModule : IModule
        {
            public interface IPuller
            {
                public event Action StartPullingEvent;
                public event Action PullDoneEvent;
                public Vector2 CurrentTargetPosition_ { get; }
                public void StartPulling(Func<Vector2> getTargetPosFunc);
                public void CancelPull();
            }
            public event Action StartPullingEvent;
            public event Action StopPullingEvent;
            public IPuller CurrentPuller_ { get; }
            public bool CanStartPulling_ { get; set; }
            public bool CanStopPulling_ { get; set; }
            public void StartPulling();
            public void StopPulling();
        }
        public interface IHidingModule : IModule 
        {
            public event Action ShowingBaseEvent;
            public event Action HidingBaseEvent;
            public bool IsShowing_ { get; set; }
        }

        public event Action StartRopeRockingEvent;
        public event Action StopRopeRockingEvent;
        public event Action<IProjectile, ShootInfo> ShootProjectileEvent;
        public event Action<float> RotationBaseEvent;
        public event Action CatchHookOffEvent;
        public event Action StartPullingEvent;
        public event Action StopPullingEvent;
        public event Action ShowingBaseEvent;
        public event Action HidingBaseEvent;
        public bool CanStopRocking_ 
        { get =>CanStopRocking__&&RockingModule_.CanStopRocking_; set => CanStopRocking__ = value; }
        protected bool CanStopRocking__ { get; set; }
        public bool IsRocking_ => RockingModule_.IsRocking_;
        public IProjectile ShootedProjectile_ => ShootingModule_.ShootedProjectile_;
        public bool CanShootProjectile_ 
        { get => CanShootProjectile__ && ShootingModule_.CanShootProjectile_; set => CanShootProjectile__ = value; }
        protected bool CanShootProjectile__ { get; set; }
        public bool CanDestroyProjectile_ 
        { get => CanDestroyProjectile__ && ShootingModule_.CanDestroyProjectile_; set => CanDestroyProjectile__ = value; }
        protected bool CanDestroyProjectile__ { get; set; }
        public bool CanRotate_ 
        { get=>CanRotate__&&RotatingModule_.CanRotate_; set=>CanRotate__=value; }
        protected bool CanRotate__ { get; set; }
        public float CurrentRotation_ 
        {
            get => RotatingModule_.CurrentRotation_;
            set
            {
                if(CanRotate_)
                    RotatingModule_.CurrentRotation_ = value;
            }
        }
        public bool IsShowing_
        { get => HidingModule_.IsShowing_; set => HidingModule_.IsShowing_ = value; }
        public bool CanCatchHookOff_ 
        { get => CanCatchHookOff__ && CatchingOffModule_.CanCatchHookOff_; set => CanCatchHookOff__ = value; }
        protected bool CanCatchHookOff__ { get; set; }
        public bool IsPull_ => PullingModule_.CurrentPuller_ != null;
        public bool CanStartPulling_ 
        { get=>CanStartPulling__&&PullingModule_.CanStartPulling_; set=>CanStartPulling__=value; }
        protected bool CanStartPulling__ { get; set; }
        public bool CanStopPulling_
        { get => CanStopPulling__ && PullingModule_.CanStartPulling_; set => CanStopPulling__ = value; }
        protected bool CanStopPulling__ { get; set; }
        public void ShootProjectile(Vector2 direction) 
        {
            if(CanShootProjectile_)
                ShootingModule_.ShootProjectile(direction);
        } 
        public void DestroyProjectile()
        {
            if (CanDestroyProjectile_)
                ShootingModule_.DestroyProjectile();
        }
        public void CatchHookOff()
        {
            if (CanCatchHookOff_)
                CatchingOffModule_.CatchHookOff();
        }
        public void StartPulling()
        {
            if (CanStartPulling_)
                PullingModule_.StartPulling();
        }
        public void StopPulling()
        {
            if (CanStopPulling_)
                PullingModule_.StopPulling();
        }

        protected IRockingModule RockingModule_ { get; }
        protected IProjectileShootingModule ShootingModule_ { get; }
        protected IRotatingModule RotatingModule_ { get; }
        protected ICatchingOffModule CatchingOffModule_ { get; }
        protected IPullingModule PullingModule_ { get; }
        protected IHidingModule HidingModule_ { get; }
    }
    public interface IGarpoonBase_ModuleChanging:
        IModuleChangingScript<IRockingModule>,
        IModuleChangingScript<IProjectileShootingModule>,
        IModuleChangingScript<IRotatingModule>,
        IModuleChangingScript<ICatchingOffModule>,
        IModuleChangingScript<IPullingModule>,
        IModuleChangingScript<IHidingModule>
    {
        IRockingModule IModuleChangingScript<IRockingModule>.Module__
        { get => RockingModule_; set => RockingModule_ = value; }
        IProjectileShootingModule IModuleChangingScript<IProjectileShootingModule>.Module__
        { get => ShootingModule_; set => ShootingModule_ = value; }
        IRotatingModule IModuleChangingScript<IRotatingModule>.Module__
        { get => RotatingModule_; set => RotatingModule_ = value; }
        ICatchingOffModule IModuleChangingScript<ICatchingOffModule>.Module__
        { get => CatchingOffModule_; set => CatchingOffModule_ = value; }
        IPullingModule IModuleChangingScript<IPullingModule>.Module__
        { get => PullingModule_; set => PullingModule_ = value; }
        IHidingModule IModuleChangingScript<IHidingModule>.Module__
        { get => HidingModule_; set => HidingModule_ = value; }

        protected IRockingModule RockingModule_ { get; set; }
        protected IProjectileShootingModule ShootingModule_ { get; set; }
        protected IRotatingModule RotatingModule_ { get; set; }
        protected ICatchingOffModule CatchingOffModule_ { get; set; }
        protected IPullingModule PullingModule_ { get; set; }
        protected IHidingModule HidingModule_ { get; set; }
    }
}
