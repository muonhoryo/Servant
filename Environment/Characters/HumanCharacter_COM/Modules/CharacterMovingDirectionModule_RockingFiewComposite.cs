using MuonhoryoLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Servant.Characters.IGroundCharacter;

namespace Servant.Characters.COP
{
    public sealed class CharacterMovingDirectionModule_RockingFiewComposite:Module, IMovingDirectionChangingModule,
        IFiewDirectionChangingModule
    {
        public event Action<int> ChangeMovingDirectionEvent=delegate { };
        public event Action<int> ChangeFiewDirectionEvent=delegate { };
        int IMovingDirectionChangingModule.MovingDirection_ => MovingDirection;
        public bool CanChangeMovingDirection_
        {
            get => CanChangeMovingDirection;
            set => CanChangeMovingDirection = value;
        }
        int IFiewDirectionChangingModule.FiewDirection_ => FiewDirection;
        bool IFiewDirectionChangingModule.CanChangeFiewDirection_
        { get => false; set { } }
        private int MovingDirection=1;
        private int FiewDirection=1;
        private bool CanChangeMovingDirection;


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
                    ChangeMovingDirectionEvent(direction);
                }
            }
        }
        private void SetFiewDirection(int direction)
        {
            if (direction>0 != FiewDirection>0)
            {
                BaseSprite.flipX = direction < 0;
                FiewDirection = direction > 0 ? 1 : -1;
                ChangeFiewDirectionEvent(FiewDirection);
            }
        }
        void IFiewDirectionChangingModule.SetFiewDirection(int direction) { }

        [SerializeField]
        private SpriteRenderer BaseSprite;
        [SerializeField]
        private Component GarpoonBaseComponent;

        private IGarpoonBase GarpoonBase;

        private void Awake()
        {
            GarpoonBase = GarpoonBaseComponent as IGarpoonBase;
            if (GarpoonBase == null)
                throw ServantException.GetNullInitialization("GarpoonBase");
            if (BaseSprite == null)
                if (!TryGetComponent(out BaseSprite))
                    throw ServantException.GetNullInitialization("BaseSprite");
        }
        private void Update()
        {
            SetFiewDirection(transform.position.x > GarpoonBase.ShootedProjectile_.Position_.x ? -1 : 1);
        }
        private void Start()
        {
            ActivateEvent +=()=> enabled = true;
            DeactivateEvent += () => enabled = false;
        }
    }
}
