
using UnityEngine;
using Servant.Serialization._0_3_0;
using Servant.GUI;

namespace Servant.Serialization
{
    public sealed class StartButtonSerComp : StaticEnvironmentSerComp<StartButtonSerData_0_3_0>
    {
        public override SaveLoadSystem.ISerializableObjectData GetSerializationData() =>
            new StartButtonSerData_0_3_0(transform.position);
    }
}
namespace Servant.Serialization._0_3_0
{
    public sealed class StartButtonSerData_0_3_0 : StaticEnvironmentEnvSerData_0_3_0
    {
        public StartButtonSerData_0_3_0() { }
        public StartButtonSerData_0_3_0(Vector2 Position) : base(Position) { }
        public StartButtonSerData_0_3_0(string json):base(json) { }
        public override int SerializationID_ => 8;
        public override object Clone() =>
            new StartButtonSerData_0_3_0(Position_);
        protected override void InstantiateObject()
        {
            this.InstanceAndInitialize<StartButtonSerComp>
                (EnvironmentPrefabs.StartButton_, GUIManager.GUICanvas.transform);
        }
    }
}
