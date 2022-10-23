
using System;
using UnityEngine;
using Servant.Serialization;
using System.Threading;

namespace Servant.InteractionObjects
{
    public class LocationTransit : DefaultInteractiveObject
    {
        /*
         SERIALIZATION QUEVE:
         Position
         NextLocationPos
         NextLocationName
        */
        [SerializeField]
        private Vector2 NextLocationPos;
        [SerializeField]
        private string NextLocationName;
        public override void Deserialize(string serializedObject, int dataStart)
        {
        	transform.position=LocationSerializationData.DeserializeVector2
                (LocationSerializationData.GetSubData(serializedObject,dataStart,out dataStart,2));
        	NextLocationPos= LocationSerializationData.DeserializeVector2
                (LocationSerializationData.GetSubData(serializedObject,dataStart+1,out dataStart,2));
        	NextLocationName=LocationSerializationData.GetSubData
                (serializedObject,dataStart+1,out dataStart,isStringData:true);
        }
        public override string Serialize()
        {
            //ID
            return this.GetSerializedId() + LocationSerializationData.SeparateSym +
               //Transit position
               transform.position.SerializeVector2() + LocationSerializationData.SeparateSym +
               //NextLocationPos
               NextLocationPos.SerializeVector2() + LocationSerializationData.SeparateSym +
               //NextLocationName
               LocationSerializationData.QuotesSym+NextLocationName+LocationSerializationData.QuotesSym
               +LocationSerializationData.SeparateSym;
        }
        protected override void Interact()
        {
            LocationSerializationData.EndLocationLoadingEvent += SetMainCharacterPosOnLoad;
            Registry.LoadLocation( NextLocationName);
        }
        private void SetMainCharacterPosOnLoad()
        {
            Registry.CharacterController.transform.position = NextLocationPos;
            LocationSerializationData.EndLocationLoadingEvent -= SetMainCharacterPosOnLoad;
        }
    }
}
