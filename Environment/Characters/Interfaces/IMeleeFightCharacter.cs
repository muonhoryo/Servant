using System;

namespace Servant.Characters
{
    public interface IMeleeFightCharacter : IAnimLockableCharacter
    {
        public interface IMeleeHitBox
        {
            public bool IsActive_ { get; set; }
        }
        /// <summary>
        /// First argument is shoot direction.
        /// </summary>
        public event Action<int> MeleeShootEvent;
        public event Action MeleeHitBoxActivateEvent;
        public event Action MeleeHitBoxDeactivateEvent;
        public event Action MeleeShootDoneEvent;
        public bool DoMeleeShoot_ { get; }
        public bool IsStrongShoot_ { get; }
        public bool CanMeleeShoot_ { get; }
        public void MeleeShoot(int direction);
    }
}
