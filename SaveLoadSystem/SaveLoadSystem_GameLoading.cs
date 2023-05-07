
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Servant.GUI;

namespace Servant.Serialization
{
    public static partial class SaveLoadSystem
    {
        public static event Action InitializeLocationUnloadingEvent;
        public static event Action StartLocationUnloadingEvent;
        public static event Action EndLocationUnloadingEvent;
        public static event Action StartLocationLoadingEvent;
        public static event Action<IEnumerable<ISerializableObjectData>> GettingDeserializedDataEvent;
        public static event Action EndLocationLoadingEvent;
        private static class LocationLoadingSystem
        {
            public static void LoadLocation(string locationFilePath, GameObject loadingScreenPrefab)
            {
                new LocationAsyncLoader(locationFilePath, loadingScreenPrefab).InitializeAndStart();
            }

            private sealed class LocationAsyncLoader : LoadingAsyncFacade
            {
                /// <summary>
                /// Create loading screen at gui transform
                /// </summary>
                /// <param name="LocationName"></param>
                /// <param name="loadingScreenPrefab"></param>
                public LocationAsyncLoader(string FilePath, in GameObject loadingScreenPrefab)
                    : base(loadingScreenPrefab)
                {
                    if (FilePath == null || FilePath.Length == 0)
                        throw ServantException.GetNullOrZeroLengthStringExc("FilePath");
                    this.FilePath = FilePath;
                }
                private readonly string FilePath;
                private LocationSerializator Serializator;
                protected override void AsyncAction()
                {
                    RunAsyncAndWait(DestroyCurrentObjects);
                    RunAsyncAndWait(ExecuteEndUnloadingEvent);
                    RunAsyncAndWait(ExecuteStartLoadingEvent);
                    RunAsyncAndWait(DeserializeLocationData);
                    RunAsyncAndWait(ExecuteGettingDataEvent);
                    RunAsyncAndWait(InstantiateObjects);
                    RunAsyncAndWait(ApplySettings);
                }
                protected override void InitializeAction() =>
                    InitializeLocationUnloadingEvent?.Invoke();
                protected override void StartAction() =>
                    StartLocationUnloadingEvent?.Invoke();
                protected override void EndAction() =>
                    EndLocationLoadingEvent?.Invoke();
                //Async methods
                private void DestroyCurrentObjects()
                {
                    List<ISerializableObject> objectsInScene = GetSerializedObjsInSceneAsync();
                    Registry.ThreadManager.AddActionsQueue
                        (objectsInScene.Select<ISerializableObject, Action>
                        (obj => () => GameObject.Destroy((obj as MonoBehaviour).gameObject)), Handler);
                }
                private void ExecuteEndUnloadingEvent() => DelegateEventExecutingToTM(EndLocationUnloadingEvent);
                private void ExecuteStartLoadingEvent() => DelegateEventExecutingToTM(StartLocationLoadingEvent);
                private void DeserializeLocationData()
                {
                    Serializator = LocationSerializationSystem.DeserializeData(FilePath);
                    Handler.Set();
                }
                private void ExecuteGettingDataEvent() 
                {
                    if (GettingDeserializedDataEvent != null)
                    {
                        DelegateEventExecutingToTM(() => 
                        { GettingDeserializedDataEvent(Serializator.DeserializedObjData); });
                    }
                    else
                        Handler.Set();
                } 
                private void InstantiateObjects()
                {
                    IEnumerable<Action> actionQueve =
                        Serializator.DeserializedObjData.Select<ISerializableObjectData, Action>
                        (data => () => data.InstantiateObject());
                    Registry.ThreadManager.AddActionsQueue(actionQueve, Handler);
                }
                private void ApplySettings()
                {
                    Serializator.DeserializedSettings.Apply();
                    Registry.SettingsOfCurrentLocation = Serializator.DeserializedSettings;
                    Handler.Set();
                }
            }
        }
    }
}
