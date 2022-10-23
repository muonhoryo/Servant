
using UnityEngine;
using Servant.GUI;
using Servant.Serialization;

namespace Servant.InteractionObjects
{
    public sealed class TextShower : DefaultInteractiveObject
    {
        /// <summary>
        /// SERIALIZATION QUEVE:
        /// Transform.position
        /// TextShowingOffset
        /// ShowedText
        /// ShowTime
        /// </summary>
        [SerializeField]
        private Vector2 TextShowingOffset;
        [SerializeField]
        private string ShowedText;
        [SerializeField]
        [Range(0, 1000)]
        private float ShowTime;
        public override void Deserialize(string serializedObject, int dataStart)
        {
            transform.position = LocationSerializationData.DeserializeVector2
                (LocationSerializationData.GetSubData(serializedObject, dataStart, out dataStart, 2));
            TextShowingOffset = LocationSerializationData.DeserializeVector2
                (LocationSerializationData.GetSubData(serializedObject, dataStart + 1, out dataStart, 2));
            ShowedText = LocationSerializationData.GetSubData(serializedObject, dataStart + 1,
                out dataStart, 1);
            if(!float.TryParse(LocationSerializationData.GetSubData(serializedObject,dataStart+1,
                out dataStart),out ShowTime))
            {
                throw ServantException.SerializationException();
            }
        }
        public override string Serialize()
        {
            //Id
            return this.GetSerializedId() + LocationSerializationData.SeparateSym +
                //transform.position
                transform.position.SerializeVector2() + LocationSerializationData.SeparateSym +
                //TextShowingOffset
                TextShowingOffset.SerializeVector2() + LocationSerializationData.SeparateSym +
                //ShowedText
                LocationSerializationData.QuotesSym+ShowedText+LocationSerializationData.QuotesSym 
                + LocationSerializationData.SeparateSym +
                //ShowTime
                ShowTime + LocationSerializationData.SeparateSym;
        }
        protected override void Interact()
        {
            GUIManager.InitializeNewTempleText((Vector2)transform.position+TextShowingOffset,
                ShowedText, ShowTime);
        }
    }
}
