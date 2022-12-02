
using Servant;
using UnityEngine;

namespace MuonhoryoLibrary
{
    public static class SingltoneExtensions
    {
        public static void ValidateSingltone<TSingltoneType>(this TSingltoneType owner)
            where TSingltoneType :Object,ISingltone<TSingltoneType>
        {
            if (owner.Singltone != null && owner.Singltone != owner)
            {
                throw new ServantException("Have more than one examples of singltone.");
            }
            else 
                owner.Singltone = owner;
        }
        public static Vector2 AngleOffset(this Vector2 offset,float degressAngle)
        {
            return new Vector2
                (offset.x * Mathf.Cos(degressAngle * Mathf.PI / 180) -
                    offset.y * Mathf.Sin(degressAngle * Mathf.PI / 180),
                offset.y * Mathf.Cos(degressAngle * Mathf.PI / 180) +
                    offset.x * Mathf.Sin(degressAngle * Mathf.PI / 180));        
        }
    }
}
