using MuonhoryoLibrary.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IDodgingCharacter;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    public abstract class CharacterForceDodgingModule:Module,
        IModuleChangingScript<IDodgingModule>,
        IModuleChangingScript<IGroundDirectionCalculatingModule>,
        IModuleChangingScript<IMovingDirectionChangingModule>,
        IModuleChangingScript<IMovingModule>
    {
        private bool IsForceDodge = false;

        [SerializeField]
        private Component DodgingModuleComponent;
        [SerializeField]
        private Component GroundCalculatingModuleComponent;
        [SerializeField]
        private Component MovingDirChangerComponent;
        [SerializeField]
        private Component MovingModuleComponent;

        private IDodgingModule DodgingModule;
        private IGroundDirectionCalculatingModule GroundCalculatingModule;
        private IMovingDirectionChangingModule MovingDirChanger;
        private IMovingModule MovingModule;

        IDodgingModule IModuleChangingScript<IDodgingModule>.Module__
        { get => DodgingModule;set=>DodgingModule = value; }
        IGroundDirectionCalculatingModule IModuleChangingScript<IGroundDirectionCalculatingModule>.Module__
        { get => GroundCalculatingModule; set => GroundCalculatingModule = value; }
        IMovingDirectionChangingModule IModuleChangingScript<IMovingDirectionChangingModule>.Module__
        { get => MovingDirChanger; set => MovingDirChanger = value; }
        IMovingModule IModuleChangingScript<IMovingModule>.Module__
        { get => MovingModule;set=>MovingModule= value; }

        private void Awake()
        {
            DodgingModule = DodgingModuleComponent as IDodgingModule;
            if (DodgingModule == null)
                throw ServantException.GetNullInitialization("DodgingModule");
            GroundCalculatingModule = GroundCalculatingModuleComponent as IGroundDirectionCalculatingModule;
            if (GroundCalculatingModule == null)
                throw ServantException.GetNullInitialization("GroundCalculatingModule");
            MovingDirChanger = MovingDirChangerComponent as IMovingDirectionChangingModule;
            if (MovingDirChanger == null)
                throw ServantException.GetNullInitialization("MovingDirChanger");
            MovingModule = MovingModuleComponent as IMovingModule;
            if (MovingModule == null)
                throw ServantException.GetNullInitialization("MovingModule");

            GroundCalculatingModule.RecalculateGroundDirectionEvent += OnChangeGroundDirectionAction_ActivateModule;
        }
        private void OnChangeGroundDirectionAction_ActivateModule(Vector2 direction)
        {
            if (!IsForceDodge)
            {
                float angle = direction.AngleFromDirection();
                int dodgeDirection = -1;
                if (angle > 180)
                {
                    angle = 360 - angle;
                    dodgeDirection = 1;
                }
                if(angle>= ForceDodgingMinGroundAngle_)
                {
                    StartForceDodging(dodgeDirection);
                }
            }
        }
        private void OnChangeGroundDirectionAction_DeactivateModule(Vector2 direction)
        {
            if (IsForceDodge)
            {
                float angle=direction.AngleFromDirection();
                if (angle > 180)
                    angle = 360 - angle;
                if (angle < ForceDodgingMinGroundAngle_)
                    StopForceDodging();
            }
        }
        private void StartForceDodging(int direction)
        {
            if (!MovingDirChanger.CanChangeMovingDirection_)
                MovingDirChanger.CanChangeMovingDirection_ = true;
            MovingDirChanger.SetMovingDirection(direction);
            MovingDirChanger.CanChangeMovingDirection_ = false;
            IsForceDodge = true;
            DodgingModule.StartDodging();
            DodgingModule.CanStopDodge_ = false;
            MovingModule.CanStopMoving_ = false;
            DodgingModule.StopDodgingEvent -= StopForceDodging;
            GroundCalculatingModule.RecalculateGroundDirectionEvent += OnChangeGroundDirectionAction_DeactivateModule;
        }
        private void StopForceDodging()
        {
            DodgingModule.StopDodgingEvent -= StopForceDodging;
            GroundCalculatingModule.RecalculateGroundDirectionEvent -= OnChangeGroundDirectionAction_DeactivateModule;
            DodgingModule.CanStopDodge_ = true;
            MovingDirChanger.CanChangeMovingDirection_ = true;
            MovingModule.CanStopMoving_ = true;
            DodgingModule.StopDodging();
            IsForceDodge = false;
        }

        protected abstract float ForceDodgingMinGroundAngle_ { get; }
        protected sealed override bool CanTurnActivityFromOutside_ => false;
    }
}
