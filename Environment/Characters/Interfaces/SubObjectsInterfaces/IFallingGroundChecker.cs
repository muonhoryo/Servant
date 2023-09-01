using System;

namespace Servant.Characters
{
    public interface IFallingGroundChecker
    {
        public event Action<float> UpdateGroundAngleEvent;
        public event Action<IGroundCharacter.LandingInfo> LandingEvent;
        /// <summary>
        /// Argument is true, if has start rising.
        /// </summary>
        public event Action<bool> LostGroundEvent;
        public event Action<IGroundCharacter.GroundFreeRisingInfo> StartGroundFreeRisingEvent;
        public event Action<IGroundCharacter.FallingStartInfo> StartFallingEvent;
        /// <summary>
        /// Argument is true, if has start rising.
        /// </summary>
        public event Action<bool> ChangeVerticalMovingDirection;

        public bool IsUp_ { get; }
        public bool IsOnAGround_ { get; }
        public float GroundAngle_ { get; }
    }
}
