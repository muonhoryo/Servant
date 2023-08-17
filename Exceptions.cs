
using UnityEngine;
using System;

namespace Servant
{
    public class ServantException : Exception
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
        public static ServantException GetNullInitialization(string initializedField)
        {
            return new ServantException(initializedField + " was not assigned.");
        }
        public static ServantException GetSerializationException(string additionalInfo="")
        {
            return new ServantException("Serialization exception."+additionalInfo);
        }
        public static ServantException GetIncorrectSerializedObjectIdExc(string additionalInfo = "") =>
            GetSerializationException("Incorrect ID of serialized object. " + additionalInfo);
        public static ServantException GetMissingObjectDataExc(int id, string addtionalInfo = "") =>
            GetSerializationException($"Missing data of object at ID={id}. " + addtionalInfo);
        public static ServantException GetNullOrZeroLengthStringExc(string strName,string additionalInfo = "") =>
            GetSerializationException($"{strName} cannot be null or with zero length. " + additionalInfo);
        public static ServantException GetNoneAssignedMethodException() => new ServantException("Method is not assigned.");
        public static ServantException GetArgumentNullException(string argName, string additionalInfo = "") =>
            new ServantException($"{argName} cannot be null. {additionalInfo}");
    }
    public class ServantCameraException: ServantException
    {
        public ServantCameraException(string message):base(message+ "\nTurn camera to CameraPointState.") 
        {
            MainCameraBehaviour.singltone.SetCameraMode_Default();
        }
        public static ServantCameraException GetNullTargetException(string additionalInfo = "") =>
            new ServantCameraException("Target does not exist.\n" + additionalInfo);
    }
    public class ServantRequaringCompDeletingException : ServantException
    {
        public ServantRequaringCompDeletingException(string compName)
            : base($"Requaring component {compName} has been destroyed. ") { }
    }
    public class ServantIncorrectInputArgument : ServantException
    {
        public ServantIncorrectInputArgument(string argumentName, string additionalInfo) :
            base($"Incorrect input argument {argumentName}. {additionalInfo}"){ } 
    }
}
