
using UnityEngine;
using UnityEngine.UI;

namespace Servant.GUI
{
    public sealed class TempleText : MonoBehaviour
    {
        [SerializeField]
        private Text text;
        private System.Collections.IEnumerator DestroyDelay(float destroyDelay)
        {
            yield return new WaitForSeconds(destroyDelay);
            Destroy(gameObject);
            yield break;
        }
        public void Initialize(string showedText,float DestroyDelayValue)
        {
            text.text= showedText;
            StartCoroutine(DestroyDelay(DestroyDelayValue));
        }
        private void Start()
        {
            if (text == null) throw ServantException.GetNullInitialization("text");
        }
    }
}
