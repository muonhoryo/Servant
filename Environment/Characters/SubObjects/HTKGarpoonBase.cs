
using System;
using UnityEngine;
using static Servant.Characters.IGarpoonBase;
using MuonhoryoLibrary.Unity;

namespace Servant.Characters
{
    public sealed class HTKGarpoonBase : MonoBehaviour, IGarpoonBase
    {
        public event Action<IProjectile> ShootEvent = delegate { };
        public event Action CatchOffProjectileEvent=delegate { };
        public event Action StartPullingEvent=delegate { };
        public event Action CancelPullingEvent;
        public event Action EndPullingEvent=delegate { };
        public event Action<float> SetRotationEvent=delegate { };
        public event Action ShowingTheBaseEvent=delegate { };
        public event Action HidingTheBaseEvent=delegate { };

        private static ServantException CantChangeRotationWithActiveProjectile =>
            new ServantException("Can't change rotation of garpoon base. Now garpoon base is rotated by shooted projectile.");

        [SerializeField]
        private GameObject GarpoonProjectilePrefab;
        [SerializeField]
        private SpriteRenderer GarpoonBaseSprite;
        private IGarpoonCharacter Owner;
        [SerializeField]
        private Vector2 ProjectileStartOffset;
        [SerializeField]
        private float OwnerPullSpeed;
        [SerializeField]
        private float ItemPullSpeed;

        private IPuller CurrentPuller;
        public IProjectile ShootedProjectile_ { get; private set; }
        public float CurrentRotation_ 
        {
            get => transform.eulerAngles.z; 
            set
            {
                if (ShootedProjectile_ != null)
                    throw CantChangeRotationWithActiveProjectile;
                SetRotation(value);
            }
        }
        public bool CanBeManualRotated_ => !enabled;
        public bool CanShoot_ => ShootedProjectile_ == null && Owner.CanUseGarpoon_;
        public bool CanCatchHookOff_ => ShootedProjectile_ != null && ShootedProjectile_.HitObject_ != null;
        public bool CanPulling_ => CurrentPuller == null && ShootedProjectile_ != null;
        public bool CanCancelPulling_ => false;
        public bool IsPull_ => CurrentPuller != null;

        public void RotateToGlobalPoint(Vector2 point)
        {
            if (CanBeManualRotated_)
            {
                InternalRotateToGlobalPoint(point);
            }
        }
        public void ShootProjectile(Vector2 Direction)
        {
            SetRotation(Vector2.SignedAngle(Vector2.up, Direction));
            ShootProjectile();
        }
        public void ShootProjectile()
        {
            if (CanShoot_)
            {
                InternalShootProjectile();
            }
        }
        public void CatchHookOff()
        {
            if (CanCatchHookOff_)
            {
                InternalCatchHookOff();
            }
        }
        public void StartPulling()
        {
            if (CanPulling_)
            {
                InternalStartPulling();
            }
        }
        public void CancelPulling() { }

        private void OnPullDoneAction()
        {
            ShootedProjectile_.Destroy();
            CurrentPuller = null;
            EndPullingEvent();
        }
        private void OnCatchPullCancel()
        {
            CurrentPuller.PullDoneEvent -= OnPullDoneAction;
            CurrentPuller.CancelPull();
            CatchOffProjectileEvent-= OnCatchPullCancel;
        }
        private void PullProjectile()
        {
            CurrentPuller = ShootedProjectile_.StartPullingToTarget(new(()=>transform.position,Owner.UnpackedCharacter_,
                ItemPullSpeed,Vector2.zero));
            CurrentPuller.PullDoneEvent += OnPullDoneAction;
            StartPullingEvent();
        }
        private void PullObjectToTarget(IPuller puller)
        {
            CurrentPuller = puller;
            CurrentPuller.PullDoneEvent += OnPullDoneAction;
            CurrentPuller.PullDoneEvent += () => CatchOffProjectileEvent -= OnCatchPullCancel;
            CatchOffProjectileEvent += OnCatchPullCancel;
            StartPullingEvent();
        }
        private void SetRotation(float rotation)
        {
            transform.eulerAngles = transform.eulerAngles.GetEulerAngleOfImage(rotation);
            SetRotationEvent(rotation);
        }
        private void OnHitAction(GameObject obj)
        {
            if (obj.TryGetComponent<IRadialRockingHookCatcher>(out var parsedObj))
            {
                void ResetRocking()
                {
                    Owner.StopRadialRocking();
                    Owner.StopRopeRocking();
                    CatchOffProjectileEvent -= ResetRocking;
                    StartPullingEvent -= ResetRocking;
                }
                Owner.StartRadialRocking(parsedObj.RockingDirection_, parsedObj.RockingSpeed_);

                CatchOffProjectileEvent += ResetRocking;
                StartPullingEvent += ResetRocking;
                ShootedProjectile_.DestroyEvent += ResetRocking;
            }
            else if (obj.GetComponent<IGarpoonPullableObj>() == null)
            {
                void FallingAction_()
                {
                    Owner.StartRopeRocking();
                }
                void FallingAction(IGroundCharacter.FallingStartInfo i)
                {
                    FallingAction_();
                }
                void LandingAction()
                {
                    Owner.StopRopeRocking();
                }
                void LandingAction_(IGroundCharacter.LandingInfo a)
                {
                    LandingAction();
                }
                void StartPullingToHitObjectAction(IGarpoonPullableObj.PullingTargetInfo i)
                {
                    ResetRockingEvents();
                }
                void ResetRockingEvents()
                {
                    if (Owner.IsRocking_)
                        Owner.StopRopeRocking();
                    Owner.StartFallingEvent -= FallingAction;
                    Owner.LandingEvent -= LandingAction_;

                    Owner.StartPullingEvent -= StartPullingToHitObjectAction;
                    ShootedProjectile_.DestroyEvent -= ResetRockingEvents;
                    CatchOffProjectileEvent -= ResetRockingEvents;
                }

                Owner.StartFallingEvent += FallingAction;
                Owner.LandingEvent += LandingAction_;

                Owner.StartPullingEvent += StartPullingToHitObjectAction;
                ShootedProjectile_.DestroyEvent += ResetRockingEvents;
                CatchOffProjectileEvent += ResetRockingEvents;

                if (Owner.IsFalling())
                {
                    FallingAction_();
                }
            }
        }

