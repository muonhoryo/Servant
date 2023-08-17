

using Servant.Serialization._0_3_0;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.Serialization
{
    public sealed class WallSerComp:StaticEnvironmentSerComp<WallSerializationData_0_3_0>
    {
        public override ISerializableObjectData GetSerializationData() 
            => new WallSerializationData_0_3_0(transform.position);
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public sealed class WallSerializationData_0_3_0 : StaticEnvironmentEnvSerData_0_3_0
    {
        public WallSerializationData_0_3_0() { }
        public WallSerializationData_0_3_0(Vector2 Position) : base(Position) { }
        public WallSerializationData_0_3_0(string json) : base(json) { }
        public override int SerializationID_ => 4;
        public override object Clone() => new WallSerializationData_0_3_0(Position_);
        protected override void InstantiateObject()
        {
            this.InstanceAndInitialize<WallSerComp>(EnvironmentPrefabs.Wall_);
        }
    }
}
