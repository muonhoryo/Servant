
using UnityEngine;
using MuonhoryoLibrary;
using MuonhoryoLibrary.Unity;
using System;
using System.Threading;

namespace Servant
{
    public sealed class ServantThreadManager : ThreadManager, ISingltone<ServantThreadManager>
    {
        [SerializeField]
        private float maxFramTime;
        protected override float MaxFrameTime => maxFramTime;
        ServantThreadManager ISingltone<ServantThreadManager>.Singltone
        { get => Registry.ThreadManager;
            set => Registry.ThreadManager = value; }
        private void Awake()
        {
            this.ValidateSingltone();
            if (MaxFrameTime <= 0) throw new ServantException("MaxFrameTime assigned at wrong value.");
        }
        public short AddActionsQueue(Action mainThreadAction,Action onEndAction)
        {
            return AddActionsQueue(new Action[1] { mainThreadAction }, onEndAction);
        }
        public short AddActionsQueue(Action mainThreadAction,AutoResetEvent handler)
        {
            return AddActionsQueue(mainThreadAction, () => handler.Set());
        }
        public short AddActionsQueue(Action mainThreadAction)
        {
            return AddActionsQueue(mainThreadAction, () => { });
        }
    }
}
