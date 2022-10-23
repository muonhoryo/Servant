
using System.Collections;
using UnityEngine;
using MuonhoryoLibrary;

namespace Servant.DevelopmentOnly
{
    public sealed class VelocityChangeTest : MonoBehaviour, ISingltone<VelocityChangeTest>
    {
        private static VelocityChangeTest singltone;
        VelocityChangeTest ISingltone<VelocityChangeTest>.Singltone
        { get => singltone; set => singltone = value; }
        private void OnValidate()
        {
            this.ValidateSingltone();
            if (Rigidbody == null) Rigidbody = GetComponent<Rigidbody2D>();
        }
        public float Delay = 1f;
        private Vector2 Velocity = Vector2.zero;
        private Rigidbody2D Rigidbody;
        private IEnumerator ShowInfo()
        {
            while (true)
            {
                Debug.Log(Velocity);
                yield return new WaitForSeconds(Delay);
            }
        }
        private void Update()
        {
            Velocity += Rigidbody.velocity;
            Velocity *= 0.5f;
        }
        private void Awake()
        {
            StartCoroutine(ShowInfo());
        }
    }
}
