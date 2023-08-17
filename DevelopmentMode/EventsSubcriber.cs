using Servant.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Servant.DevelopmentOnly
{
    public abstract class EventsSubcriber<TEventsOwnerType> : MonoBehaviour
    {
        private void Awake()
        {
            if (!TryGetComponent(out TEventsOwnerType i))
                Destroy(this);
        }
        private void Start()
        {
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            var character = GetComponent<TEventsOwnerType>();
            SubscribeAction(character);
        }
        protected abstract void SubscribeAction(TEventsOwnerType owner);
    }
}
