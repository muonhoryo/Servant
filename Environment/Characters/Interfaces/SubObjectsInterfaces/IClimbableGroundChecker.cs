using System;
using UnityEngine;

namespace Servant.Characters
{
    public interface IClimbableGroundChecker
    {
        public event Action<GameObject, int> FoundClimbableGroundEvent;
        public event Action LostClimbableGroundEvent;
        public GameObject ClimbableGround_ { get; }
        public int ClimbableGroundDirection_ { get; }
        /// <summary>
        /// Return highest point of current climbable ground.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetClimbPosition();

        public bool HasClimbableGroundAround();
    }
}
