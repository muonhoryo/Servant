using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Servant.Characters
{
    public sealed class LockableCharacterLockingAnimExit : StateMachineBehaviour
    {
        private IAnimLockableCharacter Owner;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animator.TryGetComponent(out Owner))
                throw ServantException.GetNullInitialization("Owner");

            Owner.LockingAnimationEnter();
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Owner.LockingAnimationExit();
        }
    }
}
