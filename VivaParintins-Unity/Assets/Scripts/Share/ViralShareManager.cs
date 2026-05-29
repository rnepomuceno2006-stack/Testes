// Coloque na Hierarquia: GameObject "ShareManager" na ResultScene ou MiniGameScene.
// Chame ViralShareManager.Share() ao final do Bumbódromo.
// Usa Android Intent nativo via AndroidJavaObject — funciona somente no device/APK.

using UnityEngine;
using VivaParintins.Core;

namespace VivaParintins.Share
{
    public class ViralShareManager : MonoBehaviour
    {
        public static ViralShareManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Chame ao final do Bumbódromo com a pontuação e estrelas
        public void Share(int score, int stars, bool playerWon)
        {
            string teamName = GameManager.Instance?.CurrentTeamData?.teamName ?? "meu boi";
            string emoji    = GameManager.Instance?.selectedTeam == Team.Garantido ? "❤️" : "⭐";

            string result = playerWon
                ? $"🏆 {teamName.ToUpper()} DOMINOU O BUMBÓDROMO!"
                : $"💪 {teamName} foi de coração até o fim!";

            string starsStr = new string('⭐', stars) + new string('☆', 3 - stars);

            string text =
                $"{emoji} VIVA PARINTINS!!! {emoji}\n\n" +
                $"{result}\n\n" +
                $"Pontuação: {score:N0} pontos\n" +
                $"Classificação: {starsStr}\n\n" +
                $"Jogue você também! #VivaParintins #FestivalDeParintins2026\n" +
                $"🎮 Baixe: bit.ly/vivaparintins";

#if UNITY_ANDROID && !UNITY_EDITOR
            ShareViaAndroidIntent(text);
#else
            // No editor, apenas loga
            Debug.Log("[Share] " + text);
            GUIUtility.systemCopyBuffer = text;
#endif
        }

        void ShareViaAndroidIntent(string text)
        {
            using var intentClass  = new AndroidJavaClass("android.content.Intent");
            using var intentObject = new AndroidJavaObject("android.content.Intent");

            intentObject.Call<AndroidJavaObject>("setAction",
                intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra",
                intentClass.GetStatic<string>("EXTRA_TEXT"), text);

            using var chooser = intentClass.CallStatic<AndroidJavaObject>(
                "createChooser", intentObject, "Compartilhar via...");

            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity    = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("startActivity", chooser);
        }
    }
}
