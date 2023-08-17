using MuonhoryoLibrary.Collections;
using MuonhoryoLibrary.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace Servant.Characters
{
    public sealed class ClimbableGroundChecker : MonoBehaviour, IClimbableGroundChecker
    {
        public float CastYGroundAngleOffset_
        {
            get
            {
                float radius = CastVoidCheckerCollider.direction == CapsuleDirection2D.Vertical ?
                    CastVoidCheckerCollider.size.x : CastVoidCheckerCollider.size.y;
                float angle = GlobalConstants.Singlton.HumanCharacters_GroundForceDodgeMinAngle * Mathf.Deg2Rad;
                return (radius / Mathf.Cos(angle)) - radius;
            }
        }

        public event Action<GameObject, int> FoundClimbableGroundEvent=delegate { };
        public event Action LostClimbableGroundEvent = delegate { };
        GameObject IClimbableGroundChecker.ClimbableGround_ => ClimbableGround.gameObject;
        int IClimbableGroundChecker.ClimbableGroundDirection_ => GroundDirection;
        private Collider2D ClimbableGround;
        private Vector2 ClimbPos;
        private int GroundDirection=1;
        [SerializeField]
        private CapsuleCollider2D CastVoidCheckerCollider;
        [SerializeField]
        private Vector2 NearestPointCastOffset;
        [SerializeField]
        private Collider2D[] CheckerColliders;

        Vector2 IClimbableGroundChecker.GetClimbPosition() =>ClimbPos;
        bool IClimbableGroundChecker.HasClimbableGroundAround() => HasClimbableGroundAround();
        private bool HasClimbableGroundAround()=>
            ClimbableGround != null;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject == ClimbableGround)
            {
                Vector2 climbPos;
                if (!IsClimbableGround(collision, out climbPos))
                    LostGround();
                else
                    ClimbPos = climbPos;
            }
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (IsClimbableGround(collider,out Vector2 climbPos))
            {
                ClimbPos = climbPos;
                FoundGround(collider);
            }
        }
        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider==ClimbableGround)
                LostGround();
        }
        private bool IsClimbableGround(Collider2D ground,out Vector2 ClimbPos)
        {
            if (ground.gameObject.layer.IsInLayerMask(Registry.GroundLayerMask))
            {
                Vector2 nearestPoint = ground.ClosestPoint((Vector2)transform.position + NearestPointCastOffset);
                Vector2 castPos = new Vector2
                    (nearestPoint.x,
                    nearestPoint.y + (CastVoidCheckerCollider.size.y / 2)+CastYGroundAngleOffset_);
                var cast = Physics2D.OverlapCapsule(castPos, CastVoidCheckerCollider.size, CastVoidCheckerCollider.direction, 0,
                    Registry.GroundLayerMask);
                ClimbPos = Physics2D.CapsuleCast(castPos, CastVoidCheckerCollider.size, CastVoidCheckerCollider.direction, 0,
                    Vector2.down,float.MaxValue, Registry.GroundLayerMask).point;
                return cast == null;
            }
            else
            {
                ClimbPos = Vector2.zero;
                return false;
            }
        }
        private int GetDirectionOfCollider(Collider2D ground)
        {
            return transform.position.x > ground.transform.position.x ? -1 : 1;
        }
        private Collider2D GetClimbableGroundAround(out Vector2 climbPos)
        {
            SingleLinkedList<Collider2D> castsList = new SingleLinkedList<Collider2D> { };
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask= Registry.GroundLayerMask;
            filter.useTriggers = true;
            foreach(var collider in CheckerColliders)
            {
                Collider2D[] cast=new Collider2D[1];
                if(Physics2D.OverlapCollider(collider, filter, cast) > 0)
                    castsList.AddLast(cast[0]);
            }
            foreach(var collider in castsList)
            {
                if (IsClimbableGround(collider,out climbPos))
                    return collider;
            }
            climbPos = Vector2.zero;
            return null;
        }

        private IEnumerator GroundAssigningDelay(bool IsAssigning)
        {
            yield return new WaitForFixedUpdate();
            if (IsAssigning)
            {
                if (ClimbableGround != null)
                    FoundClimbableGroundEvent(ClimbableGround.gameObject,GroundDirection);
            }
            else 
            {
                if (ClimbableGround == null)
                    LostClimbableGroundEvent();
            }
        }
        private void FoundGround(Collider2D ground)
        {
            ClimbableGround = ground;
            GroundDirection = GetDirectionOfCollider(ground);
            StartCoroutine(GroundAssigningDelay(true));
        }
        private void LostGround()
        {
            var ground= GetClimbableGroundAround(out Vector2 climbPos);
            if (ground != null)
            {
                ClimbPos = climbPos;
                FoundGround(ground);
            }
            else
            {
                ClimbableGround = null;
                StartCoroutine(GroundAssigningDelay(false));
            }
        }

        private void Awake()
        {
            if (CastVoidCheckerCollider == null)
                throw ServantException.GetNullInitialization("CastVoidCheckerCollider");
            if (CheckerColliders == null)
                throw ServantException.GetNullInitialization("CheckerColliders");

            if (!enabled)
                enabled = true;
        }
        private void OnDisable() => enabled = true;
    }
}
