using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    public sealed class CharacterRockingModule_Simple :GarpoonBaseRockingModule,
        IModuleChangingScript<IFallingCheckingModule>

    {
        IFallingCheckingModule IModuleChangingScript<IFallingCheckingModule>.Module__
        { get => FallingChecker; set => FallingChecker = value; }
        private IFallingCheckingModule FallingChecker;

        protected override void AwakeAction()
        {
            void ActivationAction()
            {
                if (FallingChecker.CurrentFallingState_ == IFallingCheckingModule.FallingState.Falling)
                    ActivateRocking();
                else
                    NonActiveRockingSubscribe();
            }
            ActivateEvent += ActivationAction;
        }

        protected override void ActivateRockingAction()
        {
            void ResetEvents()
            {
                FallingChecker.LandingEvent -= LandAction;
                StopRopeRockingEvent -= ResetEvents;
            }
            void LandAction(IFallingCheckingModule.LandingInfo i)
            {
                ResetEvents();
                DeactivateRocking();
            }
            FallingChecker.LandingEvent += LandAction;
            StopRopeRockingEvent += ResetEvents;
        }

        protected override void DeactivateRockingAction()
        {
            NonActiveRockingSubscribe();
        }
        private void NonActiveRockingSubscribe()
        {
            void ResetEvents()
            {
                FallingChecker.StartFallingEvent -= FallAction;
                DeactivateEvent -= ResetEvents;
            }
            void FallAction(IFallingCheckingModule.FallingStartInfo i)
            {
                ResetEvents();
                ActivateRocking();
            }
            FallingChecker.StartFallingEvent += FallAction;
            DeactivateEvent += ResetEvents;
        }
    }
}
