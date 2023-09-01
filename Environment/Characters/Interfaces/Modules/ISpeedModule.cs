using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant.Characters.COP
{
    public interface ISpeedModule:IModule
    {
        public CompositeParameter MoveSpeed_ { get; }
    }
}
