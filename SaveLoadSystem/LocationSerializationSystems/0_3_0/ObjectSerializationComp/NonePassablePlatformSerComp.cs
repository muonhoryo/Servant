

using System;
using UnityEngine;
using Servant.Serialization._0_3_0;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.Serialization
{
    public sealed class NonePassablePlatformSerComp : StaticEnvironmentSerComp<NonePassablePlatformData_0_3_0>
    {
        public override ISerializableObjectData GetSerializationData()=>
            new NonePassablePlatformData_0_3_0(transform.position);
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public sealed class NonePassablePlatformData_0_3_0 : StaticEnvironmentEnvSerData_0_3_0
    {
        public NonePassablePlatformData_0_3_0() { }
        public NonePassablePlatformData_0_3_0(Vector2 Position) : base(Position) { }
        public NonePassablePlatformData_0_3_0(string json) : base(json) { }
        public override int SerializationID => 1;
        public override object Clone() =>
            new NonePassablePlatformData_0_3_0(Position_);
        protected override void InstantiateObject()
        {
            this.InstanceAndInitialize<NonePassablePlatformSerComp>(EnvironmentPrefabs.NonePassablePlatform);
        }
    }
}
