using Servant.Serialization._0_3_0;
using Servant.Serialization._0_4_0;
using System;
using UnityEngine;

namespace Servant.Serialization
{
    public sealed class ClinePlatformSerComp : StaticEnvironmentSerComp<ClinePlatformSerData_0_4_0>
    {
        public override SaveLoadSystem.ISerializableObjectData GetSerializationData() =>
            new ClinePlatformSerData_0_4_0(transform.position);
    }
}
namespace Servant.Serialization._0_4_0
{
    [Serializable]
    public sealed class ClinePlatformSerData_0_4_0 : StaticEnvironmentEnvSerData_0_3_0
    {
        public ClinePlatformSerData_0_4_0() { }
        public ClinePlatformSerData_0_4_0(Vector2 Position):base(Position) { }
        public ClinePlatformSerData_0_4_0(string json):base(json) { }
        public override int SerializationID_ => 10;

        public override object Clone() => new ClinePlatformSerData_0_4_0(Position_);
        protected override void InstantiateObject()
        {
            this.InstanceAndInitialize<ClinePlatformSerComp>(EnvironmentPrefabs.ClinePlatform_);
        }
    }
}
