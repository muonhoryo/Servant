
using System;
using UnityEngine;

namespace Servant
{
    public sealed class GarpoonProjectile:MonoBehaviour
    {
        public enum HitType
        {
            none,
            Ground,
            Item
        }
        public event Action ProjectileMissEvent;
        public event Action<GameObject> ProjectileHitEvent;
        public event Action HookTurnOffEvent;
        public float Speed { get; private set; } = 0;
        public Vector2 Direction { get; private set; } = Vector2.zero;
        public float MaxHookDistance { get; private set; } = -1f;
        public float MaxDistance { get; private set; } = -1f;
        public float PassedDistance { get; private set; } = -2;
        private void FixedUpdate()
        {
            void MoveTo(float stepLength)
            {
                RaycastHit2D hit = Physics2D.Raycast
                    (transform.position, Direction, Speed, Registry.GarpoonLayerMask);
                if (hit.collider!=null)
                {
                    PassedDistance += hit.distance;
                    enabled = false;
                    transform.position = hit.point;
                    ProjectileHitEvent?.Invoke(hit.collider.gameObject);
                    if (hit.collider.gameObject.layer == Registry.MovableItemLayer)
                    {
                        gameObject.transform.parent = hit.collider.gameObject.transform;
                    }
                }
                else
                {
                    transform.position += (Vector3)Direction * stepLength;
                    PassedDistance += stepLength;
                }
            }
            if (PassedDistance >= MaxDistance) Destroy(this);
            float remDist = MaxDistance - PassedDistance;
            if (remDist < Speed)
            {
                MoveTo(remDist);
                enabled = false;
            }
            else
            {
                MoveTo(Speed);
            }
            if (PassedDistance >= MaxHookDistance)
            {
                ProjectileMissEvent?.Invoke();
            }
        }
        public void TurnHookOff()
        {
            HookTurnOffEvent?.Invoke();
        }
        public void Initialize(Vector2 Direction, float Speed, float MaxDistance,float MaxHookDistance)
        {
            //!!!
            //Validate inputs
            //!!!
            if (PassedDistance < 0)
            {
                this.Direction = Direction;
                this.Speed = Speed;
                this.MaxDistance = MaxDistance;
                this.MaxHookDistance=MaxHookDistance;
                PassedDistance = 0;
            }
        }
    }
}
