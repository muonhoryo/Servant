using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.MainCameraBehaviour;

namespace Servant.Serialization
{
    public static partial class SaveLoadSystem
    {
        [Serializable]
        public class LocationSettings_0_4_0 : LocationSettings_0_3_0 
        {
            public LocationSettings_0_4_0() : base() { }
            public LocationSettings_0_4_0(string LevelName,float Depth,Rect MoveLimit,Rect MoveTrigger):
                base(LevelName,MoveLimit,MoveTrigger)
            {
                this.Depth = Depth;
            }
            public LocationSettings_0_4_0(string json) : base(json) { }

            [SerializeField]
            protected float Depth;
            public float Depth_ => Depth;
            public override BuildVersion Version_ => BuildVersion.v0_4_0;
            public override object Clone() =>
                new LocationSettings_0_4_0(LevelName, Depth, CameraMoveLimit, CameraMoveTrigger);
            public override bool TryInitFromJson(string json)
            {
                LevelName = default;
                Depth= default;
                CameraMoveLimit= default;
                CameraMoveTrigger= default;
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
            protected override void SetCameraMode()
            {
                singltone.SetCameraMode_CharacterTracking(Registry.CharacterController_.transform,
                    CameraMoveLimit, CameraMoveTrigger, Depth);
            }
        }
    }
}
