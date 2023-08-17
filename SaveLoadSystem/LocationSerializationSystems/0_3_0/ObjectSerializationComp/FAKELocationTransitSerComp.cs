


using Servant.InteractionObjects;
using System;
using UnityEngine;
using Servant.Serialization._0_3_0;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.Serialization
{
    [RequireComponent(typeof(TextShower))]
    public sealed class FAKELocationTransitSerComp : SerializationComponent<TextShower, FAKELocationTransitData_0_3_0>
    {
        public override FAKELocationTransitData_0_3_0 GetDataOfCurrentState()
        {
            return new FAKELocationTransitData_0_3_0(Data.ShowedText_, Data.TextShowingOffset_, Data.ShowTime_,
                transform.position);
        }
        public override FAKELocationTransitData_0_3_0 GetSerializationData() =>
            Data.Clone() as FAKELocationTransitData_0_3_0;
        protected override void InitializeAction(FAKELocationTransitData_0_3_0 data)
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
    public sealed class FAKELocationTransitData_0_3_0 : ISerializableObjectData, TextShower.ITextShowerInfo
    {
        public FAKELocationTransitData_0_3_0() { }
        public FAKELocationTransitData_0_3_0(string showedText, Vector2 textShowingOffset, float showTime, Vector2 position)
        {
            ShowedText = showedText;
            TextShowingOffset = textShowingOffset;
            ShowTime = showTime;
            Position = position;
        }
        public FAKELocationTransitData_0_3_0(string json)
        {
            TryInitFromJson(json);
        }
        [SerializeField]
        private string ShowedText;
        [SerializeField]
        private Vector2 TextShowingOffset;
        [SerializeField]
        private float ShowTime;
        [SerializeField]
        private Vector2 Position;
        public string ShowedText_ => ShowedText;
        public Vector2 TextShowingOffset_ => TextShowingOffset;
        public float ShowTime_ => ShowTime;
        public Vector2 Position_ => Position;
        int ISerializableObjectData.SerializationId_ => 3;
        private bool TryInitFromJson(string json)
        {
            ShowedText = default;
            TextShowingOffset = default;
            ShowTime = default;
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

        public object Clone()
        {
            return new FAKELocationTransitData_0_3_0(ShowedText, TextShowingOffset, ShowTime, Position);
        }
        public string ToJson() => JsonUtility.ToJson(this);
        void ISerializableObjectData.InstantiateObject()
        {
            this.InstanceAndInitialize< FAKELocationTransitSerComp>(EnvironmentPrefabs.FAKELocationTransit_);
        }
        bool ISerializableData.TryInitFromJson(string json) => TryInitFromJson(json);
    }
}
