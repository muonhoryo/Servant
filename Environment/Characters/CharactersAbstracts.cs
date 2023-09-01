using MuonhoryoLibrary.Collections;
using MuonhoryoLibrary;
using System;
using UnityEngine;
using MuonhoryoLibrary.Unity;
using System.Collections.Generic;

namespace Servant.Characters
{
    public abstract class GroundMovableCharacter : MonoBehaviour
    {
        protected void NoneAcceleratedGroundMoving(float speed)
        {
            NoneAcceleratedMoving(speed, GroundCorrectDirection_);
        }
        protected void NoneAcceleratedMoving(float speed, Vector2 direction)
        {
            Rigidbody_.velocity = speed * MovingDirection_ * direction;
        }
        protected void AcceleratedGroundMoving(float speed)
        {
            AcceleratedMoving(speed, GroundCorrectDirection_);
        }
        protected void AcceleratedMoving(float speed, Vector2 direction)
        {
            Rigidbody_.AddForce(speed * direction,ForceMode2D.Force);
        }
        protected void AcceleratedRockingMoving(IGarpoonOwner owner,float speed)
        {
            Vector2 forceDirection;
            if (transform.position.x > owner.GarpoonBase_.ShootedProjectile_.Position_.x != MovingDirection_ > 0)
            {
                forceDirection = new Vector2(MovingDirection_, 0);
            }
            else
            {
                Vector2 dir = (owner.GarpoonBase_.ShootedProjectile_.Position_ - transform.position).normalized;
                forceDirection = dir.GetRadialForceDirection(MovingDirection_);
            }
            Rigidbody_.AddForce(speed  * forceDirection,ForceMode2D.Force);
            IsLeftSide_ = transform.position.x > owner.GarpoonBase_.ShootedProjectile_.Position_.x;
        }
    }
}
