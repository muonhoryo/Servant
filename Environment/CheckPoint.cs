
using UnityEngine;
using Servant.Serialization;
using Servant.GUI;

namespace Servant.InteractionObjects
{
    public sealed partial class CheckPoint : DefaultInteractiveObject
    {
        public bool IsWasActivated { get; private set; } = false;
        protected override void Interact()
        {
            IsWasActivated = true;
            IsActive_ = false;
            SaveLoadSystem.SaveGame(Registry.SettingsOfCurrentLocation,MainMenuControl.SavedGameName);
        }
    }
}
