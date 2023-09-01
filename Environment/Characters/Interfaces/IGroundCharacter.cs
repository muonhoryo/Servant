using Servant.Characters.COP;
using System;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;
using static Servant.Characters.IGroundCharacter.IFallingCheckingModule;

namespace Servant.Characters
{
    public interface IGroundCharacter
    {
        public interface IMovingModule : IModule
        {
            public event Action<int> StartMovingEvent;
            public event Action StopMovingEvent;
            public bool IsMoving_ { get; }
            /// <summary>
            /// Setter sets locker, getter can return false even locker is inactive.
            /// </summary>
            public bool CanStartMoving_ { get; set; }
            /// <summary>
            /// Setter sets locker, getter can return false even locker is inactive.
            /// </summary>
            public bool CanStopMoving_ { get; set; }
            public void StartMoving();
            public void StopMoving();
        }
        public interface IMovingDirectionChangingModule : IModule
        {
            public event Action<int> ChangeMovingDirectionEvent;
            public int MovingDirection_ { get; }
            /// <summary>
            /// Setter sets locker, getter can return false even locker is inactive.
            /// </summary>
            public bool CanChangeMovingDirection_ { get; set; }
            public void SetMovingDirection(int direction);
        }
        public interface IFiewDirectionChangingModule : IModule
        {
            public event Action<int> ChangeFiewDirectionEvent;
            public int FiewDirection_ { get; }
            /// <summary>
            /// Setter sets locker, getter can return false even locker is inactive.
            /// </summary>
            public bool CanChangeFiewDirection_ { get; set; }
            public void SetFiewDirection(int direction);
        }
        public interface IGroundDirectionCalculatingModule : IModule
        {
            public event Action<Vector2> RecalculateGroundDirectionEvent;
            public Vector2 GroundDirection_ { get; }
        }
        public interface IFallingCheckingModule : IModule
        {
            public enum FallingState
            {
                StandingOnGround,
                GroundFreeRising,
                Falling
            }
            public struct LandingInfo
            {
                public LandingInfo(Vector2 LandingForce)
                {
                    this.LandingForce = LandingForce;
                }
                public Vector2 LandingForce;
            }
            public struct FallingStartInfo
            {
                public FallingStartInfo(bool wasGroundFreeRising)
                {
                    WasGroundFreeRising = wasGroundFreeRising;
                }
                public bool WasGroundFreeRising;
            }
            public struct GroundFreeRisingInfo
            {
                public bool WasFalling;

                public GroundFreeRisingInfo(bool wasFalling)
                {
                    WasFalling = wasFalling;
                }
            }

            public event Action<LandingInfo> LandingEvent;
            public event Action<FallingStartInfo> StartFallingEvent;
            public event Action<GroundFreeRisingInfo> StartRisingEvent;
            /// <summary>
            /// If true - character is rises.
            /// </summary>
            public event Action<bool> ChangeVerticalMovingDirectionEvent;
            public FallingState CurrentFallingState_ { get; }
            public bool IsUp_ { get; }
        }
        public interface IWallCheckingModule : IModule
        {
            public event Action FoundWallAtRightSideEvent;
            public event Action FoundWallAtLeftSideEvent;
            public event Action LostWallAtRightSideEvent;
            public event Action LostWallAtLeftSideEvent;
            public bool HasWallAtDirection(int direction);
        }

