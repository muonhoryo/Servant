using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Servant.Characters.IGarpoonBase;
using UnityEngine;
using Servant.Characters.COP;
using static Servant.Characters.IGarpoonPullableObj;

namespace Servant.Characters
{
    public interface IGarpoonPullableObj
    {
        public interface IGarpoonPullingModule:IModule
        {
            public event Action<PullingTargetInfo> StartPullingEvent;
            public event Action StopPullingEvent;
            public Vector3 Position_ { get; }
            public bool IsPulled_ { get; }
            public IPuller StartPullingToTarget(PullingTargetInfo info);
        }
        public event Action<PullingTargetInfo> StartPullingEvent;
        public event Action StopPullingEvent;
        public struct PullingTargetInfo
        {
            public PullingTargetInfo(Func<Vector2> GetTargetPositionFunc, GameObject Target, float Speed,
                Vector2 ForceOffset)
            {
                if (GetTargetPositionFunc == null)
                    throw ServantException.GetArgumentNullException("GetTargetPositionFunc");
                this.GetTargetPositionFunc = GetTargetPositionFunc;
                this.Target = Target;
                this.Speed = Speed;
                this.ForceOffset = ForceOffset;
            }
            public Func<Vector2> GetTargetPositionFunc;
            public GameObject Target;
            public float Speed;
            public Vector2 ForceOffset;
        }
        public Vector3 Position_ { get; }
        public bool IsPulled_ { get; }

        protected IGarpoonPullingModule PullingModule_ { get; }
    }
    public interface IGarpoonPullableObj_ModuleChanging : IModuleChangingScript<IGarpoonPullingModule>
    {
        IGarpoonPullingModule IModuleChangingScript<IGarpoonPullingModule>.Module__
        { get => PullingModule_; set { PullingModule_ = value; } }
        protected IGarpoonPullingModule PullingModule_ { get; set; }
    }
}
