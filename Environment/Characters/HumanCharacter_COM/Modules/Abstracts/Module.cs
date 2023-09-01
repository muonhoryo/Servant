using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters.COP
{
    public abstract class Module:MonoBehaviour,IModule
    {
        event Action IModule.ActivateEvent
        {
            add { ActivateEvent += value; }
            remove { ActivateEvent -= value; }
        }
        event Action IModule.DeactivateEvent
        {
            add { DeactivateEvent += value; }
            remove { DeactivateEvent -= value; }
        }
        protected event Action ActivateEvent;
        protected event Action DeactivateEvent;
        private bool IsActive = false;
        protected bool IsActive_
        {
            get => IsActive;
            set
            {
                bool oldValue = IsActive;
                IsActive = value;
                if(oldValue != value)
                {
                    if (value)
                        ActivateEvent();
                    else
                        DeactivateEvent();
                }
            }
        }
        protected virtual bool CanTurnActivityFromOutside_ => true;
        bool IModule.IsActive_ { get => IsActive_; set {if(CanTurnActivityFromOutside_)IsActive_ = value; } }
    }
}
