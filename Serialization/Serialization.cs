
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Servant.Serialization
{
    public static class LocationSerializationData
    {
        public const string LocationSerializationPath = "Assets/Scripts/Serialization/SerializedLocations/";
        public const string FileType = ".json";
        public const string SerializationIdName="Id:";
    	public const char SeparateSym=';';
        public const char OpenArraySym = '[';
        public const char CloseArraySym = ']';
        public const char QuotesSym='"';
        public const string ServantSerializationTag = "SerializedObject";
        private static List<GameObject> Prefabs=new List<GameObject> { };
        public static event Action StartLocationUnloadingEvent=Registry.EmptyMethod;
        public static event Action EndLocationUnloadingEvent=Registry.EmptyMethod;
        public static event Action StartLocationLoadingEvent=Registry.EmptyMethod;
        public static event Action EndLocationLoadingEvent=Registry.EmptyMethod;
        public static void TurnOnLoadingSerializatedObjs()
        {
            IEnumerable<Action> queue=
                GetSerializatedObjects().Select
                <ISerializedObject, Action>((obj) => obj.OnStartLocationLoad);
            foreach (var action in queue) action();
        }
        public static void TurnOffLoadingSerializatedObjs()
        {
            IEnumerable<Action> queue =
                GetSerializatedObjects().Select
                <ISerializedObject, Action>((obj) => obj.OnEndLocationLoad);
            foreach (var action in queue) action();
        }
        public static List<ISerializedObject> GetSerializedObjectsAsync()
        {
            AutoResetEvent handler = new AutoResetEvent(false);
            List<ISerializedObject> serializedObjects = new List<ISerializedObject>(0);
            void FindObjets()
            {
                GameObject[] objects = new GameObject[0];
                objects = GameObject.FindGameObjectsWithTag(ServantSerializationTag);
                serializedObjects = new List<ISerializedObject>(objects.Length);
                foreach (GameObject obj in objects)
                {
                    serializedObjects.Add(obj.GetComponent<ISerializedObject>());
                }
            }
            Registry.ThreadManager.AddActionsQueue(FindObjets, handler);
            handler.WaitOne();
            return serializedObjects;
        }
        public static List<ISerializedObject> GetSerializatedObjects()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag
                (ServantSerializationTag);
            List<ISerializedObject> serializedObjects = new List<ISerializedObject>(objects.Length);
            foreach(GameObject obj in objects)
            {
                serializedObjects.Add(obj.GetComponent<ISerializedObject>());
            }
            return serializedObjects;
        }
        public static IEnumerator<string> GetLocationSerialization()
        {
            foreach(var obj in GetSerializatedObjects())
            {
                yield return obj.Serialize();
            }
        }
        public static void LoadEnvironmentAsync(IEnumerable<string> serializationData)
        {
            AutoResetEvent handler = new AutoResetEvent(false);
            void WaitAndReset()
            {
                handler.WaitOne();
                handler.Reset();
            }
            //Execute start unloading event
            Registry.ThreadManager.AddActionsQueue(StartLocationUnloadingEvent, handler);
            WaitAndReset();
            //
            void TurnOnHandler() => handler.Set();
            void EndObjDeserialization(ISerializedObject obj) => obj.OnEndLocationLoad();
            //Destroy objects
            List<ISerializedObject> objectsInScene = GetSerializedObjectsAsync();
            Registry.ThreadManager.AddActionsQueue
                (objectsInScene.Select<ISerializedObject,
                Action>(obj => () => GameObject.Destroy((obj as MonoBehaviour).gameObject)),TurnOnHandler);
            WaitAndReset();
            //Execute end unloading event
               Registry.ThreadManager.AddActionsQueue(EndLocationUnloadingEvent, handler);
                WaitAndReset();
            //Execute start loading event
            Registry.ThreadManager.AddActionsQueue(StartLocationLoadingEvent, handler);
            WaitAndReset();
            //
            void DeserializeObject(string data)
            {
                int firstIndex = data.IndexOf(SerializationIdName) + SerializationIdName.Length;
                int separateIndex = data.IndexOf(SeparateSym);
                if (!int.TryParse(data.Substring(firstIndex, separateIndex-firstIndex), out int id))
                {
                    throw new ServantException("Invalid id");
                }
                else
                {
                    GameObject instObj = GameObject.Instantiate(Prefabs[id]);
                    objectsInScene.Add(instObj.GetComponent<ISerializedObject>());
                    objectsInScene[objectsInScene.Count - 1].Deserialize(data, separateIndex+1);
                }
            }
            //Loading objects
            Registry.ThreadManager.AddActionsQueue
                (serializationData.Select<string, Action>(obj => () => DeserializeObject(obj)),
                TurnOnHandler);
            WaitAndReset();
            //Send to objects message about loading end
            objectsInScene = GetSerializedObjectsAsync();
            Registry.ThreadManager.AddActionsQueue
                (objectsInScene.Select<ISerializedObject, Action>
                (obj => () => EndObjDeserialization(obj)), TurnOnHandler);
            WaitAndReset();
            //Execute end loading event
                Registry.ThreadManager.AddActionsQueue(EndLocationLoadingEvent, handler);
                handler.WaitOne();
        }
        public static Vector2 DeserializeVector2(string serializedVec)
        {
            int openIndex = serializedVec.IndexOf(OpenArraySym);
            if (openIndex == -1) throw ServantException.SerializationException();
            int separateSymIndex = serializedVec.IndexOf(SeparateSym);
            if(!float.TryParse(
                serializedVec.Substring(openIndex+1,separateSymIndex-1),out float x))
            {
                throw ServantException.SerializationException("Cant parse value X.");
            }
            int closeIndex = serializedVec.IndexOf(CloseArraySym);
            if (closeIndex == -1) throw ServantException.SerializationException();
            if (!float.TryParse(
                serializedVec.Substring(separateSymIndex+1,
                closeIndex-separateSymIndex-1), out float y))
            {
                throw ServantException.SerializationException("Cant parse value Y by "+
                    serializedVec.Substring(separateSymIndex + 1,closeIndex - separateSymIndex-1)+
                    ".");
            }
            return new Vector2(x, y);
        }
        public static string SerializeVector2(this Vector2 vector)
        {
            return ""+OpenArraySym + vector.x + SeparateSym + vector.y + CloseArraySym;
        }
        public static string SerializeVector2(this Vector3 vector)
        {
            return ((Vector2)vector).SerializeVector2();
        }
        public static string GetSubData(string data,int dataStart,out int nextSepSymIndex,
            int separateSymCount=1,bool isStringData=false)
        {
            Func<char, int, bool> action;
            int separateSymIndex = -1;
            int handledSeparateSymCount = 0;
            bool SignificentState(char input, int index)
            {
                if (input == QuotesSym)
                {
                    action = QuotesState;
                    return false;
                }
                else if (input == SeparateSym)
                {
                    if (handledSeparateSymCount == separateSymCount - 1)
                    {
                        separateSymIndex = index;
                        return true;
                    }
                    else
                    {
                        handledSeparateSymCount++;
                        return false;
                    }
                }
                else return false;
            }
            bool QuotesState(char input, int index)
            {
                if (input == QuotesSym)
                {
                    action = SignificentState;
                }
                return false;
            }
            action = SignificentState;
            for (int i = dataStart; handledSeparateSymCount < separateSymCount && i < data.Length; i++)
            {
                if (action(data[i], i))
                {
                    break;
                }
            }
            if (separateSymIndex == -1) throw ServantException.SerializationException();
            nextSepSymIndex = separateSymIndex;
            string returnData() => data.Substring(dataStart, separateSymIndex - dataStart);
            return isStringData ?returnData().Trim(QuotesSym): returnData();
        }
        public static string GetSerializationPath(string locationName)
        {
            return LocationSerializationPath + locationName + FileType;
        }
        public static string GetSerializedId(this ISerializedObject owner) =>
            SerializationIdName + owner.SerializationId;
        public static void SetSerializatedObjsPrefabs(List<GameObject> prefabs)
        {
            Prefabs = prefabs;
        }
    }
    public interface ISerializedObject
    {
        public int SerializationId { get; }
        public string Serialize();
        public void Deserialize(string serializedObject,int dataStart);
        public void OnStartLocationLoad();
        public void OnEndLocationLoad();
    }
    public abstract class SerializedObject: MonoBehaviour, ISerializedObject
    {
        [SerializeField]
        private int SerializationId;
        int ISerializedObject.SerializationId => SerializationId;
        public abstract void Deserialize(string serializedObject,int dataStart);
        void ISerializedObject.Deserialize(string serializedObject, int dataStart) =>
            Deserialize(serializedObject, dataStart);
        public abstract string Serialize();
        string ISerializedObject.Serialize() => Serialize();
        public virtual void OnStartLocationLoad() { }
        void ISerializedObject.OnStartLocationLoad() => OnStartLocationLoad();
        public virtual void OnEndLocationLoad() { }
        void ISerializedObject.OnEndLocationLoad() => OnEndLocationLoad();
    }
}
