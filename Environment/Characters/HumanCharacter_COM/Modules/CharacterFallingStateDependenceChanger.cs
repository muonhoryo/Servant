using Servant.Characters.COM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters
{
    [RequireComponent(typeof())]
    public abstract class CharacterFallingStateDependenceChanger<TModuleType>:CharacterModule,
        IDoubleModuleChangingScript<TModuleType>
        where TModuleType : ICharacterModule
    {
        private IGroundCharacter Owner;
        private IMovingModule GroundModule;
        private IMovingModule AirModule;
        private void Awake()
        {
            if (!TryGetComponent(out Owner))
                throw ServantException.GetNullInitialization("Owner");
        }
    }
}
