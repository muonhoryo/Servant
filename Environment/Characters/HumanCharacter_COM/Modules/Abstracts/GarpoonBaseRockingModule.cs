using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGarpoonBase;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    public abstract class GarpoonBaseRockingModule:Module,IRockingModule,
        IModuleChangingScript<IMovingModule>,
        IModuleChangingScript<IMovingDirectionChangingModule>,
        IModuleChangingScript<IFiewDirectionChangingModule>
    {
        public event Action StartRopeRockingEvent = delegate { };
        public event Action StopRopeRockingEvent = delegate { };
        public bool CanStopRocking_
        {
            get => CanStopRocking && IsRocking_;
            set => CanStopRocking = value;
        }
        private bool CanStopRocking = true;
        public bool IsRocking_ { get; protected set; } = false;

        IMovingModule IModuleChangingScript<IMovingModule>.Module__
        { get => RockingMovingModule; set => RockingMovingModule = value; }
        IMovingDirectionChangingModule IModuleChangingScript<IMovingDirectionChangingModule>.Module__
        { get => RockingMovingDirChanger; set => RockingMovingDirChanger = value; }
        IFiewDirectionChangingModule IModuleChangingScript<IFiewDirectionChangingModule>.Module__
        { get => RockingFiewDirectionChanger; set => RockingFiewDirectionChanger = value; }

        [SerializeField]
        private Component Owner;
        [SerializeField]
        private Component GarpoonBaseComponent;

        protected IModuleChangingScript<IMovingModule> MovingModuleOwner_ { get; private set; }
        protected IModuleChangingScript<IMovingDirectionChangingModule> MovingDirChangingModuleOwner_ { get; private set; }
        protected IModuleChangingScript<IFiewDirectionChangingModule> FiewDirectionModuleOwner_ { get; private set; }
        protected IGarpoonBase GarpoonBase_ { get; private set; }

        protected IMovingModule RockingMovingModule;
        protected IMovingDirectionChangingModule RockingMovingDirChanger;
        protected IFiewDirectionChangingModule RockingFiewDirectionChanger;

        protected IMovingModule OldMovingModule;
        protected IMovingDirectionChangingModule OldMovingDirModule;
        protected IFiewDirectionChangingModule OldFiewDirModule;

        public void StopRopeRocking()
        {
            if (CanStopRocking_)
                DeactivateRocking();
        }
        /// <summary>
        /// Subscribe on deactivateEvent rocking deactivating, if is rocking currently.
        /// /// </summary>
        private void Awake()
        {
            MovingModuleOwner_ = Owner as IModuleChangingScript<IMovingModule>;
            MovingDirChangingModuleOwner_ = Owner as IModuleChangingScript<IMovingDirectionChangingModule>;
            FiewDirectionModuleOwner_ = Owner as IModuleChangingScript<IFiewDirectionChangingModule>;
            GarpoonBase_ = GarpoonBaseComponent as IGarpoonBase;

            if (MovingModuleOwner_ == null)
                throw ServantException.GetNullInitialization("MovingModuleOwner");
            if (MovingDirChangingModuleOwner_ == null)
                throw ServantException.GetNullInitialization("MovingDirChangingModuleOwner");
            if (FiewDirectionModuleOwner_ == null)
                throw ServantException.GetNullInitialization("FiewDirectionModuleOwner");
            if (GarpoonBase_ == null)
                throw ServantException.GetNullInitialization("GarpoonBase");

            DeactivateEvent += () =>
            {
                if (IsRocking_)
                    DeactivateRocking();
            };
            
            AwakeAction();
        }
        protected abstract void AwakeAction();

        /// <summary>
        /// Change modules (moving, movingDir, fiewDir) to rocking and cashes oldmodule.
        /// </summary>
        protected void ActivateRocking()
        {
            OldMovingModule = MovingModuleOwner_.Module_;
            OldMovingDirModule = MovingDirChangingModuleOwner_.Module_;
            OldFiewDirModule = FiewDirectionModuleOwner_.Module_;

            MovingModuleOwner_.Module_ = RockingMovingModule;
            MovingDirChangingModuleOwner_.Module_ = RockingMovingDirChanger;
            FiewDirectionModuleOwner_.Module_ = RockingFiewDirectionChanger;

            var joint=gameObject.AddComponent<DistanceJoint2D>();
            (GarpoonBase_.ShootedProjectile_ as IProjectileShootingModule.IRockingProjectile).ConnectRockingRopeJoint(joint);
            void DeactivateAction()
            {
                Destroy(joint);
                DeactivateEvent -= DeactivateAction;
            }
            DeactivateEvent += DeactivateAction;

            ActivateRockingAction();
            IsRocking_ = true;
            StartRopeRockingEvent();
        }
        /// <summary>
        /// Return cashes modules.
        /// </summary>
        protected void DeactivateRocking()
        {
            MovingModuleOwner_.Module_ = OldMovingModule;
            MovingDirChangingModuleOwner_.Module_ = OldMovingDirModule;
            FiewDirectionModuleOwner_.Module_ = OldFiewDirModule;

            DeactivateRockingAction();
            IsRocking_ = false;
            StopRopeRockingEvent();
        }

        protected virtual void ActivateRockingAction() { }
        protected virtual void DeactivateRockingAction() { }
    }
}
