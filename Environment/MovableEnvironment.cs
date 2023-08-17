using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Servant;
using Servant.Characters;

namespace Servant
{
    public sealed class MovableEnvironment : MonoBehaviour, IGarpoonBase.IGarpoonPullableObj
    {
        public Vector3 Position_ => transform.position;
        public bool IsPulled_ { get; private set; } = false;
        public event Action<IGarpoonBase.IGarpoonPullableObj.PullingTargetInfo> StartPullingEvent=delegate { };
        public event Action StopPullingEvent = delegate { };
        IGarpoonBase.IPuller IGarpoonBase.IGarpoonPullableObj.StartPullingToTarget
            (IGarpoonBase.IGarpoonPullableObj.PullingTargetInfo info)
        {
            var puller = gameObject.AddComponent<GarpoonMovableItemPuller>();
            var rgbody=GetComponent<Rigidbody2D>();
            if (rgbody == null)
                throw ServantException.GetNullInitialization("Rigidbody2D");
;            puller.Initialize(info.Speed, rgbody, info.Target, info.ForceOffset);
            puller.PullDoneEvent += () => StopPullingEvent();
            StartPullingEvent(info);
            return puller;
        }
    }
}
