

using UnityEngine;
using Servant.Serialization;
using System;

namespace Servant
{
    public static partial class Registry
    {
        public const int GarpoonProjectileLayerMask=192;
        public const int GroundLayerMask = 192;
        public const int GroundLayer = 6;
        public const int MovableItemLayer = 7;
        public const int CharactersLayer=8;
        public static Control.CharacterController CharacterController_=> Control.CharacterController.Controller_;
        public static SaveInfoContainer SaveInfoContainer;
        public static SaveLoadSystem.ILocationSettings SettingsOfCurrentLocation;
        public static ServantThreadManager ThreadManager;
    }
    [Serializable]
#pragma warning disable CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
    public struct BuildVersion
#pragma warning restore CS0661 // Тип определяет оператор == или оператор !=, но не переопределяет Object.GetHashCode()
    {
        public static readonly BuildVersion v0_3_0 = new(0, 3, 0);
        public static readonly BuildVersion v0_4_0 = new(0, 4, 0);

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
