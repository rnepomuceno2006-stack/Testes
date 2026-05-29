// Coloque na Hierarquia: GameObject "EconomyManager" (DontDestroyOnLoad).
// Gerencia moedas, consumíveis e estrutura de loja do jogo.

using System.Collections;
using UnityEngine;
using VivaParintins.UI;

namespace VivaParintins
{
    public class GameEconomyManager : MonoBehaviour
    {
        public static GameEconomyManager Instance { get; private set; }

        // ── Chaves PlayerPrefs ───────────────────────────────────────────
        const string KeyMicangas   = "eco_micangas";
        const string KeyPenas      = "eco_penas";
        const string KeyShields    = "eco_shields";

        // ── Estado ───────────────────────────────────────────────────────
        public int Micangas  { get; private set; }
        public int PenasOuro { get; private set; }

        int _shields;
        bool _magnetActive;
        Coroutine _magnetRoutine;

        // Referência ao jogador para o ímã puxar moedas
        [HideInInspector] public Player.PlayerMobileController activePlayer;

        // ── Unity lifecycle ──────────────────────────────────────────────
        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        // ── Moeda Soft: Miçangas ─────────────────────────────────────────

        public void AddMicangas(int amount)
        {
            Micangas += amount;
            Save();
        }

        public bool SpendMicangas(int amount)
        {
            if (Micangas < amount)
            {
                ToastManager.Show("Miçangas insuficientes!");
                return false;
            }
            Micangas -= amount;
            Save();
            return true;
        }

        // ── Moeda Hard: Penas de Ouro (IAP simulado) ─────────────────────

        public void AddPenasOuro(int amount)
        {
            PenasOuro += amount;
            Save();
        }

        // Simula compra IAP — conecte ao plugin de IAP real (Unity IAP / Google Play Billing)
        public void BuyPremiumItem(string itemId)
        {
            switch (itemId)
            {
                case "penas_100":
                    AddPenasOuro(100);
                    ToastManager.Show("✨ 100 Penas de Ouro adicionadas!");
                    break;
                case "penas_500":
                    AddPenasOuro(500);
                    ToastManager.Show("✨ 500 Penas de Ouro adicionadas!");
                    break;
                default:
                    Debug.LogWarning($"[Economy] Item desconhecido: {itemId}");
                    break;
            }
        }

        // ── Consumíveis ──────────────────────────────────────────────────

        // Garrafa de Guaraná — auto-revive ou compra dash
        public bool TryUseGuarana()
        {
            if (!SpendMicangas(30)) return false;
            activePlayer?.ActivateDash();
            ToastManager.Show("🥤 Guaraná! Velocidade máxima!");
            return true;
        }

        // Escudo de Palha — absorve 1 colisão
        public void AddShield(int count = 1)
        {
            _shields = Mathf.Min(_shields + count, 3);
            Save();
        }

        public bool TryUseShield()
        {
            if (_shields <= 0) return false;
            _shields--;
            Save();
            ToastManager.Show("🛡️ Escudo de Palha absorveu o impacto!");
            return true;
        }

        // Ímã da Galera — atrai moedas por 8 segundos usando MoveTowards
        public void ActivateMagnet(float duration = 8f)
        {
            if (_magnetRoutine != null) StopCoroutine(_magnetRoutine);
            _magnetRoutine = StartCoroutine(MagnetRoutine(duration));
        }

        IEnumerator MagnetRoutine(float duration)
        {
            _magnetActive = true;
            ToastManager.Show("🧲 Ímã da Galera ativado!");
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (activePlayer != null)
                    PullNearbyCoins(activePlayer.transform.position);
                yield return null;
            }
            _magnetActive = false;
        }

        void PullNearbyCoins(Vector3 playerPos)
        {
            // Encontra todas as moedas no raio de 8 unidades e as move para o jogador
            var coins = Physics.OverlapSphere(playerPos, 8f);
            foreach (var c in coins)
            {
                if (!c.CompareTag("Coin")) continue;
                c.transform.position = Vector3.MoveTowards(
                    c.transform.position, playerPos, 20f * Time.deltaTime);
            }
        }

        // ── Persistência ─────────────────────────────────────────────────

        void Save()
        {
            PlayerPrefs.SetInt(KeyMicangas, Micangas);
            PlayerPrefs.SetInt(KeyPenas,    PenasOuro);
            PlayerPrefs.SetInt(KeyShields,  _shields);
            PlayerPrefs.Save();
        }

        void Load()
        {
            Micangas  = PlayerPrefs.GetInt(KeyMicangas, 0);
            PenasOuro = PlayerPrefs.GetInt(KeyPenas,    0);
            _shields  = PlayerPrefs.GetInt(KeyShields,  0);
        }
    }
}
