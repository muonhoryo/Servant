using Servant.DevelopmentOnly;
using Servant.Serialization._0_3_0;
using Servant.Serialization._0_4_0;
using Servant.Serialization._0_5_0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant.Serialization
{
    public static partial class SaveLoadSystem
    {
        private sealed class LocationSerializator_0_5_0 : LocationSerializator
        {
            public LocationSerializator_0_5_0() : base() { }
            public LocationSerializator_0_5_0(ILocationSettings locationSettings, ISerializableObjectData[] data) :
                base(locationSettings, data)
            { }
            public override BuildVersion Version_ => BuildVersion.v0_5_0;
            protected override ILocationSettings GetSettings() =>
                new LocationSettings_0_4_0();
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
                    ()=>new LoadGameButtonSerData_0_3_0(),
                    ()=>new ClinePlatformSerData_0_4_0(),
                    ()=>new RadialRockingPointSerData_0_4_0(),
                    ()=>new DeathAreaSerData_0_4_0(),
                    ()=>new GuardAndroidSerData_0_5_0()
                };

        }
    }
}
