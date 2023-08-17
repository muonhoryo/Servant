
using UnityEngine;
using Servant.Serialization;
using static Servant.Serialization.SaveLoadSystem;
using System;
using Servant.GUI;
using Servant.Serialization._0_3_0;
using UnityEngine.UI;

namespace Servant.Serialization
{
    public sealed class LoadGameButtonSerComp : MonoBehaviour, ISerializableObject<LoadGameButtonSerData_0_3_0>
    {
        public ISerializableObjectData GetSerializationData()=>
            new LoadGameButtonSerData_0_3_0(transform.position);
        public void CheckSavesAvailability()
        {
            GetComponent<Button>().interactable=SaveLoadSystem.IsGameSaved(MainMenuControl.SavedGameName);
        }

        ISerializableObjectData ISerializableObject.GetDataOfCurrentState() =>
            GetSerializationData();
        ISerializableObjectData ISerializableObject.GetSerializationData() =>
            GetSerializationData();
        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, objData =>
            {
                transform.position = objData.Position_;
                CheckSavesAvailability();
            });
        }
        void ISerializableObject.OnEndLocationLoad() { }

        void ISerializableObject.OnStartLocationLoad() { }
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public sealed class LoadGameButtonSerData_0_3_0 : ISerializableObjectData
    {
        public LoadGameButtonSerData_0_3_0() { }
        public LoadGameButtonSerData_0_3_0(Vector2 Position)
        {
            this.Position = Position;
        }
        public LoadGameButtonSerData_0_3_0(string json)
        {
            TryInitFromJson(json);
        }
        [SerializeField]
        private Vector2 Position;
        public Vector2 Position_ => Position;
        public int SerializationId_ => 9;

        public object Clone() => new LoadGameButtonSerData_0_3_0(Position);
        public string ToJson() => JsonUtility.ToJson(this);
        private bool TryInitFromJson(string json)
        {
            Position = default;
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

        void ISerializableObjectData.InstantiateObject()
        {
            this.InstanceAndInitialize<LoadGameButtonSerComp>(EnvironmentPrefabs.LoadGameButton_,
                GUIManager.GUICanvas.transform);
        }

        bool ISerializableData.TryInitFromJson(string json) =>TryInitFromJson(json);
    }
}
