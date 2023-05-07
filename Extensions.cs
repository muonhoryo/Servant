
using System;
using UnityEngine;
using Servant;
using static Servant.Serialization.SaveLoadSystem;

namespace MuonhoryoLibrary
{
    public static class SingltoneExtensions
    {
        public static void ValidateSingltone<TSingltoneType>(this TSingltoneType owner)
            where TSingltoneType :UnityEngine.Object,ISingltone<TSingltoneType>
        {
            if (owner.Singltone != null && owner.Singltone != owner)
            {
                throw new ServantException("Have more than one examples of singltone.");
            }
            else 
                owner.Singltone = owner;
        }
    }
    public static class UnityExtensions
    {
        public static Vector2 AngleOffset(this Vector2 offset, float degressAngle)
        {
            return new Vector2
                (offset.x * Mathf.Cos(degressAngle * Mathf.PI / 180) -
                    offset.y * Mathf.Sin(degressAngle * Mathf.PI / 180),
                offset.y * Mathf.Cos(degressAngle * Mathf.PI / 180) +
                    offset.x * Mathf.Sin(degressAngle * Mathf.PI / 180));
        }
        public static bool IsInLayerMask(this int layer, int layerMask)
        {
            return (layerMask & (int)Math.Pow(2, layer)) != 0;
        }
        public static Vector2 GetBorderlinePoint(this Rect limit, Vector2 point)
        {
            float x = point.x;
            float y = point.y;
            if (x < limit.xMin) x = limit.xMin;
            else if (x > limit.xMax) x = limit.xMax;
            if (y < limit.yMin) y = limit.yMin;
            else if (y > limit.yMax) y = limit.yMax;
            return new Vector2(x, y);
        }
        public static Vector3 GetEulerAngleOfImage(this Vector3 oldEulerRot, float newImageAngle) =>
            new Vector3(oldEulerRot.x, oldEulerRot.y, newImageAngle);
    }
}
namespace Servant.Serialization 
{
    public static class SerializableObjectDataExtensions
    {
        private static void InstanceAndInitialize<TSerializableObject>
            (this ISerializableObjectData data, TSerializableObject instancedObj)
            where TSerializableObject : ISerializableObject
        {
            if (instancedObj == null)
                throw ServantException.GetSerializationException("Instantiated object does not exist component with" +
                    " type " + typeof(ISerializableObject));
            instancedObj.Initialize(data);
        }
        public static void InstanceAndInitialize<TSerializableObject>
            (this ISerializableObjectData data,GameObject prefab)
            where TSerializableObject : ISerializableObject
        {
            var obj=GameObject.Instantiate(prefab).GetComponent<TSerializableObject>(); 
            InstanceAndInitialize(data, obj);
        }
        public static void InstanceAndInitialize<TSerializableObject>
            (this ISerializableObjectData data,GameObject prefab,Transform parent)
            where TSerializableObject : ISerializableObject
        {
            var obj=GameObject.Instantiate(prefab, parent).GetComponent<TSerializableObject>();
            InstanceAndInitialize(data, obj);
        }
    }
    public static class SerializableObjectExtensions
    {
        public static void ValidateInputAndInitialize<TSerData>
            (this ISerializableObject<TSerData> owner,
            ISerializableObjectData input, Action<TSerData> initializeAction)
            where TSerData : class,ISerializableObjectData
        {
            if (input == null)
                throw ServantException.GetArgumentNullException("input");
            if(input is not TSerData)
                throw ServantException.GetSerializationException("Wrong type of data. Convert old data to actual version. ");
            initializeAction(input as TSerData);
        }
    }
}
