
using UnityEngine;
using Servant.GUI;
using Servant.Serialization;

namespace Servant.InteractionObjects
{
    public sealed class TextShower : DefaultInteractiveObject
    {
        public interface ITextShowerInfo
        {
            public string ShowedText_ { get; }
            public Vector2 TextShowingOffset_ { get; }
            public float ShowTime_ { get; }
        }
        private ITextShowerInfo TextShowingInfo;
        protected override void Interact()
        {
            GUIManager.InitializeNewTempleText((Vector2)transform.position+ TextShowingInfo.TextShowingOffset_,
                TextShowingInfo.ShowedText_, TextShowingInfo.ShowTime_);
        }
        public void SetData(ITextShowerInfo data)=> TextShowingInfo = data;
    }
}
