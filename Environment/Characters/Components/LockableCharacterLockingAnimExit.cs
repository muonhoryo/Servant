using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Servant.Characters
{
    public sealed class LockableCharacterLockingAnimExit : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var parsedOwner = animator.GetComponent<IAnimLockableCharacter>();
            if (parsedOwner == null)
                throw ServantException.GetNullInitialization("Owner");

            parsedOwner.LockingAnimationExit();
        }
    }
}
