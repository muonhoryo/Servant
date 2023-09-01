using System;

namespace Servant.Characters
{
    /// <summary>
    /// Character has any states, which lock his control.
    /// </summary>
    public interface ILockableCharacter
    {
        public event Action UnlockControlEvent;
        public event Action LockControlEvent;
        public bool IsLockedControl_ { get; }
    }
}
