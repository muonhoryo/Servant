using Servant.Characters;
using Servant.Characters.COP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant
{
    public abstract class LinearPuller:Module,IGarpoonBase.IPullingModule.IPuller
    {
        public event Action StartPullingEvent = delegate { };
        public event Action StopPullingEvent=delegate { };

        protected abstract float PullingSpeed_ { get; }

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
                objRgBody.AddForceAtPosition(dir * forceLevel, forcePos, ForceMode2D.Force);
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
        protected static bool SimplePulling(float forceLevel, Vector2 targetPos, float pullDoneThreshold, Transform obj)
        {
            float distance = Vector3.Distance(obj.position, targetPos);
            if (distance <= pullDoneThreshold || distance < forceLevel)
                return true;
            else
            {
                Vector3 dir = Vector3.Normalize((Vector3)targetPos - obj.position);
                obj.position += dir * forceLevel;
                return false;
            }
        }
        private float PullSpeed;
        private Func<Vector2> GetTargetPosFunc;
        private Transform Owner;
        public void Initialize(float PullSpeed, Func<Vector2> GetTargetPosFunc, Transform Owner)
        {
            if (PullSpeed <= 0)
                throw new ServantIncorrectInputArgument("PullSpeed", "PullSpeed cannot be less or equal zero.");
            if (GetTargetPosFunc == null)
                throw ServantException.GetNullInitialization("GetTurgetPosFunc");
            if (Owner == null)
                throw ServantException.GetNullInitialization("Owner");

            this.PullSpeed = PullSpeed;
            this.GetTargetPosFunc = GetTargetPosFunc;
            this.Owner = Owner;
            IsInitialized = true;
        }
        protected override bool Pull()
        {
            return SimplePulling(PullSpeed, GetTargetPosFunc(), GlobalConstants.Singlton.Garpoon_PullDoneThreshold, Owner);
        }
    }
}
