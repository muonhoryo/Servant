
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
        public static event Action InitializeLocationUnloadingEvent=delegate { };
        public static event Action StartLocationUnloadingEvent=delegate { };
        public static event Action EndLocationUnloadingEvent=delegate { };
        public static event Action StartLocationLoadingEvent=delegate { };
        public static event Action<IEnumerable<ISerializableObjectData>> GettingDeserializedDataEvent=delegate { };
        public static event Action EndLocationLoadingEvent=delegate { };
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
                    InitializeLocationUnloadingEvent();
                protected override void StartAction() =>
                    StartLocationUnloadingEvent();
                protected override void EndAction() =>
                    EndLocationLoadingEvent();
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
                private void ExecuteGettingDataEvent()=>
                    DelegateEventExecutingToTM(() =>
                    { GettingDeserializedDataEvent(Serializator.DeserializedObjData_); });
                private void InstantiateObjects()
                {
                    IEnumerable<Action> actionQueve =
                        Serializator.DeserializedObjData_.Select<ISerializableObjectData, Action>
                        (data => () => data.InstantiateObject());
                    Registry.ThreadManager.AddActionsQueue(actionQueve, Handler);
                }
                private void ApplySettings()
                {
                    Serializator.DeserializedSettings_.Apply();
                    Registry.SettingsOfCurrentLocation = Serializator.DeserializedSettings_;
                    Handler.Set();
                }
            }
        }
    }
}
