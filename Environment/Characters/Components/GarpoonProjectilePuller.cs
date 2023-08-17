
using Servant.Characters;
using System;
using UnityEngine;

namespace Servant.Control
{
    public sealed class GarpoonProjectilePuller : GarpoonSimplePuller
    {
        private float PullSpeed;
        private Func<Vector2> GetTargetPosFunc;
        private Transform Owner;
        public void Initialize(float PullSpeed,Func<Vector2> GetTargetPosFunc,Transform Owner)
        {
            if (PullSpeed <= 0)
                throw new ServantIncorrectInputArgument("PullSpeed", "PullSpeed cannot be less or equal zero.");
            if (GetTargetPosFunc == null)
                throw ServantException.GetNullInitialization("GetTurgetPosFunc");
            if (Owner == null)
                throw ServantException.GetNullInitialization("Owner");

            this.PullSpeed = PullSpeed;
            this.GetTargetPosFunc= GetTargetPosFunc;
            this.Owner = Owner;
            IsInitialized = true;
        }
        protected override bool Pull()
        {
            return SimplePulling(PullSpeed, GetTargetPosFunc(), GlobalConstants.Singlton.Garpoon_PullDoneThreshold, Owner);
        }
    }
}
