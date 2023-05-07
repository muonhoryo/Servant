﻿

using System.Collections.Generic;
using System.Threading;
using System;
using System.Linq;
using UnityEngine;
using Servant.GUI;
using System.Security.Cryptography;

namespace Servant.Serialization
{
    public static partial class SaveLoadSystem
    {
        public static event Action InitializeGameSavingEvent;
        public static event Action StartGameSavingEvent;
        public static event Action EndGameSavingEvent;

        public static event Action InitializeLocationResetingEvent;
        public static event Action StartLocationResetingEvent;
        public static event Action EndLocationResetingEvent;
        private static class CheckPointSystem
        {
            public static void SaveGame(ILocationSettings settings,string fileName)
            {
                new GameAsyncSaver(settings, fileName,GetSerializableObjectsData()).InitializeAndStart();
            }
            public static void ResetLocation()
            {
                new LocationAsyncReseter(GetResetedEnvironment(),GUIManager.Data.ResetLocationScreenPrefab).InitializeAndStart();
            }

            private sealed class GameAsyncSaver:AsyncFacade
            {
                public GameAsyncSaver(in ILocationSettings settings,string FileName,
                    in IEnumerable<ISerializableObjectData> objInSceneData) 
                {
                    if (settings == null)
                        throw ServantException.GetSerializationException("Settings cannot be null. ");
                    if (objInSceneData == null)
                        throw ServantException.GetSerializationException("Havent data to serialize. ");
                    if (FileName == null || FileName.Length == 0)
                        throw ServantException.GetNullOrZeroLengthStringExc("FileName");
                    Settings = settings.Clone() as ILocationSettings;
                    this.FileName= FileName;
                    ObjInSceneData = objInSceneData.Select(obj => obj.Clone() as ISerializableObjectData);
                }
                private readonly ILocationSettings Settings;
                private readonly string FileName;
                private readonly IEnumerable<ISerializableObjectData> ObjInSceneData;
                protected override void Initialize()
                {
                    void OnGameSavingRunningAction()
                    {
                        CurrentThread.Abort();
                        InitializeGameSavingEvent -= OnGameSavingRunningAction;
                        EndGameSavingEvent -= OnEndGameSavingAction;
                    }
                    void OnEndGameSavingAction()
                    {
                        EndGameSavingEvent -= OnEndGameSavingAction;
                        InitializeGameSavingEvent -= OnGameSavingRunningAction;
                    }
                    InitializeGameSavingEvent += OnGameSavingRunningAction;
                    EndGameSavingEvent += OnEndGameSavingAction;
                }
                protected override void AsyncAction()
                {
                    LocationSerializationSystem.SerializeGameSave(Settings, ObjInSceneData, FileName);
                }
                protected override void InitializeAction() =>
                    InitializeGameSavingEvent?.Invoke();
                protected override void StartAction() =>
                    StartGameSavingEvent?.Invoke();
                protected override void EndAction() =>
                    EndGameSavingEvent?.Invoke();
            }
            private sealed class LocationAsyncReseter:LoadingAsyncFacade
            {
                public LocationAsyncReseter(IEnumerable<IResetedEnvinronment> ObjInSceneData,
                    in GameObject resetLocationScreenPrefab)
                    :base(resetLocationScreenPrefab)
                {
                    if (ObjInSceneData == null)
                        throw ServantException.GetSerializationException("Missing objets data. ");
                    this.ObjInSceneData= ObjInSceneData;
                }
                private readonly IEnumerable<IResetedEnvinronment> ObjInSceneData;
                protected override void AsyncAction()
                {
                    RunAsyncAndWait(ResetEnvironment);
                }
                private void ResetEnvironment()
                {
                    var queve = ObjInSceneData.Select<IResetedEnvinronment, Action>(obj => () => obj.Reset());
                    Registry.ThreadManager.AddActionsQueue(queve,Handler);
                }

                protected override void InitializeAction() =>
                    InitializeLocationResetingEvent?.Invoke();
                protected override void StartAction() =>
                    StartLocationResetingEvent?.Invoke();
                protected override void EndAction() =>
                    EndLocationResetingEvent?.Invoke();
            }
        }
    }
}
