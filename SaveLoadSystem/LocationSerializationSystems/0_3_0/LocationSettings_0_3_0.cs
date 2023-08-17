
using System;
using System.Threading;
using UnityEngine;

namespace Servant.Serialization
{
    public static partial class SaveLoadSystem
    {
        [Serializable]
        public class LocationSettings_0_3_0 : ILocationSettings
        {
            public LocationSettings_0_3_0() { }
            public LocationSettings_0_3_0(string LevelName, Rect CameraMoveLimit,
                Rect CameraMoveTrigger)
            {
                this.LevelName = LevelName;
                this.CameraMoveLimit = CameraMoveLimit;
                this.CameraMoveTrigger = CameraMoveTrigger;
            }
            public LocationSettings_0_3_0(string json)
            {
                TryInitFromJson(json);
            }
            [SerializeField]
            protected string LevelName;
            [SerializeField]
            protected Rect CameraMoveLimit;
            [SerializeField]
            protected Rect CameraMoveTrigger;
            public string LevelName_ => LevelName;
            public Rect CameraMoveLimit_ => CameraMoveLimit;
            public Rect CameraMoveTrigger_ => CameraMoveTrigger;
            public virtual BuildVersion Version_ => BuildVersion.v0_3_0;

            public void Apply()
            {
                void SetCharCtrlState()
                {
                    SetCameraMode();
                }
                AutoResetEvent handler = new AutoResetEvent(false);
                Registry.ThreadManager.AddActionsQueue(SetCharCtrlState, handler);
                handler.WaitOne();
            }
            public string ToJson()
            {
                return JsonUtility.ToJson(this);
            }
            public virtual bool TryInitFromJson(string json)
            {
                LevelName = default;
                CameraMoveLimit = default;
                CameraMoveTrigger = default;
                try
                {
                    JsonUtility.FromJsonOverwrite(json, this);
                }
                catch 
                {
                    return false;
                }
                return true;
            }
            public virtual object Clone()
            {
                return new LocationSettings_0_3_0(LevelName, CameraMoveLimit, CameraMoveTrigger);
            }
            protected virtual void SetCameraMode()
            {
                MainCameraBehaviour.singltone.SetCameraMode_Default();
            }
        }
    }
}
