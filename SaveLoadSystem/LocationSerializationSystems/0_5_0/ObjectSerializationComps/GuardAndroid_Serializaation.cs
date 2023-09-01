using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servant.Characters;
using Servant.Serialization;
using Servant.Serialization._0_5_0;
using UnityEngine;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.Characters
{
    public partial class GuardAndroid : ISerializableObject<GuardAndroidSerData_0_5_0>,IResetedEnvinronment
    {
        private GuardAndroidSerData_0_5_0 SerializationData;
        private void InitializeFromAvailibleData()
        {
            transform.position = SerializationData.Position_;
        }

        ISerializableObjectData ISerializableObject.GetDataOfCurrentState() =>
            new GuardAndroidSerData_0_5_0(transform.position);
        ISerializableObjectData ISerializableObject.GetSerializationData() => SerializationData;
        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data,
                (objData) =>
                {
                    SerializationData= objData;
                    InitializeFromAvailibleData();
                });
        }
        void ISerializableObject.OnEndLocationLoad() =>
            enabled = true;
        void ISerializableObject.OnStartLocationLoad() => enabled = false;
        void IResetedEnvinronment.Reset()
        {
            if(SerializationData==null)
                throw ServantException.GetArgumentNullException("data");
            InitializeFromAvailibleData();
        }
    }
}
namespace Servant.Serialization._0_5_0 
{
    [Serializable]
    public sealed class GuardAndroidSerData_0_5_0 : ISerializableObjectData
    {
        public GuardAndroidSerData_0_5_0() { }
        public GuardAndroidSerData_0_5_0(Vector2 Position)
        {
            this.Position = Position;
        }
        public GuardAndroidSerData_0_5_0(string json)
        {
            TryInitFromJson(json);
        }
        public Vector2 Position_ { get => Position; set => Position = value; }
        [SerializeField]
        private Vector2 Position;

        public bool TryInitFromJson(string json)
        {
            Position = default;
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

        int ISerializableObjectData.SerializationId_ => 13;
        public object Clone() => new GuardAndroidSerData_0_5_0(Position);
        public void InstantiateObject()
        {
            this.InstanceAndInitialize<GuardAndroid>(EnvironmentPrefabs.GuardAndroid_);
        }
        public string ToJson() => JsonUtility.ToJson(this);
    }
}
