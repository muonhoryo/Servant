
using UnityEngine;
using MuonhoryoLibrary;
using Servant.GUI;
using System.Collections.Generic;
using Servant.Serialization;
using Servant.Control;
using System.IO;
using System.Threading;

namespace Servant
{
    public sealed class Initialization : MonoBehaviour,ISingltone<Initialization>
    {
        private static Initialization singltone;
        Initialization ISingltone<Initialization>.Singltone
        {
            get => singltone;
            set => singltone = value;
        }
        [SerializeField]
        private GameObject GUICanvas;
        [SerializeField]
        private GUIManagerData GUIData;
        [SerializeField]
        private GameObject GarpoonBasePrefab;
        [SerializeField]
        private string MenuSaveName;
        private void Awake()
        {
            this.ValidateSingltone();
            if (GUICanvas == null) throw ServantException.GetNullInitialization("GUICanvas");
            if (GUIData == null) throw ServantException.GetNullInitialization("GUIData");
            if (GarpoonBasePrefab == null) throw ServantException.GetNullInitialization("GarpoonBasePrefab");
            GUIManager.GUICanvas = GUICanvas;
            GUIManager.Data = GUIData;
            MainCharacterController.GarpoonBasePrefab = GarpoonBasePrefab;
        }
        private void Start()
        {
            SaveLoadSystem.LoadLevel(MenuSaveName);
            Destroy(this);
        }
    }
}
