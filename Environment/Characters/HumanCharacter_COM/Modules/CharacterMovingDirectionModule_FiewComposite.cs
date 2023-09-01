using System;
using UnityEngine;
using MuonhoryoLibrary;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    /// <summary>
    /// Fiew direction equal moving direction.
    /// </summary>
    public sealed class CharacterMovingDirectionModule_FiewComposite : Module, IMovingDirectionChangingModule,
        IFiewDirectionChangingModule
    {
        public event Action<int> ChangeMovingDirectionEvent=delegate { };
        event Action<int> IFiewDirectionChangingModule.ChangeFiewDirectionEvent
        {
            add { ChangeMovingDirectionEvent += value; }
            remove { ChangeMovingDirectionEvent -= value; }
        }
        int IMovingDirectionChangingModule.MovingDirection_ => MovingDirection;
        public bool CanChangeMovingDirection_ 
        {
            get => CanChangeMovingDirection;
            set => CanChangeMovingDirection = value;
        }
        int IFiewDirectionChangingModule.FiewDirection_ => MovingDirection;
        bool IFiewDirectionChangingModule.CanChangeFiewDirection_
        { get => false;set { } }
        private int MovingDirection=1;
        private bool CanChangeMovingDirection=true;


        public void SetMovingDirection(int direction)
        {
            if (CanChangeMovingDirection)
            {
                if (direction == 0)
                    throw new ServantException("MovingDirection cannot be zero.");

                direction = direction.Sign();
                if (MovingDirection != direction)
                {
                    MovingDirection = direction;
                    BaseSprite.flipX = direction < 0;
                    ChangeMovingDirectionEvent(direction);
                }
            }
        }
        void IFiewDirectionChangingModule.SetFiewDirection(int direction) { }

        [SerializeField]
        private SpriteRenderer BaseSprite;

        private void Awake()
        {
            if (BaseSprite == null)
                if (!TryGetComponent(out BaseSprite))
                    throw ServantException.GetNullInitialization("BaseSprite");
        }

        protected override bool CanTurnActivityFromOutside_ => false;
    }
}
