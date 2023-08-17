

using System;
using UnityEngine;
using MuonhoryoLibrary;
using Servant.Control;
using Servant.Serialization._0_3_0;
using static Servant.Serialization.SaveLoadSystem;
using Servant.Serialization;

namespace Servant.Characters
{
    public sealed partial class HumanCharacter :  ISerializableObject<MainCharacterData_0_3_0>, IResetedEnvinronment
    {
        private MainCharacterData_0_3_0 serializationData;
        private void InitializeFromAvailibleData()
        {
            transform.position = serializationData.Position_;
            IsLeftSide_ = serializationData.IsLeftSide_;
        }


        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, objData => 
            {
                serializationData = objData;
                InitializeFromAvailibleData();
            });
        }
        public ISerializableObjectData GetSerializationData()=>
            serializationData.Clone() as MainCharacterData_0_3_0;
        public ISerializableObjectData GetDataOfCurrentState()=>
            new MainCharacterData_0_3_0(transform.position, IsLeftSide_);
        void ISerializableObject.OnStartLocationLoad() => enabled = false;
        void ISerializableObject.OnEndLocationLoad() => enabled = true;

        void IResetedEnvinronment.Reset()
        {
            if (serializationData == null)
                throw ServantException.GetArgumentNullException("data");
            InitializeFromAvailibleData();
        }
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public sealed class MainCharacterData_0_3_0 : ISerializableObjectData
    {
        public MainCharacterData_0_3_0() { }
        public MainCharacterData_0_3_0(Vector2 Position, bool IsLeftSide)
        {
            this.Position = Position;
            this.IsLeftSide = IsLeftSide;
        }
        public MainCharacterData_0_3_0(string json)
        {
            TryInitFromJson(json);
        }
        public Vector2 Position_ { get => Position; private set => Position = value; }
        [SerializeField]
        private Vector2 Position;
        public bool IsLeftSide_ { get => IsLeftSide; private set => IsLeftSide = value; }
        [SerializeField]
        private bool IsLeftSide;

        public int SerializationId_ => 0;
        public object Clone()
        {
            return new MainCharacterData_0_3_0(Position, IsLeftSide);
        }
        public void InstantiateObject()
        {
            this.InstanceAndInitialize<Characters.HumanCharacter>(EnvironmentPrefabs.HumanCharacterPrefab_);
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        public bool TryInitFromJson(string json)
        {
            Position = default;
            IsLeftSide_ = default;
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
    }
}
