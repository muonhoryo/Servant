using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.Characters
{
    public sealed class MeleeHitBox : MonoBehaviour, IMeleeFightCharacter.IMeleeHitBox 
    {
        private IMeleeFightCharacter Owner;
        bool IMeleeFightCharacter.IMeleeHitBox.IsActive_
        {
            get => enabled;
            set=>enabled = value;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.TryGetComponent(out IHitPointCharacter hitObj)&&
                hitObj!=Owner)
            {
                hitObj.TakeDamage(Owner.IsStrongShoot_);
            }
        }
        private void Awake()
        {
            if(enabled)
                enabled = false;
        }
        private void Start()
        {
            if (Owner == null)
            {
                Owner = GetComponentInParent<IMeleeFightCharacter>();
                if (Owner == null)
                    ServantException.GetNullInitialization("Owner");
            }
               
        }
    }
}
