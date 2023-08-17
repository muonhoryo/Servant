using MuonhoryoLibrary.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public sealed class WallChecker:MonoBehaviour, IWallChecker
    {
        public event Action FoundLeftWallEvent=delegate { };
        public event Action LostLeftWallEvent=delegate { };
        public event Action FoundRightWallEvent=delegate { };
        public event Action LostRightWallEvent=delegate { };
        public bool IsThereLeftWall_ { get; private set; } = false;
        public bool IsThereRightWall_ { get; private set; } = false;
        private bool WasCollisedByRight = false;
        private bool WasCollisedByLeft = false;

        private void FixedUpdate()
        {
            if (IsThereLeftWall_ != WasCollisedByLeft)
            {
                if (WasCollisedByLeft)
                    FoundLeftWallEvent();
                else
                    LostLeftWallEvent();

                IsThereLeftWall_ = WasCollisedByLeft;
            }
            if (IsThereRightWall_ != WasCollisedByRight)
            {
                if (WasCollisedByRight)
                    FoundRightWallEvent();
                else
                    LostRightWallEvent();

                IsThereRightWall_ = WasCollisedByRight;
            }
            if (WasCollisedByLeft)
                WasCollisedByLeft = false;
            if (WasCollisedByRight)
                WasCollisedByRight = false;
        }
        /// <summary>
        /// Return 1, if collised with a wall on right side. -1, if on left side.
        /// Return 0, if collised with not a wall.
        /// </summary>
        /// <param name="collision"></param>
        /// <returns></returns>
        private int WasCollisedWithAWall(Collision2D collision)
        {
            foreach (var contact in collision.contacts)
            {
                Vector2 dir = contact.point.x < transform.position.x ? Vector2.left : Vector2.right;
                float dot = Math.Abs(Vector2.Dot(contact.normal, dir));
                if (dot > GlobalConstants.Singlton.HumanCharacters_WallDetectionMinCos)
                {
                    if (dir.x > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!WasCollisedByLeft || !WasCollisedByRight&&
                collision.collider.gameObject.layer.IsInLayerMask(Registry.GroundLayerMask))
            {
                int dir = WasCollisedWithAWall(collision);
                if (dir > 0&&!WasCollisedByRight)
                {
                    WasCollisedByRight = true;
                }
                else if(dir< 0&&!WasCollisedByLeft)
                {
                    WasCollisedByLeft = true;
                }
            }
        }
        private void Awake()
        {
            if (!enabled)
                enabled = true;
        }
        private void OnDisable()
            => enabled = true;
    }
}
