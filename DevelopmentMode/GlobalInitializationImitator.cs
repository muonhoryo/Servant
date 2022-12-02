
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
        public float GarpoonPullDoneThreshold;
        public float GarpoonPullStartSpeed;
        public float GarpoonPullAcceleration;
        public float GarpoonPullFinalImpulseMod;
        public float GarpoonPullStopThreshold;
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
                 GarpoonPullDoneThreshold: GarpoonPullDoneThreshold,
                 GarpoonPullFinalImpulseMod: GarpoonPullFinalImpulseMod,
                 GarpoonPullStartSpeed: GarpoonPullStartSpeed,
                 GarpoonPullAcceleration: GarpoonPullAcceleration,
                 GarpoonPullStopThreshold: GarpoonPullStopThreshold,
                 GarpoonRockingMoveSpeed: GarpoonRockingMoveSpeed);
        }
        private void OnValidate()
        {
            this.ValidateSingltone();
        }
    }
}
