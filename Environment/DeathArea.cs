using Servant.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant
{
    public sealed partial class DeathArea:MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.TryGetComponent<IHitPointCharacter>(out var hpChar))
            {
                hpChar.Death();
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
