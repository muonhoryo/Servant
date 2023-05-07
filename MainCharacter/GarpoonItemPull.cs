
using System;
using UnityEngine;

namespace Servant.Control
{
    public sealed class GarpoonItemPull : GarpoonPull
    {
        public Transform GarpoonOwner { get; private set; }
        public Transform Projectile { get; private set; }
        public override Vector2 ForcePoint => Projectile.position;
        public override Vector2 TargetCurrentPosition => GarpoonOwner.position;
        public override Vector2 Direction =>(GarpoonOwner.position-transform.position).normalized;
        public override float PullDoneThreshold => Registry.GarpoonItemPullDoneThreshold;
        public override float PullFinalImpulseModifier => Registry.GarpoonItemPullFinalImpulseMod;
        public void Initialize(float startPullSpeed,float PullAcceleration,Transform GarpoonOwner,
            Transform Projectile)
        {
            Initialize(startPullSpeed, PullAcceleration, GarpoonOwner.position);
            this.GarpoonOwner = GarpoonOwner;
            this.Projectile = Projectile;
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (enabled&&collision.collider.gameObject.transform == GarpoonOwner)enabled = false;
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!enabled && collision.collider.gameObject.transform == GarpoonOwner) enabled = true;
        }
    }
}
