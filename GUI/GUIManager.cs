
using UnityEngine;

namespace Servant.GUI
{
    public static class GUIManager
    {
        public static GUIManagerData Data;
        public static GameObject GUICanvas;
        public static void InitializeNewTempleText(Vector2 position,string showedText,float destroyDelay)
        {
            TempleText createdText=Object.Instantiate
                (Data.TempleTextPrefab,position,Quaternion.Euler(Vector3.zero),
                GUICanvas.transform).GetComponent<TempleText>();
            createdText.Initialize(showedText, destroyDelay);
        }
    }
}
