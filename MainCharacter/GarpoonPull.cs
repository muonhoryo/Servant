
using System;
using UnityEngine;

namespace Servant.Control
{
    public sealed class GarpoonPull : MonoBehaviour
    {
        public float PullAcceleration { get; private set; }
        public Vector3 Direction { get; private set; }
        public Vector3 HookPoint { get; private set; }
        public float CurrentSpeed { get; private set; }
        private float PrevDistance;
        public event Action OnDonePull =Registry.EmptyMethod;
        private Rigidbody2D rgbody;
        private void FixedUpdate()
        {
            if(Vector3.Distance(transform.position, HookPoint) <= Registry.GarpoonPullDoneThreshold)
            {
                CancelPull();
            }
            else
            {
                rgbody.AddForce(Direction * CurrentSpeed, ForceMode2D.Force);
                CurrentSpeed += PullAcceleration;
            }
            float dist = Vector2.Distance(HookPoint, transform.position);
            if (dist > PrevDistance) CancelPull();
            else PrevDistance = dist;
        }
        /// <summary>
        /// Add force and destroy pull
        /// </summary>
        public void CancelPull()
        {
            OnDonePull();
            rgbody.AddForce(Direction * CurrentSpeed*Registry.GarpoonPullFinalImpulseMod, 
                ForceMode2D.Impulse);
            Destroy(this);
        }
        public void Initialize(float startPullSpeed,float PullAcceleration,Vector3 Direction,
            Vector3 HookPoint)
        {
            CurrentSpeed = startPullSpeed;
            this.PullAcceleration = PullAcceleration;
            this.Direction = Direction;
            this.HookPoint = HookPoint;
            rgbody = GetComponent<Rigidbody2D>();
            PrevDistance = Vector2.Distance(transform.position, HookPoint);
        }
        private void OnCollisionStay2D(Collision2D collision)
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
        }
    }
}
