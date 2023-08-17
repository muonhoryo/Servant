
using System;
using UnityEngine;
using Servant;
using static Servant.Serialization.SaveLoadSystem;
using JetBrains.Annotations;

namespace MuonhoryoLibrary
{
    public static class UnityExtensions
    {
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

        public static int Sign(this int value)
        {
            if (value == 0)
                throw new ServantException("value cannot be zero.");
            return value > 0 ? 1 : -1;
        }
        /// <summary>
        /// 1 - counter-clockwise, -1 - clokwise
        /// </summary>
        /// <param name="normalizedDirToCenter"></param>
        /// <param name="movingDirection"></param>
        /// <returns></returns>
        public static Vector2 GetRadialForceDirection(this Vector2 normalizedDirToCenter, int movingDirection)
        {
            Vector2 dir = Vector2.Perpendicular(normalizedDirToCenter);
            if (movingDirection > 0)
                dir = -dir;
            return dir;
        }
        public static float RoundTo(this float value,float roundCoef)
        {
            return value -= value % roundCoef;
        }
    }
    public static class SingltoneExtensions
    {
        /// <summary>
        /// Destroys any scripts except first initialized.
        /// </summary>
        /// <typeparam name="TScript"></typeparam>
        /// <param name=""></param>
        public static void InitializeSingltoneWithDeleting<TScript>(this ISingltone<TScript> owner)
            where TScript : class,ISingltone<TScript>
        {
            if (owner.Singltone != null&&owner.Singltone!=owner)
            {
                owner.Destroy();
            }
            else
            {
                owner.Singltone = owner as TScript;
            }
        }
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
namespace Servant.Characters 
{
    public static class CharactersExtensions
    {
        public static bool IsInAir(this IGroundCharacter owner)=>
             owner.CurrentFallingState_ != IGroundCharacter.FallingState.StandingOnGround;
        public static bool IsFalling(this IGroundCharacter owner) =>
            owner.CurrentFallingState_ == IGroundCharacter.FallingState.Falling;
        public static bool IsTheHitObjectGround(this IGarpoonCharacter owner) =>
            owner.GarpoonBase_.ShootedProjectile_.HitObject_.layer == Registry.GroundLayer;
        public static bool DoesHookCatch_(this IGarpoonBase owner)=>
            owner.ShootedProjectile_ != null && owner.ShootedProjectile_.HitObject_ != null;
        public static bool DidGarpoonShoot(this IGarpoonBase owner) =>
            owner.ShootedProjectile_ != null;
        public static bool HasWallsAtMovingDirection(this IGroundCharacter owner, IWallChecker wallChecker) =>
            owner.MovingDirection_ > 0 ? wallChecker.IsThereRightWall_ : wallChecker.IsThereLeftWall_;
    }
}
