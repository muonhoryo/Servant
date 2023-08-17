using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Servant.Characters;

namespace Servant
{
    public sealed partial class RadialRockingHookCatcher : MonoBehaviour, IGarpoonBase.IRadialRockingHookCatcher
    {
        /// <summary>
        /// 1 - clockwise, -1 - counter-clockwise
        /// </summary>
        [SerializeField]
        private int RockingDirection;
        [SerializeField]
        private Vector2 HitPositionOffset;
        [SerializeField]
        private float RockingSpeed;

        private Vector2 HitPosition;
        Vector2 IGarpoonBase.IHittableObj.HitPosition_ => HitPosition;
        int IGarpoonBase.IRadialRockingHookCatcher.RockingDirection_ => RockingDirection;
        float IGarpoonBase.IRadialRockingHookCatcher.RockingSpeed_ => RockingSpeed;
    }
}
