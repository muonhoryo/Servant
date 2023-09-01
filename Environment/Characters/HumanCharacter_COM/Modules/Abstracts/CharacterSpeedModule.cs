using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters.COP
{
    public abstract class CharacterSpeedModule:Module,ISpeedModule
    {
        public CompositeParameter MoveSpeed_{ get; private set; }
        private void Awake()
        {
            MoveSpeed_ = new(DefaultSpeed_);
        }
        protected abstract float DefaultSpeed_ { get; }

        protected override bool CanTurnActivityFromOutside_ => false;
    }
}
