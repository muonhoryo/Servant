
using System;
using UnityEngine;
using MuonhoryoLibrary;
using Servant.Serialization._0_3_0;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.Serialization
{
    public sealed class SaveInfoContainer : MonoBehaviour, ISerializableObject<SaveInfo_0_3_0>, IResetedEnvinronment,
        ISingltone<SaveInfoContainer>
    {
        private static SaveInfoContainer instance;
        private bool IsInitialized;
        SaveInfoContainer ISingltone<SaveInfoContainer>.Singltone { get => instance; set => instance=value; }
        void ISingltone<SaveInfoContainer>.Destroy() =>
            Destroy(this);

        public int VillainLevel { get; private set; }

        public SaveInfo_0_3_0 SerializedSaveInfo { get; private set; }

        public ISerializableObjectData GetDataOfCurrentState() =>
            new SaveInfo_0_3_0(VillainLevel);
        public ISerializableObjectData GetSerializationData() =>
            SerializedSaveInfo.Clone() as ISerializableObjectData;
        private void InitializeByValidatedData(SaveInfo_0_3_0 data)
        {
            VillainLevel = data.VillainLevel_;
        }

        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, InitializeByValidatedData);
        }
        void ISerializableObject.OnEndLocationLoad() => Awake();
        void ISerializableObject.OnStartLocationLoad()
        {
            EndLocationLoadingEvent += GetDataOfCurrentState().InstantiateObject;
        }
        void IResetedEnvinronment.Reset()
        {
            InitializeByValidatedData(SerializedSaveInfo);
        }
        private void Awake()
        {
            if (!IsInitialized)
            {
                this.ValidateSingltone();
                IsInitialized = true;
            }
        }
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public sealed class SaveInfo_0_3_0 : ISerializableObjectData
    {
        public SaveInfo_0_3_0() { }
        public SaveInfo_0_3_0(int VillainLevel) 
        {
            this.VillainLevel = VillainLevel;
        }
        public SaveInfo_0_3_0(string json)
        {
            TryInitFromJson(json);
        }

        [SerializeField]
        private int VillainLevel;
        public int VillainLevel_ => VillainLevel;
        private bool TryInitFromJson(string json)
        {
            VillainLevel = default;
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
        public int SerializationId_ => 6;
        public object Clone()=> new SaveInfo_0_3_0(VillainLevel);
        public string ToJson()=>JsonUtility.ToJson(this);

        void ISerializableObjectData.InstantiateObject() =>
            this.InstanceAndInitialize<SaveInfoContainer>(EnvironmentPrefabs.SaveInfoContainer_);
        bool ISerializableData.TryInitFromJson(string json) =>
            TryInitFromJson(json);
    }
}
