
using UnityEngine;
using MuonhoryoLibrary;
using MuonhoryoLibrary.Unity;
using System;
using System.Threading;
using System.Collections.Generic;
using static Servant.Serialization.SaveLoadSystem;
using Servant.GUI;

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
        void ISingltone<ServantThreadManager>.Destroy() =>
            Destroy(this);

        private void Awake()
        {
            this.ValidateSingltone();
            if (MaxFrameTime <= 0) throw new ServantException("MaxFrameTime assigned at wrong value.");
        }
    }
    public abstract class AsyncFacade
    {
        protected AsyncFacade() 
        {
            InitializeEvent += InitializeAction;
            StartEvent += StartAction;
            EndEvent += EndAction;
        }
        protected readonly AutoResetEvent Handler = new AutoResetEvent(false);
        protected Thread CurrentThread;
        private static object lockObj = new object();

        protected void WaitAndReset()
        {
            Handler.WaitOne();
            Handler.Reset();
        }
        protected List<ISerializableObject> GetSerializedObjsInSceneAsync()
        {
            AutoResetEvent handler = new AutoResetEvent(false);
            List<ISerializableObject> serializatedObjects = new List<ISerializableObject>(0);
            void FindObjets()
            {
                GameObject[] objects = new GameObject[0];
                objects = GameObject.FindGameObjectsWithTag(ServantSerializationTag);
                serializatedObjects = new List<ISerializableObject>(objects.Length);
                foreach (GameObject obj in objects)
                {
                    serializatedObjects.Add(obj.GetComponent<ISerializableObject>());
                }
            }
            Registry.ThreadManager.AddActionsQueue(FindObjets, handler);
            handler.WaitOne();
            return serializatedObjects;
        }
        /// <summary>
        /// Check on nullable
        /// </summary>
        /// <param name="ev"></param>
        protected void DelegateEventExecutingToTM(Action ev)
        {
            if (ev != null)
                Registry.ThreadManager.AddActionsQueue(ev, Handler);
            else
                Handler.Set();
        }
        protected void RunAsyncAndWait(Action runningAsyncAction)
        {
            runningAsyncAction();
            WaitAndReset();
        }

        public event Action InitializeEvent;
        public event Action StartEvent;
        public event Action EndEvent;
        protected virtual void InitializeAction() { }
        protected virtual void StartAction() { }
        protected virtual void EndAction() { }

        public void InitializeAndStart()
        {
            InitializeEvent();
            CurrentThread = new Thread(new ThreadStart(AsyncExecute));
            Initialize();
            CurrentThread.Start();
        }
        protected abstract void Initialize();
        private void AsyncExecute()
        {
            lock (lockObj)
            {
                DelegateEventExecutingToTM(StartEvent);
                WaitAndReset();
                AsyncAction();
                Registry.ThreadManager.AddActionsQueue(EndEvent);
            }
        }
        protected abstract void AsyncAction();
    }
}
