
using UnityEngine;
using Servant.Serialization;
using Servant.GUI;

namespace Servant.InteractionObjects
{
    public sealed partial class CheckPoint : DefaultInteractiveObject
    {
        public bool IsWasActivated_ { get; private set; } = false;
        protected override void Interact()
        {
            IsWasActivated_ = true;
            IsActive_ = false;
            SaveLoadSystem.SaveGame(Registry.SettingsOfCurrentLocation,MainMenuControl.SavedGameName);
        }
    }
}
