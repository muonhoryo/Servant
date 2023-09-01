using System;
using static Servant.Characters.IGarpoonBase;
using UnityEngine;
using static Servant.Characters.IGarpoonCharacter;
using static Servant.Characters.IGarpoonBase.IProjectileShootingModule;

namespace Servant.Characters
{
    public interface IGarpoonCharacter:IGarpoonPullableObj
    {
        public GameObject UnpackedCharacter_ { get; }
    }
    public interface IGarpoonOwner :  IJumpingCharacter
    {
        public event Action StartRopeRockingEvent;
        public event Action StopRopeRockingEvent;
        public event Action<IProjectile, IProjectile.ShootInfo> ShootProjectileEvent;
        public event Action<float> RotationBaseEvent;
        public event Action CatchHookOffEvent;
        public event Action StartPullingEvent;
        public event Action StopPullingEvent;
        public event Action ShowingBaseEvent;
        public event Action HidingBaseEvent;
        protected IGarpoonBase GarpoonBase_ { get; }
    }
    public interface IGarpoonOwner_ModuleChanging:
        IModuleChangingScript<IGarpoonBase>
    {
        protected IGarpoonBase GarpoonBase_ { get; set; }
        IGarpoonBase IModuleChangingScript<IGarpoonBase>.Module__
        { get => GarpoonBase_; set => GarpoonBase_ = value; }
    }
}
