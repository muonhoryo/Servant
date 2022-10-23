
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Servant.Serialization;

namespace Servant.DevelopmentOnly
{
    public sealed class LocationSerializationManager : MonoBehaviour
    {
        public string LocationName;
        public void SerializeLocation()
        {
            string path =LocationSerializationData.LocationSerializationPath + LocationName+
                LocationSerializationData.FileType;
            if (!File.Exists(path)) File.Create(path);
            using(StreamWriter writer=new StreamWriter(path,false))
            {
                IEnumerator<string> enumerator = LocationSerializationData.GetLocationSerialization();
                while (enumerator.MoveNext())
                {
                    writer.Write(enumerator.Current);
                }
            }
        }
    }
}
