
using System;
using UnityEngine;
using Servant.Serialization;
using System.Threading;

namespace Servant.InteractionObjects
{
    public class LocationTransit : DefaultInteractiveObject
    {
        public interface ILocationTransitInfo
        {
            public string NextLocationName_ { get; }
            public Vector2 NextMainCharacterPos_ { get; }
        }
        private ILocationTransitInfo TransitInfo;

        protected override void Interact()
        {
            SaveLoadSystem.EndLocationLoadingEvent += SetMainCharacterPosOnLoad;
            SaveLoadSystem.LoadLevel(TransitInfo.NextLocationName_);
        }
        private void SetMainCharacterPosOnLoad()
        {
            Registry.CharacterController_.transform.position = TransitInfo.NextMainCharacterPos_;
            SaveLoadSystem.EndLocationLoadingEvent -= SetMainCharacterPosOnLoad;
        }
        public void SetData(ILocationTransitInfo data) => TransitInfo = data;
    }
}
