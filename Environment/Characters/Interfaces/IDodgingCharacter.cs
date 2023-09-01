using Servant.Characters.COP;
using System;
using static Servant.Characters.IDodgingCharacter;

namespace Servant.Characters
{
    public interface IDodgingCharacter : IGroundCharacter, ILayerChangableCharacter
    {
        public interface IDodgingModule : IModule
        {
            public event Action<float> StartDodgingEvent;
            public event Action StopDodgingEvent;
            public bool CanStartDodge_ { get; set; }
            public bool CanStopDodge_ { get; set; }
            public bool IsDodging_ { get; }
            public float CurrentDodgingSpeedBuff_ { get; }
            public void StartDodging();
            public void StopDodging();
        }
        /// <summary>
        /// Return start speed buff.
        /// </summary>
        public event Action<float> StartDodgingEvent;
        public event Action StopDodgingEvent;
        public bool CanStartDodge_
        { get=>DodgingModule_.CanStartDodge_&&CanStartDodge__; set=>CanStartDodge_=value; }
        protected bool CanStartDodge__ { get; set; }
        public bool CanStopDodge_
        { get=>DodgingModule_.CanStopDodge_&&CanStopDodge__; set=> CanStartDodge__ = value; }
        protected bool CanStopDodge__ { get; set; }
        public bool IsDodging_ { get=>DodgingModule_.IsDodging_; }
        public float CurrentDodgingSpeedBuff_ { get=>DodgingModule_.CurrentDodgingSpeedBuff_; }
        public void StartDodging()
        {
            if (CanStartDodge__)
                DodgingModule_.StartDodging();
        }
        public void StopDodging()
        {
            if(CanStopDodge__)
                DodgingModule_.StopDodging();
        }

        protected IDodgingModule DodgingModule_ { get; }
    }
    public interface IDodgingCharacter_ModuleChanging:
        IModuleChangingScript<IDodgingModule>
    {
        IDodgingModule IModuleChangingScript<IDodgingModule>.Module__
        { get => DodgingModule_; set => DodgingModule_ = value; }
        protected IDodgingModule DodgingModule_ { get; set; }
    }
}
