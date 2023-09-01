using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public partial class GuardAndroid : GroundMovableCharacter,IGuardAndroidCharacter
    {
        protected override float DefaultMoveSpeed_ => GlobalConstants.Singlton.GuardAndroid_MovingSpeed;
        protected override void SetIsLeftSideAction(bool value) { }

        protected override void AwakeAction()
        {
            StartFallingEvent +=(i)=> StopMoving();
            StartGroundFreeRisingEvent += (i) => StopMoving();
        }
    }
}
