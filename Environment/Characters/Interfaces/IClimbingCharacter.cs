using System;
using UnityEngine;

namespace Servant.Characters
{
    public interface IClimbingCharacter : IJumpingCharacter
    {
        public struct ClimbingInfo
        {
            public GameObject ClimbableGround;
            public int ClimbingDirection;

            public ClimbingInfo(GameObject climbableGround, int climbingDirection)
            {
                if (climbableGround == null)
                    throw ServantException.GetNullInitialization("climbableGround");

                ClimbableGround = climbableGround;
                ClimbingDirection = climbingDirection;
            }
        }
        public event Action<ClimbingInfo> StartClimbingEvent;
        public event Action ClimbingDoneEvent;
        public bool CanClimb_ { get; }
        public bool IsClimbing_ { get; }
        public void Climb();
    }
}
