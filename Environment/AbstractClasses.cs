
using MuonhoryoLibrary;
using Servant.Serialization._0_3_0;
using System;
using UnityEngine;
using static Servant.Serialization.SaveLoadSystem;

namespace Servant.InteractionObjects
{
    public abstract class DefaultInteractiveObject:MonoBehaviour,IInteractiveObject
    {
        [SerializeField]
        private SpriteRenderer sprite;
        private bool IsHovered = false;
        private bool IsActive = false;
        protected bool IsActive_
        {
            get=>IsActive;
            set
            {
                if (!value)
                    sprite.enabled = false;
                else
                    sprite.enabled = IsHovered;
            }
        }
        protected bool IsHovered_
        {
            get=> IsHovered;
            set
            {
                IsHovered = value;
                if (IsActive_)
                    sprite.enabled = IsHovered;
            }
        }
        private void Awake()
        {
            if (sprite == null) throw ServantException.GetNullInitialization("sprite");
            sprite.enabled = false;
        }
        void IInteractiveObject.Hide()
        {
            IsHovered = false;
        }
        void IInteractiveObject.Interact() 
        {
            if (IsActive)
            {
                Interact();
            }
        } 
        protected abstract void Interact();
        void IInteractiveObject.Show()
        {
            IsHovered = true;
        }
    }
}
namespace Servant.Serialization
{
    public abstract class StaticEnvironmentSerComp<TSerData>: MonoBehaviour, ISerializableObject<TSerData>
        where TSerData :StaticEnvironmentEnvSerData_0_3_0
    {
        public abstract ISerializableObjectData GetSerializationData();

        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, objData => { transform.position = objData.Position_; });
        }
        ISerializableObjectData ISerializableObject.GetSerializationData() =>
            GetSerializationData();
        ISerializableObjectData ISerializableObject.GetDataOfCurrentState() =>
            GetSerializationData();
        void ISerializableObject.OnStartLocationLoad() { }
        void ISerializableObject.OnEndLocationLoad() { }
    }
    public abstract class MovableEnvironmentSerComp<TSerData> : MonoBehaviour, ISerializableObject<TSerData>,
        IResetedEnvinronment
        where TSerData:MovableEnvironmentSerData_0_3_0
    {
        public TSerData Data { get; protected set; }
        public abstract ISerializableObjectData GetDataOfCurrentState();
        public abstract ISerializableObjectData GetSerializationData();

        protected void SetTransform(TSerData info)
        {
            transform.position = info.Position_;
            transform.eulerAngles = transform.eulerAngles.GetEulerAngleOfImage(info.Rotation_);
        }

        ISerializableObjectData ISerializableObject.GetDataOfCurrentState() =>
            GetDataOfCurrentState();
        ISerializableObjectData ISerializableObject.GetSerializationData() =>
            GetSerializationData();
        void ISerializableObject.Initialize(ISerializableObjectData data)
        {
            this.ValidateInputAndInitialize(data, objData =>
            {
                Data = objData;
                SetTransform(Data);
            });
        }
        void ISerializableObject.OnEndLocationLoad() { }
        void ISerializableObject.OnStartLocationLoad() { }
        void IResetedEnvinronment.Reset() => SetTransform(Data);
    }
}
namespace Servant.Serialization._0_3_0
{
    [Serializable]
    public abstract class StaticEnvironmentEnvSerData_0_3_0 : ISerializableObjectData
    {
        public StaticEnvironmentEnvSerData_0_3_0() { }
        public StaticEnvironmentEnvSerData_0_3_0(Vector2 Position)
        {
            this.Position = Position;
        }
        public StaticEnvironmentEnvSerData_0_3_0(string json)
        {
            TryInitFromJson(json);
        }
        [SerializeField]
        private Vector2 Position;
        public Vector2 Position_ => Position;

        public abstract int SerializationID { get; }
        int ISerializableObjectData.SerializationId=> SerializationID;

        public abstract object Clone();
        public string ToJson() =>
            JsonUtility.ToJson(this);
        private bool TryInitFromJson(string json)
        {
            Position = default;
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

        protected abstract void InstantiateObject();
        void ISerializableObjectData.InstantiateObject() => InstantiateObject();
        bool ISerializableData.TryInitFromJson(string json) =>
            TryInitFromJson(json);
    }
    public abstract class MovableEnvironmentSerData_0_3_0 : ISerializableObjectData
    {
        public MovableEnvironmentSerData_0_3_0() { }
        public MovableEnvironmentSerData_0_3_0(Vector2 Position,
            float Rotation)
        {
            this.Position = Position;
            this.Rotation = Rotation;
        }
        public MovableEnvironmentSerData_0_3_0(string json)
        {
            TryInitFromJson(json);
        }
        [SerializeField]
        private Vector2 Position;
        [SerializeField]
        private float Rotation;
        public Vector2 Position_ => Position;
        public float Rotation_ => Rotation;

        public abstract int SerializationId { get; }
        int ISerializableObjectData.SerializationId => SerializationId;

        public abstract object Clone();
        public string ToJson() => JsonUtility.ToJson(this);
        private bool TryInitFromJson(string json)
        {
            Position = default;
            Rotation= default;
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

        protected abstract void InstantiateObject();
        void ISerializableObjectData.InstantiateObject() => InstantiateObject();
        bool ISerializableData.TryInitFromJson(string json) =>
            TryInitFromJson(json);
    }
}
