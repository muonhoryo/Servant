using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Servant.Characters.IGroundCharacter;
using UnityEngine;
using static Servant.Characters.IGarpoonBase;
using MuonhoryoLibrary;

namespace Servant.Characters.COP
{
    public abstract class CharacterMovingModule_Rocking : CharacterMovingModule,
        IModuleChangingScript<IRockingModule>,
        IModuleChangingScript<IGarpoonBase>
    {
        IRockingModule IModuleChangingScript<IRockingModule>.Module__
        {
            get => RockingModule;
            set 
            {
                RockingModule = value;
                if (value != null&&IsMoving_)
                    RockingModule.StopRopeRockingEvent += StopMoving;
            }
        }
        IGarpoonBase IModuleChangingScript<IGarpoonBase>.Module__
        { get => GarpoonBase; set => GarpoonBase = value; }

        [SerializeField]
        private Component RockingModuleComponent;
        [SerializeField]
        private Component GarpoonBaseComponent;

        private IRockingModule RockingModule;
        private IGarpoonBase GarpoonBase;
        [SerializeField]
        private Rigidbody2D Rigidbody;
        protected sealed override Vector2 GetMovingDirection()
        {
            Vector2 forceDirection;
            if (transform.position.x > GarpoonBase.ShootedProjectile_.Position_.x !=MovingDirModule_.MovingDirection_ > 0)
            {
                forceDirection = new Vector2(MovingDirModule_.MovingDirection_, 0);
            }
            else
            {
                Vector2 dir = (GarpoonBase.ShootedProjectile_.Position_ - transform.position).normalized;
                forceDirection = dir.GetRadialForceDirection(MovingDirModule_.MovingDirection_);
            }
            return forceDirection;
        }
        protected sealed override void MovingAction(Vector2 direction, int horizontalDirection, float speed,float speedModifier)
        {
            Rigidbody.AddForce(horizontalDirection*speed*speedModifier* direction, ForceMode2D.Force);
        }
        protected sealed override bool CanStartMoving_AdditionalConditions => RockingModule.IsRocking_;
        protected sealed override void AwakeAction()
        {
            RockingModule = RockingModuleComponent as IRockingModule;
            if (RockingModule == null)
                throw ServantException.GetNullInitialization("RockingModule");
            GarpoonBase = GarpoonBaseComponent as IGarpoonBase;
            if (GarpoonBase == null)
                throw ServantException.GetNullInitialization("GarpoonBase");
        }
        private void Start()
        {
            StartMovingEvent += (i) =>
            {
                RockingModule.StopRopeRockingEvent += StopMoving;
            };
            StopMovingEvent += () => { RockingModule.StopRopeRockingEvent -= StopMoving; };
        }
    }
}
