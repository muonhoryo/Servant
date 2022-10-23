
using UnityEngine;
using MuonhoryoLibrary;
using Servant.GUI;
using System.Collections.Generic;
using Servant.Serialization;

namespace Servant
{
    public sealed class Initialization : MonoBehaviour,ISingltone<Initialization>
    {
        private Initialization singltone;
        [SerializeField]
        private GameObject GUICanvas;
        [SerializeField]
        private GameObject LoadingScreenPrefab;
        [SerializeField]
        private GUIManagerData GUIData;
        [SerializeField]
        private List<GameObject> SerializatedObjPrefabs;
        Initialization ISingltone<Initialization>.Singltone
        { get => singltone;
            set => singltone=value; }
        private void Awake()
        {
            this.ValidateSingltone();
            if (GUICanvas == null) throw ServantException.NullInitialization("GUICanvas");
            if (LoadingScreenPrefab == null) throw ServantException.NullInitialization
                    ("LoadingScreenPrefab");
            if (GUIData == null) throw ServantException.NullInitialization("GUIData");
            if (SerializatedObjPrefabs == null) throw ServantException.NullInitialization
                    ("SerializatedObjPrefabs");
            GUIManager.GUICanvas = GUICanvas;
            Registry.LoadingScreenPrefab = LoadingScreenPrefab;
            GUIManager.Data = GUIData;
            LocationSerializationData.SetSerializatedObjsPrefabs(SerializatedObjPrefabs);
            Destroy(this);
        }
    }
}
