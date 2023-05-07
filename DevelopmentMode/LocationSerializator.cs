
using UnityEngine;
using Servant.Serialization;

namespace Servant.DevelopmentOnly
{
    public sealed class LocationSerializator : MonoBehaviour
    {
        [SerializeField]
        private string LocationName;
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
            => new SaveLoadSystem.LocationSettings_0_3_0(LocationName,CameraMoveLimit,CameraMoveTrigger);
    }
}
