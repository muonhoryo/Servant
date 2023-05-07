
using System;
using UnityEngine;

namespace Servant.Control
{
    public abstract class GarpoonPull : MonoBehaviour
    {
        public float PullAcceleration { get; protected set; }
        public float CurrentSpeed { get; protected set; }
        public event Action PullDoneEvent;
        protected Rigidbody2D rgbody;
        public abstract Vector2 ForcePoint { get; }
        public abstract Vector2 TargetCurrentPosition { get; }
        public abstract Vector2 Direction { get; }
        public abstract float PullDoneThreshold { get; }
        public abstract float PullFinalImpulseModifier { get; }
        private void FixedUpdate()
        {
            if (Vector3.Distance(ForcePoint, TargetCurrentPosition) <= PullDoneThreshold)
            {
                CancelPull();
            }
            else
            {
                rgbody.AddForceAtPosition(Direction * CurrentSpeed*rgbody.mass, ForcePoint, ForceMode2D.Force);
                CurrentSpeed += PullAcceleration;
            }
        }
        /// <summary>
        /// Add force and destroy pull
        /// </summary>
        public void CancelPull()
        {
            PullDoneEvent();
            rgbody.AddForce(Direction * CurrentSpeed * PullFinalImpulseModifier,
                ForceMode2D.Impulse);
            Destroy(this);
        }
        protected void Initialize(float startPullSpeed, float PullAcceleration,Vector2 targetCurrentPos)
        {
            CurrentSpeed = startPullSpeed;
            this.PullAcceleration = PullAcceleration;
            rgbody = GetComponent<Rigidbody2D>();
        }
    }
    public sealed class GarpoonGroundPull : GarpoonPull
    {
        public Vector2 HookPoint { get; private set; }
        public Vector2 MoveDirection { get; private set; }

        public override Vector2 ForcePoint => transform.position;
        public override Vector2 Direction => MoveDirection;
        public override Vector2 TargetCurrentPosition => HookPoint;
        public override float PullDoneThreshold => Registry.GarpoonGroundPullDoneThreshold;
        public override float PullFinalImpulseModifier => Registry.GarpoonGroundPullFinalImpulseMod;
        public void Initialize(float startPullSpeed,float PullAcceleration,Vector2 HookPoint,
            Vector2 Direction)
        {
            Initialize(startPullSpeed, PullAcceleration,HookPoint);
            this.HookPoint = HookPoint;
            MoveDirection = Direction;
        }
        /*private void OnCollisionStay2D(Collision2D collision)
        {
            bool isHighImpulse()
            {
                foreach(var contact in collision.contacts)
                {
                    //if(contact.relativeVelocity.magnitude> Registry.GarpoonPullStopingImpulse)
                    float dot = Vector2.Dot(contact.normal, (Vector2)Direction);
                    if(dot<-Registry.GarpoonPullStopThreshold)
                    {
                        return true;
                    }
                }
                return false;
            }
            if (collision.gameObject.layer == Registry.GroundLayer&&
                isHighImpulse())
            {
                Registry.CharacterController.ResetVelocity();
                OnDonePull();
                Destroy(this);
            }
        }*/
    }
}
