using Servant.Serialization;
using Servant.Serialization._0_4_0;
using System;
using UnityEngine;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant
{
    public sealed partial class DeathArea : ISerializableObject<DeathAreaSerData_0_4_0>
    {
        private ISerializableObjectData GetData()=>
            new DeathAreaSerData_0_4_0(transform.position,transform.localScale);
        public ISerializableObjectData GetDataOfCurrentState() => GetData();
        public ISerializableObjectData GetSerializationData() => GetData();
        public void Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data,
                (info) =>
                {
                    transform.position = info.Position_;
                    transform.localScale = info.Size_;
                });
        }
        public void OnEndLocationLoad() { }
        public void OnStartLocationLoad() { }
    }
}
namespace Servant.Serialization._0_4_0
{
    [Serializable]
    public sealed class DeathAreaSerData_0_4_0 : ISerializableObjectData
    {
        public DeathAreaSerData_0_4_0() { }
        public DeathAreaSerData_0_4_0(Vector2 Position,Vector2 Size)
        {
            this.Position = Position;
            this.Size= Size;
        }
        public DeathAreaSerData_0_4_0(string json)
        {
            TryInitFromJson(json);
        }
        [SerializeField]
        private Vector2 Position;
        [SerializeField]
        private Vector2 Size;
        public Vector2 Position_ => Position;
        public Vector2 Size_ => Size;
        public int SerializationId_ => 12;
        public object Clone()=>new DeathAreaSerData_0_4_0(Position, Size);
        public void InstantiateObject()
        {
            this.InstanceAndInitialize<DeathArea>(EnvironmentPrefabs.DeathArea_);
        }
        public string ToJson() =>
            JsonUtility.ToJson(this);
        public bool TryInitFromJson(string json)
        {
            Position = default;
            Size= default;
            try
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
