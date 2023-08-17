
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
        void ISingltone<Initialization>.Destroy() =>
            Destroy(this);

        [SerializeField]
        private GameObject GUICanvas;
        [SerializeField]
        private GUIManagerData GUIData;
        [SerializeField]
        private string MenuSaveName;
        private void Awake()
        {
            this.ValidateSingltone();
            if (GUICanvas == null) throw ServantException.GetNullInitialization("GUICanvas");
            if (GUIData == null) throw ServantException.GetNullInitialization("GUIData");
            GUIManager.GUICanvas = GUICanvas;
            GUIManager.Data = GUIData;
        }
        private void Start()
        {
            SaveLoadSystem.LoadLevel(MenuSaveName);
            Destroy(this);
        }
    }
}
