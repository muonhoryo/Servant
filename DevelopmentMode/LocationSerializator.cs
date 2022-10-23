
using System.IO;
using UnityEngine;
using Servant.Serialization;
using System.Collections.Generic;

namespace Servant.DevelopmentOnly
{
    public sealed class LocationSerializator : MonoBehaviour
    {
        [SerializeField]
        private string LocationName;
        [ContextMenu("Serialize")]
        void Serialize()
        {
            string path = LocationSerializationData.GetSerializationPath(LocationName);
            using(StreamWriter writer= new StreamWriter(path, false))
            {
                IEnumerator<string> list = LocationSerializationData.GetLocationSerialization();
                while (list.MoveNext())
                {
                    writer.WriteLine(list.Current);
                }
            }
        }
    }
}
