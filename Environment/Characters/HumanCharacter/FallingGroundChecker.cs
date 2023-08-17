using MuonhoryoLibrary;
using MuonhoryoLibrary.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Servant.Characters
{
    public sealed class FallingGroundChecker:MonoBehaviour,IFallingGroundChecker
    {
        [SerializeField]
        private float GroundAngleRoundingCoef;

        public event Action<float> UpdateGroundAngleEvent = delegate { };
        public event Action<IGroundCharacter.LandingInfo> LandingEvent = delegate { };
        /// <summary>
        /// Argument is true, if has start rising.
        /// </summary>
        public event Action<bool> LostGroundEvent = delegate { };
        public event Action<IGroundCharacter.GroundFreeRisingInfo> StartGroundFreeRisingEvent=delegate { };
        public event Action<IGroundCharacter.FallingStartInfo> StartFallingEvent=delegate { };
        /// <summary>
        /// Argument is true, if has start rising.
        /// </summary>
        public event Action<bool> ChangeVerticalMovingDirection=delegate { };

        public bool IsUp_ { get; private set; } = false;
        public bool IsOnAGround_ { get; private set; } = false;
        public float GroundAngle_ { get; private set; } = 0;
        private float PrevHeight = 0;
        private bool WasCollisedGround = false;
        private float Dot = 1;
        private float MinCollisionPointDistance=10000;
        private int GroundNormalDir = 1;
        [SerializeField]
        private Rigidbody2D RGBody;
        [SerializeField]
        private Collider2D GroundSubChecker;
        


        private void FixedUpdate()
        {
            float currentHeight = transform.position.y;
            HeightHandlingAction(currentHeight);
            PrevHeight = transform.position.y;

            GroundCheckingAction();
            WasCollisedGround = false;
            MinCollisionPointDistance = 10000;
        }
        private Action GroundCheckingAction;
        private Action<float> HeightHandlingAction;
        private void RiseHandling(float currentHeight)
        {
            if (currentHeight < PrevHeight)
            {
                IsUp_ = false;
                ChangeVerticalMovingDirection(false);
                HeightHandlingAction= DescentHandling;
            }
        }
        private void DescentHandling(float currentHeight)
        {
            if (currentHeight > PrevHeight)
            {
                IsUp_ = true;
                ChangeVerticalMovingDirection(true);
                HeightHandlingAction = RiseHandling;
            }
        }
        private void GroundStayUpdAction()
        {
            if (WasCollisedGround)
            {
                UpdateGroundAngle();
            }
            else if(!HaveAnyGround())
            {
                LostGround();
            }
        }
        private void FallingFixedUpdateAction()
        {
            if (WasCollisedGround&& HaveAnyGround())
            {
                FoundGround();
            }
        }
        private void FoundGround()
        {
            IsOnAGround_ = true;
            GroundAngle_ = 90;
            UpdateGroundAngle();
            GroundCheckingAction = GroundStayUpdAction;
            LandingEvent(new(RGBody.velocity));
        }
        private void LostGround()
        {
            void ChangeDirectionAction(bool hasStartRising)
            {
                if (hasStartRising)
                    StartGroundFreeRisingEvent(new IGroundCharacter.GroundFreeRisingInfo(true));
                else
                    StartFallingEvent(new IGroundCharacter.FallingStartInfo(true));
            }
            ChangeVerticalMovingDirection += ChangeDirectionAction;
            void ResetEvent(IGroundCharacter.LandingInfo i)
            {
                ChangeVerticalMovingDirection -= ChangeDirectionAction;
                LandingEvent -= ResetEvent;
            }
            LandingEvent += ResetEvent;
            IsOnAGround_ = false;
            GroundCheckingAction = FallingFixedUpdateAction;
            LostGroundEvent(IsUp_);
            if (IsUp_)
                StartGroundFreeRisingEvent(new IGroundCharacter.GroundFreeRisingInfo(false));
            else
                StartFallingEvent(new IGroundCharacter.FallingStartInfo(false));
        }
        private void UpdateGroundAngle()
        {
            float angle = DotToDegAngle(Dot, GroundNormalDir);
            if (angle != GroundAngle_)
            {
                GroundAngle_ = angle;
                UpdateGroundAngleEvent(GroundAngle_);
            }
        }
        private float DotToDegAngle(float dot,int dir)
        {
            float angle = MathF.Acos(dot) * Mathf.Rad2Deg;
            if (dir > 0)
                angle = 360 - angle;
            return angle.RoundTo(GroundAngleRoundingCoef);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.collider.gameObject.layer.IsInLayerMask(Registry.GroundLayerMask))
            {
                if (WasCollisedWithAGround(collision,out float dot,out int normalDir))
                {
                    if (!WasCollisedGround)
                        WasCollisedGround = true;
                    float distance = collision.contacts.Min((contact) =>MathF.Abs(transform.position.x-contact.point.x));
                    if (distance < MinCollisionPointDistance)
                    {
                        Dot = dot;
                        GroundNormalDir = normalDir;
                        MinCollisionPointDistance = distance;
                    }
                }
            }
        }
        /// <summary>
        /// If collised with a ground, return true.
        /// </summary>
        /// <param name="collision"></param>
        /// <param name="groundDot"></param>
        /// <returns></returns>
        private bool WasCollisedWithAGround(Collision2D collision, out float groundDot, out int normalDir)
        {
            groundDot = 0;
            foreach (var contact in collision.contacts)
            {
                groundDot = Vector2.Dot(contact.normal, Vector2.up);
                if (groundDot > GlobalConstants.Singlton.HumanCharacters_GroundDetectionMinCos)
                {
                    normalDir = contact.normal.x > 0 ? 1 : -1;
                    return true;
                }
            }
            normalDir = 0;
            return false;
        }
        private bool HaveAnyGround()
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(Registry.GroundLayerMask);
            filter.useTriggers = false;
            Collider2D[] i = new Collider2D[1];
            return Physics2D.OverlapCollider(GroundSubChecker, filter, i) > 0;
        }
        private void Awake()
        {
            if (RGBody == null)
                if (!TryGetComponent(out RGBody))
                    throw ServantException.GetNullInitialization("RGBody");
            if (GroundSubChecker == null)
                throw ServantException.GetNullInitialization("GroundSubChecker");

            HeightHandlingAction = DescentHandling;
            GroundCheckingAction = FallingFixedUpdateAction;

            if (!enabled)
                enabled = true;
        }
        private void Start()
        {
            PrevHeight = transform.position.y;
        }
        private void OnDisable()
        {
            enabled = true;
        }
    }
}
