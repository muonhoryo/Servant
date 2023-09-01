using System;

namespace Servant.Characters
{
    /// <summary>
    /// Character can be locked on animation executing time.
    /// </summary>
    public interface IAnimLockableCharacter
    {
        public event Action LockingAnimationEnterEvent;
        public event Action LockingAnimationExitEvent;
        public void LockingAnimationEnter();
        public void LockingAnimationExit();
    }
}
