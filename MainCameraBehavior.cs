
using UnityEngine;
using MuonhoryoLibrary;
using System;
using Servant.Serialization;
using static UnityEditor.PlayerSettings;

namespace Servant
{
    public sealed class MainCameraBehavior : MonoBehaviour, ISingltone<MainCameraBehavior>
    {
        public static MainCameraBehavior singltone { get; private set; }
        [SerializeField]
        private Camera CameraComp;
        MainCameraBehavior ISingltone<MainCameraBehavior>.Singltone 
        { get => singltone;
            set => singltone=value; }
        private CameraState CurrentState;
        public CameraMode CurrentCameraMode { get; private set; } = CameraMode.None;
        public event Action ChangeModeEvent;
        public enum CameraMode
        {
            None,
            Point,
            Target,
            GameCharacterTarget
        }
        private abstract class CameraState
        {
            public abstract void CheckCameraPosUpdate();
            public event Action<Vector2> CameraPositionUpdateEvent;
            protected void Run_CameraPositionUpdateEvent(Vector2 newCameraPos)
            {
                CameraPositionUpdateEvent?.Invoke(newCameraPos);
            }
            public event Action TurnOffEvent;
            public event Action TurnOnEvent;
            public void TurnOff()
            {
                TurnOffAction();
                TurnOffEvent?.Invoke();
            }
            public void TurnOn() 
            {
                TurnOnAction();
                TurnOnEvent?.Invoke();
            }
            protected virtual void TurnOnAction() { }
            protected virtual void TurnOffAction() { }
        }
        private class CameraPointState : CameraState
        {
            public CameraPointState(Vector2 Point)
            {
                this.Point = Point;
            }
            public Vector2 Point { get; private set; }
            public override void CheckCameraPosUpdate() { }
            public void SetPoint(Vector2 Point)
            {
                this.Point = Point;
                Run_CameraPositionUpdateEvent(Point);
            }
            protected override void TurnOnAction()
            {
                Run_CameraPositionUpdateEvent(Point);
            }
        }
        private class CameraTargetState:CameraState
        {
            public CameraTargetState(Func<Transform> GetTransformFunc)
            {
                this.GetTransformFunc = GetTransformFunc;
            }
            public event Action<Transform> ChangeTargetEvent;
            protected readonly Func<Transform> GetTransformFunc;
            protected Transform Target;
            public override void CheckCameraPosUpdate()
            {
                if (Target == null)
                    throw ServantCameraException.GetNullTargetException();
                else
                    Run_CameraPositionUpdateEvent(Target.position);
            }
            private void RegetTarget()
            {
                Target = GetTransformFunc();
                ChangeTargetEvent?.Invoke(Target);
            }
            protected override void TurnOnAction()
            {
                RegetTarget();
            }
        }
        private class CharacterControlState : CameraTargetState
        {
            public CharacterControlState(Func<Transform> getTransformFunc,Rect MoveLimit,
                Rect MoveTrigger)
            :base(getTransformFunc)
            {
                this.MoveLimit = MoveLimit;
                this.MoveTrigger = MoveTrigger;
                this.CurrentMoveTrigger= MoveTrigger;
            }
            public event Action<Rect> ChangeMoveLimitEvent;
            public event Action<Rect> ChangeMoveTriggerEvent;
            public Rect MoveLimit { get; private set; }
            public Rect MoveTrigger { get; private set; }
            public Rect CurrentMoveTrigger { get; private set; }
            public void SetLimit(Rect MoveLimit) 
            {
                this.MoveLimit = MoveLimit;
                ChangeMoveLimitEvent?.Invoke(MoveLimit);
            }
            public void SetTrigger(Rect MoveTrigger)
            {
                this.MoveTrigger = MoveTrigger;
                ChangeMoveTriggerEvent?.Invoke(MoveTrigger);
            }
            private void UpdateCurrentMoveTrigger(Vector2 charPos)
            {
                float x=CurrentMoveTrigger.xMin;
                float y = CurrentMoveTrigger.yMin;

                if (charPos.x > CurrentMoveTrigger.xMax)
                    if (charPos.x > MoveLimit.xMax)
                        x += (MoveLimit.xMax - CurrentMoveTrigger.yMax);
                    else
                        x += (charPos.x - CurrentMoveTrigger.xMax);
                else if (charPos.x < CurrentMoveTrigger.xMin)
                    if (charPos.x < MoveLimit.xMin)
                        x = MoveLimit.xMin;
                    else
                        x = charPos.x;

                if (charPos.y > CurrentMoveTrigger.yMax)
                    if (charPos.y > MoveLimit.xMax)
                        y += (MoveLimit.yMax - CurrentMoveTrigger.yMax);
                    else
                        y += (charPos.y - CurrentMoveTrigger.yMax);
                else if (charPos.y < CurrentMoveTrigger.yMin)
                    if(charPos.y<MoveLimit.yMin)
                        y= MoveLimit.yMin;
                    else
                        y = charPos.y;

                CurrentMoveTrigger=new Rect(x, y, CurrentMoveTrigger.width, CurrentMoveTrigger.height);
            }
            public override void CheckCameraPosUpdate()
            {
                if (Target == null)
                    throw ServantCameraException.GetNullTargetException();
                else
                {
                    if (!CurrentMoveTrigger.Contains(Target.transform.position))
                    {
                        UpdateCurrentMoveTrigger(Target.transform.position);
                        Run_CameraPositionUpdateEvent(CurrentMoveTrigger.center);
                    }
                }
            }
        }


