
using UnityEngine;
using MuonhoryoLibrary.Unity;
using System;

namespace Servant
{
    public sealed class GarpoonMovableItemPuller : GarpoonObjToTargetPuller
    {
        private Vector2 ForceOffset;
        private float InitializeAngle;
        /// <summary>
        /// ForceOffset recalculate with item rotation.
        /// </summary>
        /// <param name="ForceSpeed"></param>
        /// <param name="PulledItem"></param>
        /// <param name="Target"></param>
        /// <param name=""></param>
        public void Initialize(float ForceLevel,Rigidbody2D PulledItem,GameObject Target,Vector2 forceOffset)
        {
            ForceOffset = forceOffset;
            InitializeAngle = transform.eulerAngles.z;
            InitializeAction(ForceLevel, Target, PulledItem);
        }
        protected override bool Pull()
        {
            Vector2 offset = ForceOffset.AngleOffset(transform.eulerAngles.z+InitializeAngle);
            return PhysicalPullingAtPosition(PulledObj_, ForceLevel_, offset, Target_.transform.position,
                GlobalConstants.Singlton.Garpoon_PullDoneThreshold);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.gameObject == Target_)
                CancelPull();
        }
    }
}
