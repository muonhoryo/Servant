using System;

namespace Servant.Characters
{
    public interface ILandRollingCharacter : IGroundCharacter
    {
        public event Action StartLandingRollEvent;
        public event Action LandingRollDoneEvent;
        public bool IsLandRolling_ { get; }
    }
}
