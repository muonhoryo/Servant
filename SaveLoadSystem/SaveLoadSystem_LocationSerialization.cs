
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.Serialization
{
    public static partial class SaveLoadSystem
    {
        public const string CompanyName = "ServantTeam";
        public const string ProductName = "Servant";
        public const string ServantSerializationTag = "SerializableObject";
        public const char ObjectHeaderSym = ':';

        public static List<ISerializableObject> GetSerializableObjects()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag
                (ServantSerializationTag);
            List<ISerializableObject> serializedObjects = new List<ISerializableObject>(objects.Length);
            foreach (GameObject obj in objects)
            {
                serializedObjects.Add(obj.GetComponent<ISerializableObject>());
            }
            return serializedObjects;
        }
        public static IEnumerable<ISerializableObjectData> GetSerializableObjectsData()
        {
            List<ISerializableObject> list = GetSerializableObjects();
            return list.Select(obj => obj.GetSerializationData());
        }
        public static IEnumerable<ISerializableObjectData> GetDataOfCurrentSerObjsState()
        {
            List<ISerializableObject> list = GetSerializableObjects();
            return list.Select(obj => obj.GetDataOfCurrentState());
        }
        public static List<IResetedEnvinronment> GetResetedEnvironment()
        {
            List<IResetedEnvinronment> list = new List<IResetedEnvinronment>();
            void Operation(ISerializableObject obj)
            {
                if(obj is IResetedEnvinronment env)
                    list.Add(env);
            }
            OperateWithSerializatedObjs(Operation);
            return list;
        }
        public static void OperateWithSerializatedObjs(Action<ISerializableObject> operation)
        {
            List<ISerializableObject> list = GetSerializableObjects();
            foreach (var obj in list)
            {
                operation(obj);
            }
        }
        public static bool IsGameSaved(string savedGameName)
        {
            return File.Exists(LocationSerializationSystem.GetSavedGameSerPath(savedGameName));
        }
        public static bool IsLevelSaved(string savedLevelName)
        {
            return File.Exists(LocationSerializationSystem.GetLevelSerializationPath(savedLevelName));
        }
        private static class LocationSerializationSystem
        {
            public const string LevelSerializationPath = "Assets/Scripts/SaveLoadSystem/SerializedLevels/";
            public static string GameSaveSerializationPath_ =>
                $"AppData/LocalLow/{CompanyName}/{ProductName}/Saves/";
            public const string FileType = ".json";
            public const string TemplePostfix = "_tmpl";
            private static LocationSerializator GetActualSerializator() => new LocationSerializator_0_4_0();



            public static void Serialize(ILocationSettings settings,IEnumerable<ISerializableObjectData> dataQueve,
                string fileName,string fileDir)
            {
                string templPath = GetTempleSerPath(fileName, fileDir);
                string path = GetSerializationPath(fileName, fileDir);
                GetActualSerializator().Serialize(path,templPath, dataQueve, settings);
            }
            public static void SerializeLevel(ILocationSettings settings,IEnumerable<ISerializableObjectData> dataQueve)
            {
                Serialize(settings,dataQueve, settings.LevelName_, LevelSerializationPath);
            }
            public static void SerializeGameSave(ILocationSettings settings,IEnumerable<ISerializableObjectData> dataQueve,
                string fileName)
            {
                Serialize(settings,dataQueve,fileName,GameSaveSerializationPath_);
            }
            public static LocationSerializator DeserializeData(string serializationPath)
            {
                if (!File.Exists(serializationPath))
                    throw ServantException.GetSerializationException($"File at {serializationPath} does not exist. ");
                using (StreamReader reader = new StreamReader(serializationPath))
                {
                    List<string> serializedLocation = new List<string>() { };
                    while (reader.Peek() != -1)
                    {
                        serializedLocation.Add(reader.ReadLine());
                    }
                    BuildVersion ver = JsonUtility.FromJson<BuildVersion>(serializedLocation[0]);
                    if (ver == default)
                        throw ServantException.GetSerializationException("Cannot get info about serialization version. ");
                    LocationSerializator serializator = GetActualSerializator();
                    if (ver != serializator.Version_)
                    {
                        serializator= GetOldVersionSerializator(ver);
                    }
                    serializator.DeserializeDataAndSettings(serializedLocation);
                    return serializator;
                }
            }
            public static LocationSerializator DeserializeLevelData(string fileName)
            {
                return DeserializeData(GetSerializationPath(fileName, LevelSerializationPath));
            }
            public static LocationSerializator DeserializeGameSaveData(string fileName)
            {
                return DeserializeData(GetSerializationPath(fileName, GameSaveSerializationPath_));
            }

            private static LocationSerializator GetOldVersionSerializator(BuildVersion ver)
            {
                if (ver == BuildVersion.v0_3_0)
                    return new LocationSerializator_0_3_0();

                throw ServantException.GetSingltoneException($"Cannot deserialize location at {ver} serialization" +
                    $" version. ");
            }



            public static IEnumerator<string> GetJsonLocation()
            {
                List<ISerializableObject> objs = GetSerializableObjects();
                foreach (var obj in objs)
                {
                    yield return obj.GetSerializationData().ToJson();
                }
            }
            public static string GetSerializationPath(string fileName,string serializationDir)
            {
                return serializationDir + fileName + FileType;
            }
            public static string GetLevelSerializationPath(string fileName)
            {
                return GetSerializationPath(fileName, LevelSerializationPath);
            }
            public static string GetSavedGameSerPath(string fileName)
            {
                return GetSerializationPath(fileName, GameSaveSerializationPath_);
            }
            public static string GetTempleSerPath(string fileName,string serializationDir)
            {
                return GetSerializationPath(fileName + TemplePostfix,serializationDir);
            }
        }


        public interface ISerializableData : ICloneable 
        {
            public string ToJson();
            /// <summary>
            /// Return true if data was been deserialized
            /// </summary>
            /// <param name="serSett"></param>
            /// <returns></returns>
            public bool TryInitFromJson(string json);
        }
        public interface ISerializableObject
        {
            /// <summary>
            /// Return Data, which wroten in file and deserialized to object
            /// </summary>
            /// <returns></returns>
            public ISerializableObjectData GetSerializationData();
            /// <summary>
            /// Return Data, which created by current state of object
            /// </summary>
            /// <returns></returns>
            public ISerializableObjectData GetDataOfCurrentState();
            public void OnStartLocationLoad();
            public void OnEndLocationLoad();
            public void Initialize(ISerializableObjectData data);
        }
        public interface ISerializableObject<TSerializationDataType>:ISerializableObject
            where TSerializationDataType :class,ISerializableObjectData
        { }
        public interface ISerializableObjectData: ISerializableData
        {
            public int SerializationId_ { get; }
            public void InstantiateObject();
        }
        public interface ILocationSettings: ISerializableData
        {
            public BuildVersion Version_ { get; }
            public string LevelName_ { get; }
            public void Apply();
        }
        public interface IResetedEnvinronment
        {
            public void Reset();
        }

        private abstract class LocationSerializator
        {
            public LocationSerializator() : this(null, null) { }
            public LocationSerializator(ILocationSettings DeserializedSettings,
                ISerializableObjectData[] DeserializedObjData)
            {
                if (DeserializedSettings!=null&&DeserializedSettings.Version_ != Version_)
                    throw ServantException.GetSerializationException("Invalid settings version. ");

                DeserializedSettings_ = DeserializedSettings;
                DeserializedObjData_ = DeserializedObjData;
            }

            public abstract BuildVersion Version_ { get; }
            protected abstract ILocationSettings GetSettings();
            protected abstract Func<ISerializableObjectData>[] ObjDataGetters_ { get; }

            public ILocationSettings DeserializedSettings_ { get; protected set; }
            public ISerializableObjectData[] DeserializedObjData_ { get; protected set; }

            public virtual void DeserializeDataAndSettings(IEnumerable<string> serializedLocation)
            {
                using (IEnumerator<string> enumerator = serializedLocation.GetEnumerator())
                {
                    List<ISerializableObjectData> list = new List<ISerializableObjectData>();
                    void AddDataInList(string current)
                    {
                        ObjectData serializedData = new(current);
                        if (serializedData.id >= ObjDataGetters_.Length)
                            throw ServantException.GetSerializationException($"Cant find data at ID={serializedData.id}. ");
                        list.Add(ObjDataGetters_[serializedData.id]());
                        ISerializableObjectData dataContainer = list[list.Count - 1];
                        if (!dataContainer.TryInitFromJson(serializedData.data))
                            throw ServantException.GetSerializationException("Cant deserialize data. ");
                    }
                    enumerator.MoveNext(); //move to version line
                    enumerator.MoveNext(); //move to settings line
                    DeserializedSettings_ = GetSettings();
                    if (!DeserializedSettings_.TryInitFromJson(enumerator.Current))
                        throw ServantException.GetSerializationException("Cant deserialize location settings. ");
                    while (enumerator.MoveNext())
                    {
                        AddDataInList(enumerator.Current);
                    }
                    DeserializedObjData_=list.ToArray();
                }
            }
            public virtual void Serialize(string serializationPath,string templePath,
                IEnumerable<ISerializableObjectData> dataQueve,ILocationSettings locationSettings)
            {
                StreamWriter writer;
                if (!File.Exists(templePath))
                    writer = new StreamWriter(File.Create(templePath));
                else
                    writer = new StreamWriter(templePath, false);
                using (writer)
                {
                    writer.WriteLine(JsonUtility.ToJson(Version_));
                    if (locationSettings == null)
                        throw ServantException.GetSerializationException("Havent location settings. ");
                    if (locationSettings.Version_ != GetSettings().Version_)
                        throw ServantException.GetSerializationException("None actual location settings." +
                            " Update LocationSerializationManager!!!");
                    writer.WriteLine(locationSettings.ToJson());
                    if (dataQueve == null)
                        throw ServantException.GetSerializationException("Havent objects data to serialize. ");
                    foreach(var data in dataQueve)
                    {
                        writer.WriteLine($"{data.SerializationId_}{ObjectHeaderSym}{data.ToJson()}");
                    }
                }
                File.Copy(templePath, serializationPath, true);
                File.Delete(templePath);
            }

            protected struct ObjectData 
            {
                public ObjectData(string input)
                {
                    int index = input.IndexOf(ObjectHeaderSym);
                    if (index == -1)
                        throw ServantException.GetIncorrectSerializedObjectIdExc();
                    if(!int.TryParse(input.Substring(0,index),out id))
                        throw ServantException.GetIncorrectSerializedObjectIdExc();
                    data=input.Substring(index+1,input.Length - index-1);
                    if (data.Length <= 0 || data[0] == '\n')
                        throw ServantException.GetMissingObjectDataExc(id);
                }
                public int id;
                public string data;
            }
        }
        private interface ILocationSerilConverter<TOutputType>
            where TOutputType:LocationSerializator
        {
            public BuildVersion OutputVersion_ { get; }
            public TOutputType Convert();
        }
    }
    public abstract class SerializationComponent<TOwnerScriptType, TSerializationData> : MonoBehaviour,
        ISerializableObject<TSerializationData>
        where TOwnerScriptType : MonoBehaviour
        where TSerializationData : class, ISerializableObjectData
    {
        [SerializeField]
        protected TSerializationData Data;
        protected TOwnerScriptType Owner_
        {
            get
            {
                if (Owner == null)
                    throw ServantException.GetNullInitialization("Owner");
                return Owner;
            }
            set => Owner = value;
        }
        [SerializeField]
        private TOwnerScriptType Owner;
        public abstract TSerializationData GetDataOfCurrentState();
        public abstract TSerializationData GetSerializationData();
        protected abstract void InitializeAction(TSerializationData data);
        protected virtual void OnEndLocationLoad() { }
        protected virtual void OnStartLocationLoad() { }


        ISerializableObjectData ISerializableObject.GetDataOfCurrentState() => GetDataOfCurrentState();
        ISerializableObjectData ISerializableObject.GetSerializationData() => GetSerializationData();
        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, InitializeAction);
        }
        void ISerializableObject.OnEndLocationLoad() => OnEndLocationLoad();
        void ISerializableObject.OnStartLocationLoad() => OnStartLocationLoad();
        private void Awake()
        {
            if (Owner == null)
                Owner = GetComponent<TOwnerScriptType>();
        }
    }
}
