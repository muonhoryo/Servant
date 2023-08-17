
using System;
using UnityEngine;
using Servant.InteractionObjects;
using Servant.Serialization._0_3_0;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.Serialization
{
    [RequireComponent(typeof(LocationTransit))]
    public sealed class SimpleLocationTransitSerComp : 
        SerializationComponent<LocationTransit,SimpleLocationTransitData_0_3_0>
    {
        public override SimpleLocationTransitData_0_3_0 GetDataOfCurrentState()
        {
            return new SimpleLocationTransitData_0_3_0(transform.position, Data.NextMainCharacterPos_, Data.NextLocationName_);
        }
        public override SimpleLocationTransitData_0_3_0 GetSerializationData()
        {
            return Data.Clone() as SimpleLocationTransitData_0_3_0;
        }
        protected override void InitializeAction(SimpleLocationTransitData_0_3_0 data)
        {
            Data = data;
            transform.position = Data.Position_;
            Owner_.SetData(data);
        }
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public sealed class SimpleLocationTransitData_0_3_0 : ISerializableObjectData,
        LocationTransit.ILocationTransitInfo
    {
        public SimpleLocationTransitData_0_3_0() { }
        public SimpleLocationTransitData_0_3_0(Vector2 Position, Vector2 NextMainCharacterPos, string NextLocationName)
        {
            this.Position = Position;
            this.NextMainCharacterPos = NextMainCharacterPos;
            this.NextLocationName = NextLocationName;
        }
        public SimpleLocationTransitData_0_3_0(string json)
        {
            TryInitFromJson(json);
        }
        public Vector2 Position_ => Position;
        public Vector2 NextMainCharacterPos_ => NextMainCharacterPos;
        public string NextLocationName_ => NextLocationName;
        [SerializeField]
        private Vector2 Position;
        [SerializeField]
        private Vector2 NextMainCharacterPos;
        [SerializeField]
        private string NextLocationName;
        private bool TryInitFromJson(string json)
        {
            Position = default;
            NextMainCharacterPos = default;
            NextLocationName = default;
            try
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public int SerializationId_ => 2;
        public object Clone()
        {
            return new SimpleLocationTransitData_0_3_0(Position, NextMainCharacterPos, NextLocationName);
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        void ISerializableObjectData.InstantiateObject()
        {
            this.InstanceAndInitialize<SimpleLocationTransitSerComp>(EnvironmentPrefabs.SimpleLocationTransit_);
        }
        bool ISerializableData.TryInitFromJson(string json) => TryInitFromJson(json);
    }
}
