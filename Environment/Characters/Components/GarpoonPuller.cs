
using System;
using static Servant.Characters.IGarpoonBase;
using UnityEngine;

namespace Servant
{
    public abstract class GarpoonSimplePuller : MonoBehaviour, IPuller
    {
        protected bool IsInitialized = false;
        public event Action PullDoneEvent;
        private void FixedUpdate()
        {
            if (Pull())
                CancelPull();
        }
        public void CancelPull()
        {
            PullDoneEvent();
            enabled = false;
            Destroy(this);
        }
        /// <summary>
        /// Return true if pulling is done. Called in FixedUpdate. 
        /// </summary>
        /// <returns></returns>
        protected abstract bool Pull();
        private void Awake()
        {
            if (!enabled)
                enabled = true;
        }
        private void Start()
        {
            if (!IsInitialized)
                throw new ServantException("Puller was not initialized or initialization method doesn't realized in class. ");
        }
        private void OnDisable() =>
            enabled = true;



        /// <summary>
        /// Pull object to target pos while distance between object
        /// and target more than pullDoneThreshold.
        /// </summary>
        /// <param name="objRgBody"></param>
        /// <param name="forceLevel"></param>
        /// <param name="forceOffset"></param>
        /// <param name="ownerPosition"></param>
        /// <param name="targetPos"></param>
        /// <param name="pullDoneThreshold"></param>
        /// <returns></returns>
        protected static bool PhysicalPulling(Rigidbody2D objRgBody, float forceLevel,
            Vector2 targetPos, float pullDoneThreshold)
        {
            return PhysicalPullingAtPosition(objRgBody, forceLevel, Vector2.zero, targetPos, pullDoneThreshold);
        }
        /// <summary>
        /// Pull object to target pos while distance between object and target more than pullDoneThreshold.
        /// Force added at offset's position.
        /// (don't recalculate with object's rotation).
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        protected static bool PhysicalPullingAtPosition(Rigidbody2D objRgBody, float forceLevel,
            Vector2 forceOffset, Vector2 targetPos, float pullDonThreshold)
        {
            Vector2 objPos = objRgBody.transform.position;
            if (Vector3.Distance(objPos, targetPos) <= pullDonThreshold)
            {
                return true;
            }
            else
            {
                Vector2 forcePos = (Vector2)objRgBody.transform.position - forceOffset;
                Vector2 dir = Vector3.Normalize(targetPos - forcePos);
                objRgBody.AddForceAtPosition(dir * forceLevel , forcePos, ForceMode2D.Force);
                return false;
            }
        }
        /// <summary>
        /// Pull object to target pos while distance between object and target more than pullDoneThreshold.
        /// Pulling is not physical action.
        /// </summary>
        /// <param name="forceLevel"></param>
        /// <param name="targetPos"></param>
        /// <param name="pullDoneThreshold"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected static bool SimplePulling(float forceLevel,Vector2 targetPos,float pullDoneThreshold,Transform obj)
        {
            float distance = Vector3.Distance(obj.position, targetPos);
            if (distance <= pullDoneThreshold||distance<forceLevel)
                return true;
            else
            {
                Vector3 dir = Vector3.Normalize((Vector3)targetPos - obj.position);
                obj.position += dir * forceLevel;
                return false;
            }
        }
    }
    public abstract class GarpoonObjToPositionPuller : GarpoonSimplePuller
    {
        protected float ForceLevel_ { get; private set; }
        protected Rigidbody2D PulledObj_ { get; private set; }
        protected Func<Vector2> GetTargetPosFunc_ { get; private set; }
        protected virtual void InitializeAction(float ForceLevel, Func<Vector2> GetTargetPosFunc, Rigidbody2D Owner)
        {
            if (Owner == null)
                throw ServantException.GetNullInitialization("Owner");

            ForceLevel_ = ForceLevel; GetTargetPosFunc_ = GetTargetPosFunc; PulledObj_ = Owner;
            IsInitialized = true;
        }
    }
    public abstract class GarpoonObjToTargetPuller : GarpoonSimplePuller
    {
        protected float ForceLevel_ { get; private set; }
        protected Rigidbody2D PulledObj_ { get; private set; }
        protected GameObject Target_ { get; private set; }
        protected virtual void InitializeAction(float ForceLevel, GameObject Target, Rigidbody2D Owner)
        {
            if (Owner == null)
                throw ServantException.GetNullInitialization("Owner");

            ForceLevel_ = ForceLevel; Target_ = Target; PulledObj_ = Owner;
            IsInitialized = true;
        }
    }
}
