
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
                LevelName_ = LevelName;
                CameraMoveLimit_ = CameraMoveLimit;
                CameraMoveTrigger_ = CameraMoveTrigger;
            }
            public LocationSettings_0_3_0(string json)
            {
                TryInitFromJson(json);
            }
            public string LevelName { get => LevelName_; set => LevelName_ = value; }
            public Rect CameraMoveLimit { get => CameraMoveLimit_; set => CameraMoveLimit_ = value; }
            public Rect CameraMoveTrigger { get => CameraMoveTrigger_; set => CameraMoveTrigger_ = value; }
            public BuildVersion Version => BuildVersion.v0_3_0;
            [SerializeField]
            private string LevelName_;
            [SerializeField]
            private Rect CameraMoveLimit_;
            [SerializeField]
            private Rect CameraMoveTrigger_;
            public void Apply()
            {
                if (Registry.CharacterController!=null)
                {
                    void SetCharCtrlState()
                    {
                        MainCameraBehavior.singltone.SetCharCtrlState(Registry.CharacterController.transform,
                            CameraMoveLimit, CameraMoveTrigger);
                    }
                    AutoResetEvent handler=new AutoResetEvent(false);
                    Registry.ThreadManager.AddActionsQueue(SetCharCtrlState,handler);
                    handler.WaitOne();
                }
            }
            public string ToJson()
            {
                return JsonUtility.ToJson(this);
            }
            public bool TryInitFromJson(string json)
            {
                LevelName_ = default;
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
            public object Clone()
            {
                return new LocationSettings_0_3_0(LevelName_, CameraMoveLimit_, CameraMoveTrigger_);
            }
        }
    }
}
