using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MuonhoryoLibrary;
using static Servant.Characters.IGarpoonBase;

namespace Servant.Characters.COP
{
    public abstract class CharacterMovingModule_RadialRocking : CharacterMovingModule,
        IModuleChangingScript<IGarpoonBase>
    {
        IGarpoonBase IModuleChangingScript<IGarpoonBase>.Module__
        { get => GarpoonBase; set => GarpoonBase = value; }

        [SerializeField]
        private Component GarpoonBaseComponent;
        [SerializeField]
        private Component RockingModuleComponent;

        private IGarpoonBase GarpoonBase;
        private IRockingModule RockingModule;
        protected sealed override Vector2 GetMovingDirection()
        {
            Vector2 dir = (GarpoonBase.ShootedProjectile_.Position_ - transform.position).normalized;
            dir = dir.GetRadialForceDirection(MovingDirModule_.MovingDirection_);
            return dir;
        }
        protected sealed override void MovingAction(Vector2 direction, int horizontalDirection, float speed,float speedModifer)
        {
            Rigidbody_.velocity = speed *speedModifer* direction* horizontalDirection;
        }
        protected sealed override bool CanStartMoving_AdditionalConditions =>!RockingModule.IsRocking_;
        protected sealed override void AwakeAction()
        {
            GarpoonBase = GarpoonBaseComponent as IGarpoonBase;
            if (GarpoonBase == null)
                throw ServantException.GetNullInitialization("GarpoonBase");
            RockingModule = RockingModuleComponent as IRockingModule;
            if (RockingModule == null)
                throw ServantException.GetNullInitialization("RockingModule");
        }
        private void Start()
        {
            StartMovingEvent += (i) => { RockingModule.StopRopeRockingEvent += StopMoving; };
            StopMovingEvent += () => { RockingModule.StopRopeRockingEvent -= StopMoving; };
        }
    }
}
