
using MuonhoryoLibrary;
using MuonhoryoLibrary.Unity;
using UnityEngine;

namespace Servant.Characters
{
    public sealed class GarpoonRope : MonoBehaviour,IGarpoonBase.IRope
    {
        [SerializeField]
        private SpriteRenderer RopeComp;
        private Transform Base;
        [SerializeField]
        private Transform Projectile;
        private void Update()
        {
            RopeComp.size = new Vector2(RopeComp.size.x, Vector2.Distance(Base.position,
                Projectile.position));
            transform.eulerAngles = transform.eulerAngles.GetEulerAngleOfImage
                ((Base.position - Projectile.position).AngleFromDirection() - 90);
        }
        void IGarpoonBase.IRope.Connect(Transform Base)
        {
            this.Base = Base;
        }
        private void Awake()
        {
            if (!enabled)
                enabled = true;
        }
        private void OnDisable()=>enabled = true;
    }
}
