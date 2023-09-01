using System;

namespace Servant.Characters
{
    public interface ILayerChangableCharacter
    {
        public event Action GoingToBackGroundEvent;
        public event Action ReturnToCharactersLayerEvent;
        public bool IsInBackGroundLayer_ { get; }
    }
}
