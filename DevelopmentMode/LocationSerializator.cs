
using UnityEngine;
using Servant.Serialization;

namespace Servant.DevelopmentOnly
{
    public sealed class LocationSerializator : MonoBehaviour
    {
        [SerializeField]
        private string LocationName;
        //Camera settings
        [SerializeField]
        private float Depth;
        [SerializeField]
        private Rect CameraMoveLimit;
        [SerializeField]
        private Rect CameraMoveTrigger;

        [ContextMenu("Serialize")]
        void Serialize()
        {
            SaveLoadSystem.SerializeLocation(GetSettings());
        }
        private SaveLoadSystem.ILocationSettings GetSettings()
            =>new SaveLoadSystem.LocationSettings_0_4_0(LocationName,Depth,
                CameraMoveLimit, CameraMoveTrigger);
    }
}
