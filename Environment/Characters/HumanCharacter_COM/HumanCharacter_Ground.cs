using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter
    {
        private IMovingModule MovingModule;
        private IMovingDirectionChangingModule MovingDirChangningModule;
        private IFiewDirectionChangingModule FiewDirectionChangingModule;
        private IGroundDirectionCalculatingModule GroundDirectionCalculator;
        private IFallingCheckingModule FallingCheckingModule;
        IMovingModule IGroundCharacter.MovingModule_ => MovingModule;
        IMovingDirectionChangingModule IGroundCharacter.MovingDirChangingModule_ => MovingDirChangningModule;
        IFiewDirectionChangingModule IGroundCharacter.FiewDirectionChangingModule_ => FiewDirectionChangingModule;
        IGroundDirectionCalculatingModule IGroundCharacter.GroundDirectionCalculator_ => GroundDirectionCalculator;
        IFallingCheckingModule IGroundCharacter.FallingCheckingModule_ => FallingCheckingModule;

        private void AwakeAction_Ground()
        {
            if (!TryGetComponent(out MovingModule))
                throw ServantException.GetNullInitialization("MovingModule");
            if (!TryGetComponent(out MovingDirChangningModule))
                throw ServantException.GetNullInitialization("MovingDirChangningModule");
            if (!TryGetComponent(out FiewDirectionChangingModule))
                throw ServantException.GetNullInitialization("FiewDirectionChangingModule");
            if (!TryGetComponent(out GroundDirectionCalculator))
                throw ServantException.GetNullInitialization("GroundDirectionCalculator");
            if (!TryGetComponent(out FallingCheckingModule))
                throw ServantException.GetNullInitialization("FallingCheckingModule");
        }
    }
}
