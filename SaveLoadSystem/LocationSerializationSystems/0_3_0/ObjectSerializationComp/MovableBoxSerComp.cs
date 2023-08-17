
using Servant.Serialization._0_3_0;
using System;
using UnityEngine;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.Serialization
{
    public sealed class MovableBoxSerComp : MovableEnvironmentSerComp<MovableBoxSerData_0_3_0>
    {
        public override ISerializableObjectData GetDataOfCurrentState() =>
            new MovableBoxSerData_0_3_0(transform.position, transform.eulerAngles.z);
        public override ISerializableObjectData GetSerializationData() =>
            Data_.Clone() as MovableBoxSerData_0_3_0;
    }
}
namespace Servant.Serialization._0_3_0 
{
    [Serializable]
    public sealed class MovableBoxSerData_0_3_0 : MovableEnvironmentSerData_0_3_0
    {
        public MovableBoxSerData_0_3_0() { }
        public MovableBoxSerData_0_3_0(Vector2 Position,float Rotation):base(Position,Rotation) { }
        public MovableBoxSerData_0_3_0(string json) : base(json) { }
        public override int SerializationId_ => 5;
        public override object Clone() =>
            new MovableBoxSerData_0_3_0(Position_, Rotation_);
        protected override void InstantiateObject()
        {
            this.InstanceAndInitialize<MovableBoxSerComp>(EnvironmentPrefabs.MovableBox_);
        }
    }
}
