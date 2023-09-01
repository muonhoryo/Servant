using Servant.Characters.COP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant.Characters
{
    /// <summary>
    /// Character has module, which can be changed by any script
    /// </summary>
    /// <typeparam name="TModuleType"></typeparam>
    public interface IModuleChangingScript<TModuleType>
        where TModuleType:IModule
    {
        protected TModuleType Module__ { get; set; }
        public sealed TModuleType Module_
        {
            get => Module__;
            set
            {
                Module__.IsActive_ = false;
                Module__ = value;
                Module__.IsActive_ = true;
            }
        }
    }
    public interface IDoubleModuleChangingScript<TModuleType>
        where TModuleType : IModule
    {
        protected TModuleType FirstModule__ { get; set; }
        protected TModuleType SecondModule__ { get; set; }
        public sealed TModuleType FirstModule_
        {
            set
            {
                FirstModule__.IsActive_ = false;
                FirstModule__ = value;
                FirstModule__.IsActive_ = true;
            }
        }
        public sealed TModuleType SecondModule_
        {
            set
            {
                SecondModule__.IsActive_ = false;
                SecondModule__ = value;
                SecondModule__.IsActive_ = true;
            }
        }
    }
}
