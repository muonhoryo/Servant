
using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Servant
{
    public sealed class ServantException : Exception
    {
        private void ExceptionAction(string message)
        {
            Debug.LogError(message);
        }
        public ServantException(string message):base(message)
        {
            ExceptionAction(message);
        }
        public static ServantException GetSingltoneException(string singltoneName)
        {
            return new ServantException(singltoneName+" is already exist.");
        }
        public static ServantException NullInitialization(string initializedField)
        {
            return new ServantException(initializedField + " was not assigned.");
        }
        public static ServantException SerializationException(string additionalInfo="")
        {
            return new ServantException("Serialization exception."+additionalInfo);
        }
        public static ServantException NoneAssignedMethod() =>
            new ServantException("Method is not assigned.");
    }
}
