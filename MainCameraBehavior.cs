
using UnityEngine;
using MuonhoryoLibrary;
using Servant.Serialization;
using System;

namespace Servant
{
    public sealed class MainCameraBehavior : MonoBehaviour, ISingltone<MainCameraBehavior>
    {
        private static MainCameraBehavior singltone;
        MainCameraBehavior ISingltone<MainCameraBehavior>.Singltone 
        { get => singltone;
            set => singltone=value; }
        private CameraState CurrentState;
        public event Action ChangeModeEvent = Registry.EmptyMethod;
        private abstract class CameraState
        {
            public CameraState()
            {

            }
            public abstract Vector2 GetNextPos();
            public virtual void OnTurningOffMode() { }
            public virtual void OnTurningOnMode() { }
        }
        private sealed class CameraTargetState:CameraState
        {
            public CameraTargetState(Func<Transform> GetTransformFunc)
            {
                this.GetTransformFunc = GetTransformFunc;
                Target = GetTransformFunc();
            }
            private readonly Func<Transform> GetTransformFunc;
            private Transform Target;
            public override Vector2 GetNextPos()
            {
                return Target!=null?Target.position:Vector2.zero;
            }
            void regetTarget()
            {
                Target = GetTransformFunc();
            }
            private void RegetTarget() => regetTarget();
            public override void OnTurningOffMode()
            {
                LocationSerializationData.EndLocationLoadingEvent -= RegetTarget;
            }
            public override void OnTurningOnMode()
            {
                LocationSerializationData.EndLocationLoadingEvent += RegetTarget;
            }
        }
        private void ChangeMode(CameraState state)
        {
            if (CurrentState != null) CurrentState.OnTurningOffMode();
            if (state == null) throw new ServantException("Turning camera mode to not existed state.");
            state.OnTurningOnMode();
            CurrentState = state;
            ChangeModeEvent();
        }
        private void LateUpdate()
        {
            Vector2 nextPos = CurrentState.GetNextPos();
            transform.position = new Vector3(nextPos.x, nextPos.y,transform.position.z);
        }
        public void Start()
        {
            this.ValidateSingltone();
            ChangeMode(new CameraTargetState(()=>Registry.CharacterController.transform));
        }
    }
}

