

using System;
using UnityEngine;
using MuonhoryoLibrary;
using Servant.Control;
using Servant.Serialization._0_3_0;
using static Servant.Serialization.SaveLoadSystem;
using Servant.Serialization;

namespace Servant.Control
{
    public sealed partial class MainCharacterController :  ISerializableObject<MainCharacterData_0_3_0>,
        ISingltone<MainCharacterController>, IResetedEnvinronment
    {
        private MainCharacterData_0_3_0 serializationData;
        private bool IsInitialized = false;
        private void InitializeFromAvailibleData()
        {
            transform.position = serializationData.Position;
            isLeftSide = serializationData.IsLeftSide;
            Awake();
        }
        private void Awake()
        {
            if (!IsInitialized)
            {
                if (Registry.CharacterController != null &&
                    Registry.CharacterController != this)
                {
                    Destroy(Registry.CharacterController);
                }
                Registry.CharacterController = this;

                if (rigidbody == null) rigidbody = GetComponent<Rigidbody2D>();
                if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
                if (animator == null) animator = GetComponent<Animator>();
                if (groundChecker == null) groundChecker = GetComponentInChildren<GroundChecker>();


                if (rigidbody == null) throw ServantException.GetNullInitialization("rigidbody");
                if (spriteRenderer == null) throw ServantException.GetNullInitialization("spriteRenderer");
                if (animator == null) throw ServantException.GetNullInitialization("animator");
                if (groundChecker == null) throw ServantException.GetNullInitialization("groundChecker");


                FallingEvent += () => CurrentControllerState.FallAction();
                LandingEvent += () => CurrentControllerState.LandAction();
                FallingEvent += () => CurrentGarpoonState.FallAction();
                LandingEvent += () => CurrentGarpoonState.LandAction();
                DefaultGravity = rigidbody.gravityScale;
            }
        }


        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, objData => 
            {
                serializationData = objData;
                InitializeFromAvailibleData();
            });
        }
        public ISerializableObjectData GetSerializationData()=>
            serializationData.Clone() as MainCharacterData_0_3_0;
        public ISerializableObjectData GetDataOfCurrentState()=>
            new MainCharacterData_0_3_0(transform.position, isLeftSide);
        void ISerializableObject.OnStartLocationLoad() => enabled = false;
        void ISerializableObject.OnEndLocationLoad() => enabled = true;

        void IResetedEnvinronment.Reset()
        {
            if (serializationData == null)
                throw ServantException.GetArgumentNullException("data");
            InitializeFromAvailibleData();
        }

        MainCharacterController ISingltone<MainCharacterController>.Singltone
        {
            get => Registry.CharacterController;
            set => Registry.CharacterController = value;
        }
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public sealed class MainCharacterData_0_3_0 : ISerializableObjectData
    {
        public MainCharacterData_0_3_0() { }
        public MainCharacterData_0_3_0(Vector2 Position, bool IsLeftSide)
        {
            Position_ = Position;
            IsLeftSide_ = IsLeftSide;
        }
        public MainCharacterData_0_3_0(string json)
        {
            TryInitFromJson(json);
        }
        public Vector2 Position { get => Position_; private set => Position_ = value; }
        [SerializeField]
        private Vector2 Position_;
        public bool IsLeftSide { get => IsLeftSide_; private set => IsLeftSide_ = value; }
        [SerializeField]
        private bool IsLeftSide_;

        public int SerializationId => 0;
        public object Clone()
        {
            return new MainCharacterData_0_3_0(Position_, IsLeftSide_);
        }
        public void InstantiateObject()
        {
            this.InstanceAndInitialize< MainCharacterController>(EnvironmentPrefabs.MainCharacterPrefab);
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        public bool TryInitFromJson(string json)
        {
            Position_ = default;
            IsLeftSide = default;
            try
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
