using System;

namespace Servant.Characters
{
    public interface IWallChecker
    {
        public event Action FoundLeftWallEvent;
        public event Action LostLeftWallEvent;
        public event Action FoundRightWallEvent;
        public event Action LostRightWallEvent;
        public bool IsThereLeftWall_ { get; }
        public bool IsThereRightWall_ { get; }
    }
}
