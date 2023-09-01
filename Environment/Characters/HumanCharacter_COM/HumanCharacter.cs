using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter:MonoBehaviour, IHumanCharacter
    {
        private void Awake()
        {
            AwakeAction_Interaction();
            AwakeAction_Ground();
        }
    }
}
