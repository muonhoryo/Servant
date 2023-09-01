using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGarpoonBase;

namespace Servant.Characters.COP
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class CharacterRockingModule_Radial:GarpoonBaseRockingModule,
        IModuleChangingScript<IGarpoonBase>,
        IModuleChangingScript<ISpeedModule>
    {
        IGarpoonBase IModuleChangingScript<IGarpoonBase>.Module__
        { get => GarpoonBase; set => GarpoonBase = value; }
        ISpeedModule IModuleChangingScript<ISpeedModule>.Module__
        { get => SpeedModule;set=>SpeedModule = value; }
        private IGarpoonBase GarpoonBase;
        private ISpeedModule SpeedModule;

        protected override void AwakeAction()
        {
            void ActivationAction()
            {
                if (GarpoonBase.ShootedProjectile_.HitObject_ != null &&
                    GarpoonBase.ShootedProjectile_.HitObject_.TryGetComponent(out IRadialRockingHookCatcher catcher))
                {
                    ActivateRocking();
                }
                else
                    IsActive_ = false;
            }
            ActivateEvent += ActivationAction;
        }
        protected override void ActivateRockingAction()
        {

            var catcher = GarpoonBase.ShootedProjectile_.HitObject_.GetComponent<IRadialRockingHookCatcher>();

            RockingMovingDirChanger.SetMovingDirection( catcher.RockingDirection_);
            RockingMovingDirChanger.CanChangeMovingDirection_ = false;

            var multiplyBuff = SpeedModule.MoveSpeed_.AddModifier_Multiply(catcher.RockingSpeedMultiplier_);
            var addBuff = SpeedModule.MoveSpeed_.AddModifier_Add(catcher.RockingSpeedAdder_);

            void RemoveModifiers()
            {
                multiplyBuff.RemoveModifier();
                addBuff.RemoveModifier();
                StopRopeRockingEvent -= RemoveModifiers;
            }

            StopRopeRockingEvent += RemoveModifiers;
        }
        protected override void DeactivateRockingAction()
        {
            RockingMovingDirChanger.CanChangeMovingDirection_ = true;
        }
    }
}
