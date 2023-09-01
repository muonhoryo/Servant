using MuonhoryoLibrary.Unity;
using MuonhoryoLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class CharacterGroundDirectionCalculator_FallCheckerComposite : Module,
        IGroundDirectionCalculatingModule, IFallingCheckingModule
    {
        public event Action<Vector2> RecalculateGroundDirectionEvent = delegate { };
        public event Action<IFallingCheckingModule.LandingInfo> LandingEvent = delegate { };
        public event Action<IFallingCheckingModule.FallingStartInfo> StartFallingEvent = delegate { };
        public event Action<IFallingCheckingModule.GroundFreeRisingInfo> StartRisingEvent = delegate { };
        public event Action<bool> ChangeVerticalMovingDirectionEvent = delegate { };
        public Vector2 GroundDirection_ { get; private set; } = Vector2.right;
        public IFallingCheckingModule.FallingState CurrentFallingState_ { get; private set; }
        public bool IsUp_ { get; private set; } = false;


        [SerializeField]
        private float GroundAngleRoundingCoef;

        private float GroundAngle= 0;
        private float PrevHeight = 0;
        private bool WasCollisedGround = false;
        private float Dot = 1;
        private float MinCollisionPointDistance = 10000;
        private int GroundNormalDir = 1;
        [SerializeField]
        private Rigidbody2D RGBody;
        [SerializeField]
        private Collider2D GroundSubChecker;

        private void FoundGround()
        {
            CurrentFallingState_ = IFallingCheckingModule.FallingState.StandingOnGround;
            GroundAngle = 0;
            GroundDirection_ = Vector2.right;
            GroundCheckingAction = GroundStayUpdAction;
            LandingEvent(new(RGBody.velocity));
            UpdateGroundAngle();
        }
        private void LostGround()
        {
            void ChangeDirectionAction(bool hasStartRising)
            {
                ChangeDirection(hasStartRising, true);
            }
            void ChangeDirection(bool hasStartRising,bool wasNotStayOnGround)
            {
                if (hasStartRising)
                {
                    CurrentFallingState_ = IFallingCheckingModule.FallingState.GroundFreeRising;
                    StartRisingEvent(new IFallingCheckingModule.GroundFreeRisingInfo(wasNotStayOnGround));
                }
                else
                {
                    CurrentFallingState_ = IFallingCheckingModule.FallingState.Falling;
                    StartFallingEvent(new IFallingCheckingModule.FallingStartInfo(wasNotStayOnGround));
                }
            }
            ChangeVerticalMovingDirectionEvent += ChangeDirectionAction;
            void ResetEvent(IFallingCheckingModule.LandingInfo i)
            {
                ChangeVerticalMovingDirectionEvent -= ChangeDirectionAction;
                LandingEvent -= ResetEvent;
            }
            LandingEvent += ResetEvent;
            GroundCheckingAction = FallingFixedUpdateAction;
            ChangeDirection(IsUp_, false);
        }
        private void UpdateGroundAngle()
        {
            float angle = DotToDegAngle(Dot, GroundNormalDir);
            if (angle != GroundAngle)
            {
                GroundAngle = angle;
                GroundDirection_ = angle.DirectionOfAngle();
                RecalculateGroundDirectionEvent(GroundDirection_);
            }
        }
        private float DotToDegAngle(float dot, int dir)
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
                if (WasCollisedWithAGround(collision, out float dot, out int normalDir))
                {
                    if (!WasCollisedGround)
                        WasCollisedGround = true;
                    float distance = collision.contacts.Min((contact) => MathF.Abs(transform.position.x - contact.point.x));
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
                if (groundDot > GroundDetectionMinCos_)
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
        private void FixedUpdate()
        {
            float currentHeight = transform.position.y;
            HeightHandlingAction(currentHeight);
            PrevHeight = transform.position.y;

            GroundCheckingAction();
            WasCollisedGround = false;
            MinCollisionPointDistance = 10000;
        }

        private Action<float> HeightHandlingAction;
        private void RiseHandling(float currentHeight)
        {
            if (currentHeight < PrevHeight)
            {
                IsUp_ = false;
                ChangeVerticalMovingDirectionEvent(false);
                HeightHandlingAction = DescentHandling;
            }
        }
        private void DescentHandling(float currentHeight)
        {
            if (currentHeight > PrevHeight)
            {
                IsUp_ = true;
                ChangeVerticalMovingDirectionEvent(true);
                HeightHandlingAction = RiseHandling;
            }
        }

        private Action GroundCheckingAction;
        private void GroundStayUpdAction()
        {
            if (WasCollisedGround)
            {
                UpdateGroundAngle();
            }
            else if (!HaveAnyGround())
            {
                LostGround();
            }
        }
        private void FallingFixedUpdateAction()
        {
            if (WasCollisedGround && HaveAnyGround())
            {
                FoundGround();
            }
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

        protected abstract float GroundDetectionMinCos_ { get; }
        protected sealed override bool CanTurnActivityFromOutside_ => false;
    }
}
