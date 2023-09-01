using Servant.Characters.COP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters
{
    /// <summary>
    /// Requaire DoubleInitializer and ModuleInitializer's
    /// </summary>
    /// <typeparam name="TModuleType"></typeparam>
    public abstract class CharacterFallingStateModuleDependence<TModuleType>:Module,
        IDoubleModuleChangingScript<TModuleType>,
        IModuleChangingScript<IFallingCheckingModule>
        where TModuleType :class,IModule
    {
        TModuleType IDoubleModuleChangingScript<TModuleType>.FirstModule__ 
        { get => GroundModule; set => GroundModule = value; }
        TModuleType IDoubleModuleChangingScript<TModuleType>.SecondModule__
        { get => AirModule;set=> AirModule = value; }
        IFallingCheckingModule IModuleChangingScript<IFallingCheckingModule>.Module__
        { get => FallingChecker; set => FallingChecker = value; }

        [SerializeField]
        private Component OwnerComponent;
        [SerializeField]
        private Component GroundModuleComponent;
        [SerializeField]
        private Component AirModuleComponent;
        [SerializeField]
        private Component FallingCheckerComponent;

        private IModuleChangingScript<TModuleType> Owner;
        private TModuleType GroundModule;
        private TModuleType AirModule;
        private IFallingCheckingModule FallingChecker;
        private void Awake()
        {
            Owner = OwnerComponent as IModuleChangingScript<TModuleType>;
            if (Owner == null)
                throw ServantException.GetNullInitialization("Owner");
            GroundModule = GroundModuleComponent as TModuleType;
            if (GroundModule == null)
                throw ServantException.GetNullInitialization("GroundModule");
            AirModule = AirModuleComponent as TModuleType;
            if (AirModule == null)
                throw ServantException.GetNullInitialization("AirModule");
            FallingChecker = FallingCheckerComponent as IFallingCheckingModule;
            if (FallingChecker == null)
                throw ServantException.GetNullInitialization("FallingChecker");

            void ActivationAction()
            {
                FallingChecker.LandingEvent += LandAction;
                FallingChecker.StartFallingEvent += FallAction;
                FallingChecker.StartRisingEvent += RisingAction;

                GroundModule.ActivateEvent -= ActivateModuleAction;
                AirModule.ActivateEvent -= ActivateModuleAction;

                if (FallingChecker.IsInAir())
                    Owner.Module_ = AirModule;
                else
                    Owner.Module_ = GroundModule;
            }
            void ActivateModuleAction()
            {
                IsActive_ = true;
            }
            void DeactivationAction()
            {
                FallingChecker.LandingEvent -= LandAction;
                FallingChecker.StartFallingEvent -= FallAction;
                FallingChecker.StartRisingEvent -= RisingAction;

                GroundModule.ActivateEvent += ActivateModuleAction;
                AirModule.ActivateEvent += ActivateModuleAction;
            }
            void LandAction(IFallingCheckingModule.LandingInfo i)
            {
                Owner.Module_ = GroundModule;
            }
            void FallAction(IFallingCheckingModule.FallingStartInfo i)
            {
                Owner.Module_ = AirModule;
            }
            void RisingAction(IFallingCheckingModule.GroundFreeRisingInfo i)
            {
                Owner.Module_ = AirModule;
            }

            ActivateEvent += ActivationAction;
            DeactivateEvent += DeactivationAction;
        }
        private void Start()
        {
            void OnDeactivateMovingModuleAction()
            {
                if ((GroundModule.IsActive_ || AirModule.IsActive_) == false)
                {
                    IsActive_ = false;
                }
            }

            GroundModule.DeactivateEvent += OnDeactivateMovingModuleAction;
            AirModule.DeactivateEvent += OnDeactivateMovingModuleAction;

            IsActive_ = GroundModule.IsActive_ || AirModule.IsActive_;
        }
        protected sealed override bool CanTurnActivityFromOutside_ => false;
    }
}
