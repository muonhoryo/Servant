using Servant.Serialization;
using Servant.Serialization._0_4_0;
using System;
using UnityEngine;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant
{
    public sealed partial class RadialRockingHookCatcher : ISerializableObject<RadialRockingPointSerData_0_4_0>
    {
        private ISerializableObjectData GetData() =>
            new RadialRockingPointSerData_0_4_0(transform.position, RockingDirection,RockingSpeed);
        ISerializableObjectData ISerializableObject.GetDataOfCurrentState() => GetData();
        ISerializableObjectData ISerializableObject.GetSerializationData() => GetData();
        public void Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, objData =>
            {
                transform.position = objData.Position_;
                RockingDirection =objData.RockingDirection_;
                RockingSpeed =objData.RockingSpeed_;
            });
        }
        public void OnEndLocationLoad() 
        {
            HitPosition = (Vector2)transform.position + HitPositionOffset;
        }
        public void OnStartLocationLoad() { }
    }
}
namespace Servant.Serialization._0_4_0
{
    [Serializable]
    public sealed class RadialRockingPointSerData_0_4_0 : ISerializableObjectData
    {
        public RadialRockingPointSerData_0_4_0() { }
        public RadialRockingPointSerData_0_4_0(Vector2 Position, int RockingDirection,
            float RockingSpeed)
        {
            this.Position = Position;
            this.RockingDirection = RockingDirection;
            this.RockingSpeed = RockingSpeed;
        }
        public RadialRockingPointSerData_0_4_0(string json)
        {
            TryInitFromJson(json);
        }
        [SerializeField]
        private Vector2 Position;
        [SerializeField]
        private int RockingDirection;
        [SerializeField]
        private float RockingSpeed;
        public Vector2 Position_ => Position;
        public int RockingDirection_ => RockingDirection; 
        public float RockingSpeed_  => RockingSpeed;
        public int SerializationId_ => 11;
        public object Clone() =>
            new RadialRockingPointSerData_0_4_0(Position, RockingDirection, RockingSpeed);
        public void InstantiateObject()
        {
            this.InstanceAndInitialize<RadialRockingHookCatcher>(EnvironmentPrefabs.RadialRockingPoint_);
        }
        public string ToJson()=>JsonUtility.ToJson(this);
        public bool TryInitFromJson(string json)
        {
            Position = default;
            RockingDirection = default;
            RockingSpeed = default;
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
