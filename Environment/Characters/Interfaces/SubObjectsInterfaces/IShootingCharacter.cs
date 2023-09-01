using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servant.Characters
{
    public interface IShootingCharacter
    {
        public interface IRangeWeapon
        {
            public event Action<float> ShootEvent;
            public event Action CoolDownDoneEvent;
            public float CurrentCoolDown_ { get; }
            public void Shoot();
            public void RotateWeapon(float angle);
        }
        public event Action<float> ShootEvent;
        public event Action CoolDownDoneEvent;
        public void Shoot();
        /// <summary>
        /// </summary>
        /// <param name="angle">Rotation angle of weapon</param>
        public void Shoot(float angle);
        public void RotateWeapon(float angle);
    }
}
