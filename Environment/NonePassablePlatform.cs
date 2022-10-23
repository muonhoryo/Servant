

namespace Servant.Serialization
{
    public sealed class NonePassablePlatform : SerializedObject
    {
        public override void Deserialize(string serializedObject, int dataStart)
        {
            transform.position = LocationSerializationData.DeserializeVector2
                (LocationSerializationData.GetSubData(serializedObject, dataStart, out dataStart, 2));
        }
        public override string Serialize()
        {
            return this.GetSerializedId() + LocationSerializationData.SeparateSym +
                transform.position.SerializeVector2() + LocationSerializationData.SeparateSym;
        }
    }
}
