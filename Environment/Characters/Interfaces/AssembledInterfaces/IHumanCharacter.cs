using static Servant.Characters.IGarpoonBase;

namespace Servant.Characters
{
    public interface IHumanCharacter : IInteractingCharacter, IDodgingCharacter, IClimbingCharacter, ILandRollingCharacter,
        IGarpoonOwner, IHitPointCharacter, IMeleeFightCharacter, ILockableCharacter
    { }
    public interface IHumanCharacter_ModuleChanging : IGroundCharacter_ModuleChanging { }
}
