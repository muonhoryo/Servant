

using UnityEngine;
using Servant.Serialization;
using System;

namespace Servant
{
    public static partial class Registry
    {
        public const int GarpoonLayerMask=192;
        public const int GroundLayerMask = 192;
        public const int GroundLayer = 6;
        public const int MovableItemLayer = 7;
        public static Control.MainCharacterController CharacterController;
        public static SaveInfoContainer SaveInfoContainer;
        public static SaveLoadSystem.ILocationSettings SettingsOfCurrentLocation;
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
        public static float GarpoonGroundPullDoneThreshold { get; private set; }
        public static float GarpoonGroundPullFinalImpulseMod { get; private set; }
        public static float GarpoonGroundPullStartSpeed { get; private set; }
        public static float GarpoonGroundPullAcceleration { get; private set; }
        public static float GarpoonGroundCollisionPullStopThreshold { get; private set; }
        public static float GarpoonItemPullDoneThreshold { get; private set; }
        public static float GarpoonItemPullFinalImpulseMod { get; private set; }
        public static float GarpoonItemPullStartSpeed { get; private set; }
        public static float GarpoonItemPullAcceleration { get; private set; }
        public static float GarpoonRockingMoveSpeed { get; private set; }
        public static void SetConst(float MainCharacterWalkSpeed,float MainCharacterRunSpeed,
            float MainCharacterJumpMoveSpeed,float AccelMoveModifier,float MainCharacterJumpForce,
            float JumpStateTransitDelay,float GarpoonSpeed,float GarpoonProjectileMaxDistance,
            float GarpoonMaxHookDistance,float GarpoonGroundPullDoneThreshold,
            float GarpoonGroundPullFinalImpulseMod, float GarpoonGroundPullStartSpeed,
            float GarpoonGroundPullAcceleration, float GarpoonGroundCollisionPullStopThreshold,
            float GarpoonItemPullDoneThreshold,float GarpoonItemPullFinalImpulseMod,
            float GarpoonItemPullStartSpeed,float GarpoonItemPullAcceleration,
            float GarpoonRockingMoveSpeed)
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
            Registry.GarpoonGroundPullDoneThreshold = GarpoonGroundPullDoneThreshold;
            Registry.GarpoonGroundPullFinalImpulseMod = GarpoonGroundPullFinalImpulseMod;
            Registry.GarpoonGroundPullStartSpeed = GarpoonGroundPullStartSpeed;
            Registry.GarpoonGroundPullAcceleration = GarpoonGroundPullAcceleration;
            Registry.GarpoonGroundCollisionPullStopThreshold = GarpoonGroundCollisionPullStopThreshold;
            Registry.GarpoonItemPullDoneThreshold = GarpoonItemPullDoneThreshold;
            Registry.GarpoonItemPullFinalImpulseMod = GarpoonItemPullFinalImpulseMod;
            Registry.GarpoonItemPullStartSpeed = GarpoonItemPullStartSpeed;
            Registry.GarpoonItemPullAcceleration = GarpoonItemPullAcceleration;
            Registry.GarpoonRockingMoveSpeed = GarpoonRockingMoveSpeed;
        }
    }
    [Serializable]
#pragma warning disable CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
    public struct BuildVersion
#pragma warning restore CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
    {
        public static readonly BuildVersion v0_3_0 = new(0, 3, 0);

        public BuildVersion(int ReleaseVersion, int SystemVersion, int FixVersion)
        {
            R = ReleaseVersion;
            S = SystemVersion;
            F = FixVersion;
        }
        /// <summary>
        /// Release Version
        /// </summary>
        public int R;
        /// <summary>
        /// System Version
        /// </summary>
        public int S;
        /// <summary>
        /// Fix Version
        /// </summary>
        public int F;
        public static bool operator ==(BuildVersion left, BuildVersion right) =>
            left.R == right.R &&
            left.S == right.S &&
            left.F == right.F;
        public static bool operator !=(BuildVersion left, BuildVersion right) =>
            !(left == right);
    }
}