        private void OnCameraPositionUpdateAction(Vector2 newPos)
        {
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }
        private void ChangeMode(CameraState state,CameraMode mode)
        {
            CurrentState?.TurnOff();
            if (state == null) throw new ServantException("Turning camera mode to not existed state.");
            state.TurnOn();
            CurrentState = state;
            CurrentState.CameraPositionUpdateEvent += OnCameraPositionUpdateAction;
            CurrentCameraMode = mode;
            ChangeModeEvent?.Invoke();
        }

        public void CharCtrl_SetLimit(Rect limit)
        {
            CharacterControlState state=VerificateStateChangesCorrection<CharacterControlState>();
            state.SetLimit(limit);
        }
        public void CharCtrl_SetTrigger(Rect trigger)
        {
            CharacterControlState state = VerificateStateChangesCorrection<CharacterControlState>();
            state.SetTrigger(trigger);
        }
        public void SetPointState(Vector2 point)
        {
            ChangeMode(new CameraPointState(point),CameraMode.Point);
        }
        public void SetTargetState(Transform target)
        {
            ChangeMode(new CameraTargetState(() => target),CameraMode.Target);
        }
        public void SetCharCtrlState(Transform character,Rect limit=default,Rect trigger=default)
        {
            CharacterControlState state = new CharacterControlState
                (() => character,
                limit,
                trigger);
            ChangeMode(state,CameraMode.GameCharacterTarget);
        }



        /// <summary>
        /// If type of state is input type return current state as input type
        /// </summary>
        /// <typeparam name="CameraStateType"></typeparam>
        /// <returns></returns>
        /// <exception cref="ServantException"></exception>
        private CameraStateType VerificateStateChangesCorrection<CameraStateType>()
            where CameraStateType : CameraState
        {
            if (CurrentState is not CameraStateType state)
                throw new ServantException("Trying to changes params of current state as an other state.");
            else
                return state;
        }
        public Vector2 GetCursorPos() => CameraComp.ScreenToWorldPoint
                (new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.y));
        //UnityAPI
        private void LateUpdate()
        {
            if (CurrentState != null)
                CurrentState.CheckCameraPosUpdate();
        }
        public void Start()
        {
            this.ValidateSingltone();
            SetPointState(Vector2.zero);
        }
    }
}

