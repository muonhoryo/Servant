using System;

namespace Servant.Characters
{
    public interface IHitPointCharacter
    {
        public event Action DeathEvent;
        /// <summary>
        /// First argument equal true, if taken damage is strong.
        /// </summary>
        public event Action<bool> TakeDamageEvent;
        public void TakeDamage(bool isStrongHit);
        public void Death();
    }
}
