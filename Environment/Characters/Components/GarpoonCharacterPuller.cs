
using System;
using UnityEngine;

namespace Servant.Characters
{
    public sealed class GarpoonCharacterPuller : GarpoonObjToPositionPuller
    {
        protected override bool Pull()
        {
            return PhysicalPulling(PulledObj_, ForceLevel_, GetTargetPosFunc_(),
                GlobalConstants.Singlton.Garpoon_PullDoneThreshold );
        }
        public void Initialize(float ForceLevel,Rigidbody2D Character, Func<Vector2> GetTargetPosFunc_)
        {
            InitializeAction(ForceLevel, GetTargetPosFunc_, Character);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (enabled&&collision.gameObject.layer == Registry.GroundLayer)
            {
                Vector2 dir=(GetTargetPosFunc_()-(Vector2)transform.position).normalized;
                foreach(var contact in collision.contacts)
                {
                    float dot = Vector2.Dot(contact.normal, dir);
                    if (dot <= GlobalConstants.Singlton.Garpoon_CollisionForceDirectionToStopPull)
                    {
                        CancelPull();
                        break;
                    }
                }
            }
        }
    }
}
