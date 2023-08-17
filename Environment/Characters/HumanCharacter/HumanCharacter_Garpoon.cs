
using System;
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        public event Action<IGarpoonBase.IGarpoonPullableObj.PullingTargetInfo> StartPullingEvent = delegate { };
        public event Action StopPullingEvent = delegate { };
        public event Action StartRopeRockingEvent = delegate { };
        public event Action StopRopeRockingEvent=delegate { };
        public event Action<int, float> StartRadialRockingEvent=delegate { };
        public event Action StopRadialRockingEvent=delegate { };

        [SerializeField]
        private Vector2 HitPositionOffset;

        public IGarpoonBase GarpoonBase_ { get; private set; }
        public bool CanStartRocking_ => !IsRocking_ && GarpoonBase_.ShootedProjectile_ != null;
        public bool CanStopRocking_ => IsRocking_;
        public bool IsRocking_ { get; private set; } = false;
        public bool CanStartRadialRocking_ => !IsRadialRocking_ && GarpoonBase_.ShootedProjectile_ != null;
        public bool CanStopRadialRocking_ => IsRadialRocking_;
        public bool IsRadialRocking_ { get; private set; } = false;
        bool IGarpoonCharacter.CanUseGarpoon_ { get => CanUseGarpoon_; }
        private bool CanUseGarpoon_ => !IsLockedControl_;
        public bool IsPulled_ { get; private set; } = false;
        GameObject IGarpoonCharacter.UnpackedCharacter_ => gameObject;
        Vector3 IGarpoonBase.IGarpoonPullableObj.Position_ => transform.position;
        Vector2 IGarpoonBase.IHittableObj.HitPosition_ => (Vector2)transform.position+HitPositionOffset;

        IGarpoonBase.IPuller IGarpoonBase.IGarpoonPullableObj.StartPullingToTarget
            (IGarpoonBase.IGarpoonPullableObj.PullingTargetInfo info)
        {
            if (info.Target == null)
                throw ServantException.GetArgumentNullException("info.Target");
            if (info.GetTargetPositionFunc == null)
                throw ServantException.GetArgumentNullException("info.GetTargetPositionFunc");

            return InternalStartPullingToTarget(info);
        }
        void IGarpoonCharacter.StartRopeRocking()
        {
            if (CanStartRocking_)
            {
                InternalStartRopeRocking();
            }
        }
        void IGarpoonCharacter.StopRopeRocking()
        {
            if (CanStopRocking_)
            {
                InternalStopRopeRocking();
            }
        }
        void IGarpoonCharacter.StartRadialRocking(int movingDirection, float rockingSpeed)
        {
            if (movingDirection != 1 && movingDirection != -1)
                throw new ServantException("Incorrect input direction.");

            if (CanStartRadialRocking_)
            {
                InternalStartRadialRocking(movingDirection, rockingSpeed);
            }
        }
        void IGarpoonCharacter.StopRadialRocking()
        {
            if (CanStopRadialRocking_)
            {
                InternalStopRadialRocking();
            }
        }
        void IGarpoonCharacter.TurnGarpoon(bool isGarpoonActive)
        {
            Animator_.SetLayerWeight(FreeAnimLayerIndex, isGarpoonActive ? 0 : 1);
            Animator_.SetLayerWeight(GarpoonAnimLayerIndex, isGarpoonActive ? 1 : 0);
        }


        private IGarpoonBase.IPuller InternalStartPullingToTarget
            (IGarpoonBase.IGarpoonPullableObj.PullingTargetInfo info)
        {
            Rigidbody_.gravityScale = 0;
            Rigidbody_.velocity = Vector2.zero;
            GarpoonCharacterPuller puller = gameObject.AddComponent<GarpoonCharacterPuller>();
            puller.Initialize(info.Speed, Rigidbody_, info.GetTargetPositionFunc);
            if (IsMoving_)
                StopMoving();
            IsLockedControl_ = true;
            puller.PullDoneEvent += () => IsLockedControl_ = false;
            puller.PullDoneEvent += () => Rigidbody_.gravityScale = DefaultGravity_;
            puller.PullDoneEvent += () => IsPulled_ = false;
            puller.PullDoneEvent += () => StopPullingEvent();
            StartPullingEvent(info);
            IsPulled_ = true;
            return puller;
        }
        private void InternalStartRopeRocking()
        {
            DistanceJoint2D joint = gameObject.AddComponent<DistanceJoint2D>();
            GarpoonBase_.ShootedProjectile_.ConnectRockingRopeJoint(joint);
            void StopRopeRockingAction()
            {
                StopRopeRockingEvent -= StopRopeRockingAction;
                Destroy(joint);
                GarpoonBase_.CatchOffProjectileEvent -= InternalStopRopeRocking;
            }
            StopRopeRockingEvent += StopRopeRockingAction;
            IsRocking_ = true;
            GarpoonBase_.CatchOffProjectileEvent += InternalStopRopeRocking;
            StartRopeRockingEvent();
        }
        private void InternalStopRopeRocking()
        {
            IsRocking_ = false;
            StopRopeRockingEvent();
        }
        private void InternalStartRadialRocking(int movingDirection,float rockingSpeed)
        {
            if (!IsRocking_)
                InternalStartRopeRocking();
            MovingDirection_ = movingDirection;
            void RockingAction()
            {
                Vector2 dir = (GarpoonBase_.ShootedProjectile_.Position_ - transform.position).normalized;
                dir = dir.GetRadialForceDirection(MovingDirection_);
                NoneAcceleratedMoving(rockingSpeed, dir);
                IsLeftSide_ = transform.position.x < GarpoonBase_.ShootedProjectile_.Position_.x;
            }
            UnityFixedUpdateEvent += RockingAction;
            void StopRadialRockingAction()
            {
                UnityFixedUpdateEvent -= RockingAction;
                StopRadialRockingEvent -= StopRadialRockingAction;
            }
            StopRadialRockingEvent += StopRadialRockingAction;
            if (IsMoving_)
                StopMoving();
            IsRadialRocking_ = true;
            CanChangeMovingDirection = false;
            StartRadialRockingEvent(MovingDirection_, rockingSpeed);
        }
        private void InternalStopRadialRocking()
        {
            IsRadialRocking_ = false;
            CanChangeMovingDirection = true;
            StopRadialRockingEvent();
        }

        private void AwakeAction_Garpoon()
        {
            GarpoonBase_ = GetComponentInChildren<IGarpoonBase>(true);
            if (GarpoonBase_ == null)
                throw ServantException.GetNullInitialization("GarpoonBase_");
        }
    }
}
