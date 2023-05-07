
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant.DevelopmentOnly
{
    public sealed class GlobalInitializationImitator : MonoBehaviour, ISingltone<GlobalInitializationImitator>
    {
        public float MainCharacterWalkSpeed;
        public float MainCharacterRunSpeed;
        public float MainCharacterJumpMoveSpeed;
        public float AccelMoveModifier;
        public float MainCharacterJumpForce;
        public float JumpStateTransitDelay;
        public float GarpoonSpeed;
        public float GarpoonProjectileMaxDistance;
        public float GarpoonMaxHookDistance;
        public float GarpoonGroundPullDoneThreshold;
        public float GarpoonGroundPullStartSpeed;
        public float GarpoonGroundPullAcceleration;
        public float GarpoonGroundPullFinalImpulseMod;
        public float GarpoonGroundCollisionPullStopThreshold;
        public float GarpoonItemPullDoneThreshold;
        public float GarpoonItemPullFinalImpulseMod;
        public float GarpoonItemPullStartSpeed;
        public float GarpoonItemPullAcceleration;
        public float GarpoonRockingMoveSpeed;
        private static GlobalInitializationImitator singltone;
        GlobalInitializationImitator ISingltone<GlobalInitializationImitator>.Singltone
        { get => singltone; set => singltone = value; }
        private void Awake()
        {
            Registry.SetConst
                (MainCharacterWalkSpeed: MainCharacterWalkSpeed,
                 MainCharacterRunSpeed: MainCharacterRunSpeed,
                 MainCharacterJumpMoveSpeed: MainCharacterJumpMoveSpeed,
                 AccelMoveModifier: AccelMoveModifier,
                 MainCharacterJumpForce: MainCharacterJumpForce,
                 JumpStateTransitDelay: JumpStateTransitDelay,
                 GarpoonSpeed: GarpoonSpeed,
                 GarpoonProjectileMaxDistance: GarpoonProjectileMaxDistance,
                 GarpoonMaxHookDistance: GarpoonMaxHookDistance,
                 GarpoonGroundPullDoneThreshold: GarpoonGroundPullDoneThreshold,
                 GarpoonGroundPullFinalImpulseMod: GarpoonGroundPullFinalImpulseMod,
                 GarpoonGroundPullStartSpeed: GarpoonGroundPullStartSpeed,
                 GarpoonGroundPullAcceleration: GarpoonGroundPullAcceleration,
                 GarpoonGroundCollisionPullStopThreshold: GarpoonGroundCollisionPullStopThreshold,
                 GarpoonItemPullDoneThreshold: GarpoonItemPullDoneThreshold,
                 GarpoonItemPullFinalImpulseMod: GarpoonItemPullFinalImpulseMod,
                 GarpoonItemPullStartSpeed: GarpoonItemPullStartSpeed,
                 GarpoonItemPullAcceleration: GarpoonItemPullAcceleration,
                 GarpoonRockingMoveSpeed: GarpoonRockingMoveSpeed) ;
        }
        private void OnValidate()
        {
            this.ValidateSingltone();
        }
    }
}
