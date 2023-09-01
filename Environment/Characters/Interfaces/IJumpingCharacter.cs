using System;

namespace Servant.Characters
{
    public interface IJumpingCharacter : IGroundCharacter
    {
        public bool CanJump_ { get; }
        public event Action JumpEvent;
        public event Action JumpDelayDoneEvent;
        public event Action JumpDelayStartEvent;
        public event Action JumpHasBeenAccepted;
        public void Jump();
    }
}
