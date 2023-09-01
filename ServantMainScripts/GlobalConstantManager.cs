
using MuonhoryoLibrary;
using Servant.DevelopmentOnly;
using System;
using System.IO;
using UnityEngine;

namespace Servant.DevelopmentOnly
{
    public sealed class GlobalConstantManager : MonoBehaviour, ISingltone<GlobalConstantManager>,
        ISerializationCallbackReceiver
    {
        [SerializeField]
        private GlobalConstants Constants;
        public GlobalConstants Constants_ { get => Constants; }
        private const string SerializationPath = "Assets/Scripts/GlobalConstants.json";
        public static GlobalConstantManager instance_ { get; private set; } = null;
        GlobalConstantManager ISingltone<GlobalConstantManager>.Singltone
        { get => instance_; set => instance_ = value; }
        void ISingltone<GlobalConstantManager>.Destroy()
        {
            Destroy(this);
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (!File.Exists(SerializationPath))
                File.Create(SerializationPath).Close();
            using (StreamWriter writer = new StreamWriter(SerializationPath, false))
            {
                writer.Write(JsonUtility.ToJson(Constants, true));
            }
        }
        public void OnCreateInspector()
        {
            if (!File.Exists(SerializationPath))
                throw ServantException.GetSerializationException("File at path " + SerializationPath + " doesn't exists.");
            using (StreamReader reader = new StreamReader(SerializationPath))
            {
                Constants = JsonUtility.FromJson<GlobalConstants>(reader.ReadToEnd());
            }
        }
        private void Awake()
        {
            this.InitializeSingltoneWithDeleting();
        }
        private void OnValidate()
        {
            instance_ = this;
        }
    }
}
namespace Servant
{
    [Serializable]
    public struct GlobalConstants
    {
        public static GlobalConstants Singlton => GlobalConstantManager.instance_.Constants_;

        public float HumanCharacters_RunSpeed;
        public float HumanCharacters_JumpForce;
        public float HumanCharacters_JumpDelay;
        public float HumanCharacters_AirMovingSpeedModifier;
        public float HumanCharacters_RockingMoveSpeed;
        public float HumanCharacters_DodgingSpeedMinBuff;
        public float HumanCharacters_DodgingSpeedDescentStep;
        public float HumanCharacters_LandingDiagonalRollSpeedModifier;
        public float HumanCharacters_LandingRollMinForce;
        public float HumanCharacters_WallDetectionMinCos;
        public float HumanCharacters_GroundDetectionMinCos;
        public float HumanCharacters_GroundForceDodgeMinAngle;
        public float HTK_ProjectileMaxDistance;
        public float Garpoon_CollisionForceDirectionToStopPull;
        public float Garpoon_PullDoneThreshold;
        public float GuardAndroid_MovingSpeed;
    }
}
