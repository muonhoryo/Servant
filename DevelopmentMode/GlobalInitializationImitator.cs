
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
        private static GlobalInitializationImitator singltone;
        GlobalInitializationImitator ISingltone<GlobalInitializationImitator>.Singltone
        { get => singltone; set => singltone = value; }
        private void Awake()
        {
            Registry.SetConst(MainCharacterWalkSpeed, MainCharacterRunSpeed, MainCharacterJumpMoveSpeed,
                AccelMoveModifier, MainCharacterJumpForce, JumpStateTransitDelay);
        }
        private void OnValidate()
        {
            this.ValidateSingltone();
        }
    }
}
