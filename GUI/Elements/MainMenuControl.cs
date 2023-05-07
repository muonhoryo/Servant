

using Servant.Serialization;
using UnityEngine;

namespace Servant.GUI
{
    public sealed class MainMenuControl : MonoBehaviour
    {
        public const string SavedGameName = "001";
        private const string StartLevel = "001";
        public void StartGame()
        {
            SaveLoadSystem.LoadLevel(StartLevel);
        }
        public void LoadSavedGame()
        {
            SaveLoadSystem.LoadSavedGame(SavedGameName);
        }
    }
}