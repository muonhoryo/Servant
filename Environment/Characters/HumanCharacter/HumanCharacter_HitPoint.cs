using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant.Characters 
{ 
    public sealed partial class HumanCharacter_OLD
    {
        public event Action DeathEvent=delegate { };
        public event Action<bool> TakeDamageEvent = delegate { };
        void IHitPointCharacter.Death()
        {
            DeathEvent();
        }
        void IHitPointCharacter.TakeDamage(bool isStrongHit)
        {
            TakeDamageEvent(isStrongHit);
        }
    }
}
