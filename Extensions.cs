
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
    }
}
