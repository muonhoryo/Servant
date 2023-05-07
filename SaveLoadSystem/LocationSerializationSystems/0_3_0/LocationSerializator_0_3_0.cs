


using System;
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
        private sealed class LocationSerializator_0_3_0 : LocationSerializator
        {
            public LocationSerializator_0_3_0() : base() { }

            public override BuildVersion Version => BuildVersion.v0_3_0;
            protected override ILocationSettings GetSettings() => new LocationSettings_0_3_0();
            protected override Func<ISerializableObjectData>[] ObjDataGetters =>
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
        }
    }
}
