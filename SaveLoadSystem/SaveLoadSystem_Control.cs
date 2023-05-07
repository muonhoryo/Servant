


using System.Collections.Generic;
using System;
using UnityEngine;
using Servant.GUI;
using System.Threading;

namespace Servant.Serialization
{
    public static partial class SaveLoadSystem
    {
        /// <summary>
        /// DEV-ONLY
        /// </summary>
        /// <param name="settings"></param>
        public static void SerializeLocation(ILocationSettings settings)
        {
            LocationSerializationSystem.SerializeLevel(settings, GetDataOfCurrentSerObjsState());
        }
        public static void SaveGame(ILocationSettings settings,string saveName)
        {
            CheckPointSystem.SaveGame(settings, saveName);
        }
        /// <summary>
        /// Used in transition between locations. 
        /// If loadingScreenPrefab is null, use default loading screen
        /// </summary>
        /// <param name="locationFilePath"></param>
        /// <param name="loadingScreenPrefab"></param>
        public static void LoadLevel(string levelName,GameObject loadingScreenPrefab=null)
        {
            LoadLocation(LocationSerializationSystem.GetLevelSerializationPath(levelName), loadingScreenPrefab);
        }
        /// <summary>
        /// Used in save loading. 
        /// If loadingScreenPrefab is null, use default loading screen
        /// </summary>
        /// <param name="locationFilePath"></param>
        /// <param name="loadingScreenPrefab"></param>
        public static void LoadSavedGame(string saveName, GameObject loadingScreenPrefab = null)
        {
            LoadLocation(LocationSerializationSystem.GetSavedGameSerPath(saveName), loadingScreenPrefab);
        }
        /// <summary>
        /// If loadingScreenPrefab is null, use default loading screen
        /// </summary>
        /// <param name="locationFilePath"></param>
        /// <param name="loadingScreenPrefab"></param>
        public static void LoadLocation(string locationFilePath,GameObject loadingScreenPrefab=null)
        {
            if(loadingScreenPrefab==null)
                loadingScreenPrefab=GUIManager.Data.LoadingScreenPrefab;
            LocationLoadingSystem.LoadLocation(locationFilePath, loadingScreenPrefab);
        }
        public static void ResetLocation()
        {
            CheckPointSystem.ResetLocation();
        }

        public abstract class LoadingAsyncFacade : AsyncFacade
        {
            /// <summary>
            /// Create screen at gui transform
            /// </summary>
            /// <param name="LocationName"></param>
            /// <param name="loadingScreenPrefab"></param>
            protected LoadingAsyncFacade(GameObject LoadingScreenPrefab)
            {
                if (LoadingScreenPrefab == null)
                    throw ServantException.GetSerializationException("Missing loadingScreenPrefab. ");
                LoadingScreen = GameObject.Instantiate(LoadingScreenPrefab, GUIManager.GUICanvas.transform);
            }
            protected readonly GameObject LoadingScreen;
            protected sealed override void Initialize()
            {
                TurnOnLoadingSerializatedObjs();
                void OnEndLocationLoadingAction()
                {
                    TurnOffLoadingSerializatedObjs();
                    GameObject.Destroy(LoadingScreen);
                    EndEvent -= OnEndLocationLoadingAction;
                }
                EndEvent += OnEndLocationLoadingAction;
                Initialize_();
            }
            protected virtual void Initialize_() { }
        }

        private static void TurnOnLoadingSerializatedObjs()
            => OperateWithSerializatedObjs((obj) => obj.OnStartLocationLoad());
        private static void TurnOffLoadingSerializatedObjs()
            => OperateWithSerializatedObjs((obj) => obj.OnEndLocationLoad());
    }
}
