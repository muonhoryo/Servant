using Servant.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.AI
{
    public sealed class SimpleGAController:MonoBehaviour
    {
        private IGuardAndroidCharacter Owner;
        private void Awake()
        {
            if (!TryGetComponent(out Owner))
                throw ServantException.GetNullInitialization("Owner");
        }
        private void Start()
        {
            Owner.StartMoving();
            Owner.LandingEvent += (i) => ChangeDirection();
            void ChangeDirection()
            {
                if (!Owner.IsInAir())
                    Owner.MovingDirection_ = Owner.MovingDirection_ > 0 ? -1 : 1;
                Owner.StartMoving();
            }
            Owner.StopMovingEvent += ChangeDirection;
        }
    }
}