        private void InternalRotateToGlobalPoint(Vector2 point)
        {
            Vector2 dir = Vector3.Normalize((Vector3)point - transform.position);
            SetRotation(Vector2.SignedAngle(Vector2.up, dir));
        }
        private void InternalShootProjectile()
        {
            enabled = true;
            GameObject projObj = Instantiate(GarpoonProjectilePrefab,
                (Vector2)transform.position + ProjectileStartOffset.AngleOffset(transform.eulerAngles.z),
                Quaternion.Euler(transform.eulerAngles));
            ShootedProjectile_ = projObj.GetComponent<IProjectile>();
            ShootedProjectile_.Initialize(transform, (transform.eulerAngles.z + 90).DirectionOfAngle());
            ShootedProjectile_.MissEvent += StartPulling;
            ShootedProjectile_.HitEvent += OnHitAction;
            ShootedProjectile_.DestroyEvent += () => enabled = false;
            ShootedProjectile_.DestroyEvent += () => ShootedProjectile_ = null;
            ShootEvent(ShootedProjectile_);
        }
        private void InternalCatchHookOff()
        {
            CatchOffProjectileEvent();
            PullProjectile();
        }
        private void InternalStartPulling()
        {
            if (ShootedProjectile_.HitObject_ == null)
            {
                PullProjectile();
            }
            else
            {
                IGarpoonPullableObj.PullingTargetInfo info;
                if (ShootedProjectile_.HitObject_.TryGetComponent<IGarpoonPullableObj>(out var obj))
                {
                    Vector2 offset = ShootedProjectile_.Position_ - obj.Position_;
                    info = new(() => transform.position, Owner.UnpackedCharacter_, ItemPullSpeed, offset);
                    PullObjectToTarget(obj.StartPullingToTarget(info));
                }
                else
                {
                    info = new(() => ShootedProjectile_.Position_, ShootedProjectile_.HitObject_,
                        OwnerPullSpeed, Vector2.zero);
                    PullObjectToTarget(Owner.StartPullingToTarget(info));
                }
            }
        }

        private void Update()
        {
            InternalRotateToGlobalPoint(ShootedProjectile_.Position_);
        }
        private void Awake()
        {
            if (GarpoonProjectilePrefab == null)
                throw ServantException.GetNullInitialization("GarpoonProjectilePrefab");
            if (GarpoonBaseSprite == null)
            {
                GarpoonBaseSprite = GetComponentInChildren<SpriteRenderer>(true);
                if (GarpoonBaseSprite == null)
                    throw ServantException.GetNullInitialization("GarpoonBaseSprite");
            }
            Owner = GetComponentInParent<IGarpoonCharacter>();
            if (Owner == null)
                throw ServantException.GetNullInitialization("Owner");

            if(enabled)
                enabled = false;
        }
        private void OnEnable()
        {
            GarpoonBaseSprite.enabled = true;
            Owner.TurnGarpoon(true);
            ShowingTheBaseEvent();
        }
        private void OnDisable()
        {
            GarpoonBaseSprite.enabled = false;
            Owner.TurnGarpoon(false);
            HidingTheBaseEvent();
        }
    }
}
