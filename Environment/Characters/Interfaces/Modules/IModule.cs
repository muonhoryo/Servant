using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant.Characters.COP
{
    public interface IModule
    {
        public event Action ActivateEvent;
        public event Action DeactivateEvent;
        public bool IsActive_ { get; set; }
    }
}