        public event Action<int> StartMovingEvent;
        public event Action StopMovingEvent;
        public event Action<int> ChangeMovingDirectionEvent;
        public event Action<int> ChangeFiewDirectionEvent;
        public event Action<LandingInfo> LandingEvent;
        public event Action<FallingStartInfo> StartFallingEvent;
        public event Action<GroundFreeRisingInfo> StartRisingEvent;
        public event Action<bool> ChangeVerticalMovingDirectionEvent;
        public event Action FoundWallAtRightSideEvent;
        public event Action FoundWallAtLeftSideEvent;
        public event Action LostWallAtRightSideEvent;
        public event Action LostWallAtLeftSideEvent;
        public float MoveSpeed_ => SpeedModule_.MoveSpeed_.CurrentValue_;
        public bool IsMoving_ => MovingModule_.IsMoving_;
        public bool CanStartMoving_ 
        { get=>MovingModule_.CanStartMoving_&&CanStartMoving__; set=>CanStartMoving__=value; }
        protected bool CanStartMoving__ { get; set; }
        public bool CanStopMoving_ 
        { get=>MovingModule_.CanStopMoving_&&CanStopMoving__; set=>CanStopMoving__=value; }
        protected bool CanStopMoving__ { get; set; }
        public int MovingDirection_ => MovingDirChangingModule_.MovingDirection_;
        public bool CanSetMovingDirection_ 
        { get=>MovingDirChangingModule_.CanChangeMovingDirection_&&CanSetMovingDirection__; set=> CanSetMovingDirection__ = value; }
        protected bool CanSetMovingDirection__ { get; set; }
        public int FiewDirection_ => FiewDirectionChangingModule_.FiewDirection_;
        public bool CanSetFiewDirection_ 
        { get=>FiewDirectionChangingModule_.CanChangeFiewDirection_&&CanSetFiewDirection__; set=>CanSetFiewDirection__=value; }
        protected bool CanSetFiewDirection__ { get; set; }
        public FallingState CurrentFallingState_ => FallingCheckingModule_.CurrentFallingState_;
        public bool IsUp_ => FallingCheckingModule_.IsUp_;
        public void StartMoving() 
        {
            if (CanStartMoving_)
                MovingModule_.StartMoving();
        }
        public void StopMoving() 
        {
            if (CanStopMoving_)
                MovingModule_.StopMoving();
        }
        public void SetDirection(int direction)
        {
            if (CanSetMovingDirection_)
                MovingDirChangingModule_.SetMovingDirection(direction);
        }
        public void SetFiewDirection(int direction)
        {
            if (CanSetFiewDirection_)
                FiewDirectionChangingModule_.SetFiewDirection(direction);
        }
        public bool HasWallAtDirection(int direction) => WallChecker_.HasWallAtDirection(direction);

        protected IMovingModule MovingModule_ { get; }
        protected IMovingDirectionChangingModule MovingDirChangingModule_ { get; }
        protected IFiewDirectionChangingModule FiewDirectionChangingModule_ { get; }
        protected IFallingCheckingModule FallingCheckingModule_ { get; }
        protected IWallCheckingModule WallChecker_ { get; }
        protected ISpeedModule SpeedModule_ { get; }
    }
    public interface IGroundCharacter_ModuleChanging:
        IModuleChangingScript<IMovingModule>,
        IModuleChangingScript<IMovingDirectionChangingModule>,
        IModuleChangingScript<IFiewDirectionChangingModule>,
        IModuleChangingScript<IFallingCheckingModule>,
        IModuleChangingScript<IWallCheckingModule>,
        IModuleChangingScript<ISpeedModule>
    {
        protected IMovingModule MovingModule_ { get; set; }
        protected IMovingDirectionChangingModule MovingDirChangningModule_ { get; set; }
        protected IFiewDirectionChangingModule FiewDirectionChangingModule_ { get; set; }
        protected IFallingCheckingModule FallingCheckingModule_ { get; set; }
        protected IWallCheckingModule WallChecker_ { get; set; }
        protected ISpeedModule SpeedModule_ { get; set; }

        IMovingModule IModuleChangingScript<IMovingModule>.Module__
        { get => MovingModule_; set => MovingModule_ = value; }
        IMovingDirectionChangingModule IModuleChangingScript<IMovingDirectionChangingModule>.Module__
        { get => MovingDirChangningModule_; set => MovingDirChangningModule_ = value; }
        IFiewDirectionChangingModule IModuleChangingScript<IFiewDirectionChangingModule>.Module__
        { get => FiewDirectionChangingModule_; set => FiewDirectionChangingModule_ = value; }
        IFallingCheckingModule IModuleChangingScript<IFallingCheckingModule>.Module__
        { get => FallingCheckingModule_; set => FallingCheckingModule_ = value; }
        IWallCheckingModule IModuleChangingScript<IWallCheckingModule>.Module__
        { get => WallChecker_; set => WallChecker_ = value; }
        ISpeedModule IModuleChangingScript<ISpeedModule>.Module__ 
        { get => SpeedModule_; set => SpeedModule_ = value; }
    }
}
