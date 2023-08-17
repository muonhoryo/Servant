using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public sealed class IntegerVariableResetAnimExit:StateMachineBehaviour
    {
        [SerializeField]
        private string Name;
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetInteger(Name) != 0)
                animator.SetInteger(Name, 0);
        }
    }
}
