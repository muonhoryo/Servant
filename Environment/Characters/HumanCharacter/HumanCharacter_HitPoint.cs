using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant.Characters 
{ 
    public sealed partial class HumanCharacter
    {
        public event Action DeathEvent=delegate { };
        void IHitPointCharacter.Death()
        {
            DeathEvent();
        }

    }
}
