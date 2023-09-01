using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters.COP
{
    public abstract class CharacterProjHitObjTypeModuleDependence<TModulesType,TDependedType>:Module,
        IDoubleModuleChangingScript<TModulesType>
        where TModulesType:class,IModule
    {
        TModulesType IDoubleModuleChangingScript<TModulesType>.FirstModule__ 
        { get => EqualModule; set => EqualModule = value; }
        TModulesType IDoubleModuleChangingScript<TModulesType>.SecondModule__
        { get => NotEqualModule; set => NotEqualModule = value; }

        [SerializeField]
        private Component GarpoonBaseComponent;
        [SerializeField]
        private Component OwnerComponent;
        [SerializeField]
        private Component EqualModuleComponent;
        [SerializeField]
        private Component NotEqualModuleComponent;

        private IModuleChangingScript<TModulesType> Owner;
        private TModulesType EqualModule;
        private TModulesType NotEqualModule;
        private void Awake()
        {
            Owner=OwnerComponent as IModuleChangingScript<TModulesType>;
            if (Owner == null)
                throw ServantException.GetNullInitialization("Owner");
            EqualModule = EqualModuleComponent as TModulesType;
            if (EqualModule == null)
                throw ServantException.GetNullInitialization("EqualModule");
            NotEqualModule = NotEqualModuleComponent as TModulesType;
            if (NotEqualModule == null)
                throw ServantException.GetNullInitialization("NotEqualModule");
        }
        private void Start()
        {
            if (GarpoonBaseComponent is not IGarpoonBase garpoonBase)
                throw ServantException.GetNullInitialization("SubscribeTarget");
            void ShootAction(IGarpoonBase.IProjectileShootingModule.IProjectile proj,
                IGarpoonBase.IProjectileShootingModule.IProjectile.ShootInfo info)
            {
                void OnHitAction(GameObject hitObj)
                {
                    if (hitObj.TryGetComponent<TDependedType>(out var j))
                        Owner.Module_ = EqualModule;
                    else
                        Owner.Module_ = NotEqualModule;
                }
                proj.HitEvent += OnHitAction;
            }

            ActivateEvent += () => garpoonBase.ShootProjectileEvent += ShootAction;
            DeactivateEvent += () => garpoonBase.ShootProjectileEvent -= ShootAction;

            IsActive_ = true;
        }
        protected sealed override bool CanTurnActivityFromOutside_ => false;
    }
}
