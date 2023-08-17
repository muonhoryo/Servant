


using System;
using System.Linq;
using System.Collections.Generic;
using static Servant.Serialization.SaveLoadSystem;
using UnityEngine;
using MuonhoryoLibrary.Serialization;
using System.IO;
using Servant.Serialization._0_3_0;

namespace Servant.Serialization
{
    public static partial class SaveLoadSystem
    {
        private sealed class LocationSerializator_0_3_0 : LocationSerializator,ILocationSerilConverter<LocationSerializator_0_4_0>
        {
            public LocationSerializator_0_3_0() : base() { }
            public LocationSerializator_0_3_0(ILocationSettings locationSettings, ISerializableObjectData[] data):
                base(locationSettings, data){ }

            public override BuildVersion Version_ => BuildVersion.v0_3_0;
            protected override ILocationSettings GetSettings() => new LocationSettings_0_3_0();

            protected override Func<ISerializableObjectData>[] ObjDataGetters_ =>
                new Func<ISerializableObjectData>[]
                {
                    ()=>new MainCharacterData_0_3_0(),
                    ()=>new NonePassablePlatformData_0_3_0(),
                    ()=>new SimpleLocationTransitData_0_3_0(),
                    ()=>new FAKELocationTransitData_0_3_0(),
                    ()=>new WallSerializationData_0_3_0(),
                    ()=>new MovableBoxSerData_0_3_0(),
                    ()=>new SaveInfo_0_3_0(),
                    ()=>new CheckPointSerData_0_3_0(),
                    ()=>new StartButtonSerData_0_3_0(),
                    ()=>new LoadGameButtonSerData_0_3_0()
                };
            BuildVersion ILocationSerilConverter<LocationSerializator_0_4_0>.OutputVersion_ => BuildVersion.v0_4_0;
            LocationSerializator_0_4_0 ILocationSerilConverter<LocationSerializator_0_4_0>.Convert()
            {
                var parsedSettings = DeserializedSettings_ as LocationSettings_0_3_0;
                var data_ = (ISerializableObjectData[])DeserializedObjData_.Clone();
                parsedSettings = new LocationSettings_0_4_0(parsedSettings.LevelName_, default, parsedSettings.CameraMoveLimit_,
                    parsedSettings.CameraMoveTrigger_);
                LocationSerializator_0_4_0 serializator = new LocationSerializator_0_4_0(parsedSettings, data_);
                return serializator;
            }
        }
    }
}
