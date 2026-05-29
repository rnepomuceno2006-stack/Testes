using System.Collections;
using UnityEngine;
using TMPro;

namespace VivaParintins.UI
{
    public class ToastManager : MonoBehaviour
    {
        static ToastManager _instance;
        public TextMeshProUGUI toastText;
        public Animator toastAnimator;

        void Awake() { _instance = this; }

        public static void Show(string msg, float duration = 2.5f)
        {
            if (_instance) _instance.StartCoroutine(_instance.ShowRoutine(msg, duration));
        }

        IEnumerator ShowRoutine(string msg, float duration)
        {
            toastText.text = msg;
            toastAnimator.SetTrigger("Show");
            yield return new WaitForSeconds(duration);
            toastAnimator.SetTrigger("Hide");
        }
    }
}
