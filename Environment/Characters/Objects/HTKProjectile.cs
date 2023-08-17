
using MuonhoryoLibrary.Unity;
using Servant.Control;
using System;
using UnityEngine;

namespace Servant.Characters
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class HTKProjectile : MonoBehaviour, IGarpoonBase.IProjectile
    {
        public event Action<IGarpoonBase.IGarpoonPullableObj.PullingTargetInfo> StartPullingEvent = delegate { };
        public event Action StopPullingEvent = delegate { };
        public event Action MissEvent=delegate { };
        public event Action<GameObject> HitEvent=delegate { };
        public event Action DestroyEvent=delegate { };

        [SerializeField]
        private float Speed;
        private float PassedDistance;
        private IGarpoonBase.IRope Rope;
        public GameObject HitObject_ { get; private set; }
        public float Speed_ => Speed;
        public Vector2 Direction_ { get; private set; }
        public float PassedDistance_ => PassedDistance;
        public Vector3 Position_ => transform.position;
        public bool IsPulled_ { get; private set; } = false;
        private void OnHitAction(Vector2 hitPosition,GameObject hitObj )
        {
            enabled = false;
            transform.position = hitPosition;
            HitObject_ = hitObj;
            HitEvent(HitObject_);
            if (HitObject_.layer == Registry.MovableItemLayer)
            {
                gameObject.transform.parent = HitObject_.transform;
            }
        }

        private void MovingAction()
        {
            void MoveTo(float stepLength)
            {
                RaycastHit2D hit = Physics2D.Raycast
                    (transform.position, Direction_, Speed_, Registry.GarpoonProjectileLayerMask);
                if (hit.collider != null)
                {
                    PassedDistance += hit.distance;
                    Vector2 hitPoint;
                    if (hit.collider.gameObject.TryGetComponent<IGarpoonBase.IHittableObj>(out var hitObj))
                        hitPoint = hitObj.HitPosition_;
                    else
                        hitPoint = hit.point;
                    OnHitAction(hitPoint, hit.collider.gameObject);
                }
                else
                {
                    transform.position += (Vector3)Direction_ * stepLength;
                    PassedDistance += stepLength;
                }
            }
            float remDist = GlobalConstants.Singlton.HTK_ProjectileMaxDistance - PassedDistance_;
            if (remDist < Speed_)
            {
                MoveTo(remDist);
                enabled = false;
            }
            else
            {
                MoveTo(Speed_);
            }
            if (PassedDistance_ >= GlobalConstants.Singlton.HTK_ProjectileMaxDistance)
            {
                MissEvent();
            }
        }
        private Action FixedUpdateAction;
        private void FixedUpdate()
        {
            FixedUpdateAction();
        }
        private void PullingRotationAction(float rotation)
        {
            transform.eulerAngles = transform.eulerAngles.GetEulerAngleOfImage(rotation);
        }
        private Action UpdateAction;
        private void Update()
        {
            UpdateAction();
        }
        private void TurnToMovingMode()
        {
            FixedUpdateAction = MovingAction;
            UpdateAction = delegate { };
        }
        private void TurnToPullingMode(Func<float> getRotationFunc)
        {
            FixedUpdateAction = delegate { };
            UpdateAction =()=> PullingRotationAction(getRotationFunc());
        }
        private void Awake()
        {
            Rope = GetComponentInChildren<IGarpoonBase.IRope>();
            if (Rope == null)
                throw ServantException.GetNullInitialization("Rope");

            TurnToMovingMode();

            if (!enabled)
                enabled = true;
        }
        void IGarpoonBase.IProjectile.Initialize(Transform Base,Vector2 Direction)
        {
            if (Direction == Vector2.zero)
                throw new ServantIncorrectInputArgument("Direction", "Direction cannot have zero length.");
            Direction = Direction.normalized;

            Direction_ = Direction;
            Rope.Connect(Base);
        }
        void IGarpoonBase.IProjectile.Destroy()
        {
            DestroyEvent();
            Destroy(gameObject);
        }
        void IGarpoonBase.IProjectile.ConnectRockingRopeJoint(DistanceJoint2D rope)
        {
            rope.connectedBody = GetComponent<Rigidbody2D>();
        }

        IGarpoonBase.IPuller IGarpoonBase.IGarpoonPullableObj.StartPullingToTarget
            (IGarpoonBase.IGarpoonPullableObj.PullingTargetInfo info)
        {
            var puller = gameObject.AddComponent<GarpoonProjectilePuller>();
            puller.Initialize(info.Speed, info.GetTargetPositionFunc,transform);
            puller.PullDoneEvent +=()=> IsPulled_ = false;
            puller.PullDoneEvent += () => StopPullingEvent();
            StartPullingEvent(info);
            IsPulled_ = true;
            if (!enabled)
                enabled = true;
            float GetRotationFunc()
            {
                return (transform.position - info.Target.transform.position).AngleFromDirection()-90;
            }
            TurnToPullingMode(GetRotationFunc);
            return puller;
        }
    }
}
