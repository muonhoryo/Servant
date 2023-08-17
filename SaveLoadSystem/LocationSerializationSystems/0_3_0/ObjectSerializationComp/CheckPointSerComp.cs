
using UnityEngine;
using Servant.Serialization._0_3_0;
using static Servant.Serialization.SaveLoadSystem;
using System;
using Servant.Serialization;
using Servant.InteractionObjects;

namespace Servant.InteractionObjects
{
    public sealed partial class CheckPoint : ISerializableObject<CheckPointSerData_0_3_0>
    {
        public ISerializableObjectData GetDataOfCurrentState() => GetSerializationData();
        public ISerializableObjectData GetSerializationData() =>
            new CheckPointSerData_0_3_0(transform.position, IsWasActivated_);

        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, objData =>
            {
                transform.position = objData.Position_;
                IsWasActivated_ = objData.IsWasActivated_;
            });
        }
        void ISerializableObject.OnEndLocationLoad() { }
        void ISerializableObject.OnStartLocationLoad() { }
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public sealed class CheckPointSerData_0_3_0 : ISerializableObjectData
    {
        public CheckPointSerData_0_3_0() { }
        public CheckPointSerData_0_3_0(Vector2 Position,bool IsWasActivated = false)
        {
            this.Position = Position;
            this.IsWasActivated = IsWasActivated;
        }
        public CheckPointSerData_0_3_0(string json)
        {
            TryInitFromJson(json);
        }
        [SerializeField]
        private Vector2 Position;
        [SerializeField]
        private bool IsWasActivated;
        public Vector2 Position_ => Position;
        public bool IsWasActivated_ => IsWasActivated;
        public int SerializationId_ => 7;

        public object Clone() => new CheckPointSerData_0_3_0(Position,IsWasActivated);
        public string ToJson() => JsonUtility.ToJson(this);
        private bool TryInitFromJson(string json)
        {
            Position = default;
            IsWasActivated= false;
            try
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
            catch(Exception) 
            {
                return false;
            }
            return true;
        }
        void ISerializableObjectData.InstantiateObject()
        {
            this.InstanceAndInitialize<CheckPoint>(EnvironmentPrefabs.CheckPoint_);
        }
        bool ISerializableData.TryInitFromJson(string json) =>
            TryInitFromJson(json);
    }
}
