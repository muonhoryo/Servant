
using System.IO;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Servant.Serialization;
using Servant.GUI;

namespace Servant
{
    public static class Registry
    {
        public const int GarpoonLayerMask=64;
        public const int GroundLayer = 6;
        public const string SerializedObjectTag = "SerializedObject";
        public static GameObject LoadingScreenPrefab;
        public static Control.MainCharacterController CharacterController;
        public static ServantThreadManager ThreadManager;
        public static float MainCharacterWalkSpeed { get; private set; }
        public static float MainCharacterRunSpeed { get; private set; }
        public static float MainCharacterJumpMoveSpeed { get; private set; }
        public static float AccelMoveModifier {get;private set;}
        public static float MainCharacterJumpForce {get;private set;}
        public static float JumpStateTransitDelay { get; private set; }
        public static float GarpoonSpeed { get; private set; }
        public static float GarpoonProjectileMaxDistance { get; private set; }
        public static float GarpoonMaxHookDistance { get; private set; }
        public static float GarpoonPullDoneThreshold { get; private set; }
        public static float GarpoonPullFinalImpulseMod { get; private set; }
        public static float GarpoonPullStartSpeed { get; private set; }
        public static float GarpoonPullAcceleration { get; private set; }
        public static float GarpoonPullStopThreshold { get; private set; }
        public static float GarpoonRockingMoveSpeed { get; private set; }
        public static void SetConst(float MainCharacterWalkSpeed,float MainCharacterRunSpeed,
            float MainCharacterJumpMoveSpeed,float AccelMoveModifier,float MainCharacterJumpForce,
            float JumpStateTransitDelay,float GarpoonSpeed,float GarpoonProjectileMaxDistance,
            float GarpoonMaxHookDistance,float GarpoonPullDoneThreshold,
            float GarpoonPullFinalImpulseMod, float GarpoonPullStartSpeed,
            float GarpoonPullAcceleration,float GarpoonPullStopThreshold,float GarpoonRockingMoveSpeed)
        {
            Registry.MainCharacterWalkSpeed = MainCharacterWalkSpeed;
            Registry.MainCharacterRunSpeed = MainCharacterRunSpeed;
            Registry.MainCharacterJumpMoveSpeed = MainCharacterJumpMoveSpeed;
            Registry.AccelMoveModifier=AccelMoveModifier;
            Registry.MainCharacterJumpForce=MainCharacterJumpForce;
            Registry.JumpStateTransitDelay = JumpStateTransitDelay;
            Registry.GarpoonSpeed = GarpoonSpeed;
            Registry.GarpoonProjectileMaxDistance = GarpoonProjectileMaxDistance;
            Registry.GarpoonMaxHookDistance = GarpoonMaxHookDistance;
            Registry.GarpoonPullDoneThreshold = GarpoonPullDoneThreshold;
            Registry.GarpoonPullFinalImpulseMod = GarpoonPullFinalImpulseMod;
            Registry.GarpoonPullStartSpeed = GarpoonPullStartSpeed;
            Registry.GarpoonPullAcceleration = GarpoonPullAcceleration;
            Registry.GarpoonPullStopThreshold = GarpoonPullStopThreshold;
            Registry.GarpoonRockingMoveSpeed = GarpoonRockingMoveSpeed;
        }
        public static void EmptyMethod() { }
        public static void LoadLocation(string locationName)
        {
            string path = LocationSerializationData.GetSerializationPath(locationName);
            if (!File.Exists(path))
                throw ServantException.SerializationException("File at " + path + " does not exist.");
            void LoadLocationAsync()
            {
                List<string> serializationData = new List<string>();
                using(StreamReader stream=new StreamReader(path))
                {
                    while (stream.Peek() != -1)
                    {
                        serializationData.Add(stream.ReadLine());
                    }
                }
                LocationSerializationData.LoadEnvironmentAsync(serializationData);
            }
            LocationSerializationData.TurnOnLoadingSerializatedObjs();
            GameObject loadingScreen = GameObject.Instantiate(LoadingScreenPrefab,
                GUIManager.GUICanvas.transform);
            void EndLocationEvent()
            {
                LocationSerializationData.TurnOffLoadingSerializatedObjs();
                GameObject.Destroy(loadingScreen);
                LocationSerializationData.EndLocationLoadingEvent -= EndLocationEvent;
            }
            LocationSerializationData.EndLocationLoadingEvent += EndLocationEvent;
            new Thread(new ThreadStart(LoadLocationAsync)).Start();
        }
    }
}
