// Coloque no mesmo GameObject do PlayerMobileController.
// Lê a RelicData equipada no GameManager e aplica o efeito no torcedor.
// Para equipar uma relíquia: GameManager.Instance.EquipRelic(relicData)

using System.Collections;
using UnityEngine;
using VivaParintins.Data;

namespace VivaParintins.Player
{
    [RequireComponent(typeof(PlayerMobileController))]
    public class BuffSystem : MonoBehaviour
    {
        [Header("VFX de buff (arraste os prefabs 2D)")]
        public GameObject shieldVFX;       // partícula de escudo ao redor do torcedor
        public GameObject magnetVFX;       // aro magnético animado
        public GameObject arrowVFX;        // flecha disparada para frente
        public GameObject glideVFX;        // sombrinha estrelada
        public GameObject dashVFX;         // rastro de invencibilidade

        [Header("Arrow")]
        public float arrowCooldown = 2f;
        public float arrowRange    = 15f;

        // ── Estado ───────────────────────────────────────────────────────
        PlayerMobileController _player;
        RelicData _equipped;

        bool _shieldActive;
        bool _magnetActive;
        bool _dashActive;
        bool _glideActive;
        float _scoreMultiplier = 1f;

        float _arrowTimer;

        // ── Acessores para PhaseRunner ────────────────────────────────────
        public float ScoreMultiplier => _scoreMultiplier;
        public bool  ShieldActive    => _shieldActive;

        void Awake() => _player = GetComponent<PlayerMobileController>();

        void Start() => ApplyEquippedRelic();

        // ── Aplicar relíquia equipada ─────────────────────────────────────
        public void ApplyEquippedRelic()
        {
            _equipped = Core.GameManager.Instance?.equippedRelic;
            ResetBuffs();
            if (_equipped == null) return;

            float value = Data.ItemDatabase.Instance?.GetEffectiveBuffValue(_equipped) ?? _equipped.buffValue;

            switch (_equipped.buffType)
            {
                case RelicBuff.Shield:
                    ActivateShield((int)value);
                    break;
                case RelicBuff.Magnet:
                    ActivateMagnet(value);
                    break;
                case RelicBuff.Dash:
                    // Berrante: dash disponível a cada 15s
                    StartCoroutine(DashCooldownLoop(value));
                    break;
                case RelicBuff.ScoreMultiplier:
                    _scoreMultiplier = value;
                    break;
                case RelicBuff.Glide:
                    _glideActive = true;
                    _player.enableDoubleJump = true;
                    glideVFX?.SetActive(true);
                    break;
                case RelicBuff.ArrowShot:
                    // Botão de flecha aparece na UI — chame FireArrow() via botão
                    break;
            }
        }

        void ResetBuffs()
        {
            _shieldActive    = false;
            _magnetActive    = false;
            _dashActive      = false;
            _glideActive     = false;
            _scoreMultiplier = 1f;
            _player.enableDoubleJump = false;

            shieldVFX?.SetActive(false);
            magnetVFX?.SetActive(false);
            glideVFX?.SetActive(false);
        }

        // ── Maracá Místico — Escudo ───────────────────────────────────────
        void ActivateShield(int charges)
        {
            _shieldActive = true;
            GameEconomyManager.Instance?.AddShield(charges);
            shieldVFX?.SetActive(true);
            UI.ToastManager.Show("🛡️ Maracá Místico ativo!");
        }

        // O PlayerMobileController chama este método antes de TakeDamage
        public bool TryAbsorbHit()
        {
            if (!_shieldActive) return false;
            _shieldActive = false;
            shieldVFX?.SetActive(false);
            StartCoroutine(ShieldBreakFeedback());
            return true;
        }

        IEnumerator ShieldBreakFeedback()
        {
            // Pisca o VFX 3x ao quebrar
            for (int i = 0; i < 3; i++)
            {
                shieldVFX?.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                shieldVFX?.SetActive(false);
                yield return new WaitForSeconds(0.1f);
            }
        }

        // ── Microfone de Ouro — Ímã ──────────────────────────────────────
        void ActivateMagnet(float radius)
        {
            _magnetActive = true;
            magnetVFX?.SetActive(true);
            StartCoroutine(MagnetLoop(radius));
            UI.ToastManager.Show("🎤 Microfone de Ouro: ímã ativo!");
        }

        IEnumerator MagnetLoop(float radius)
        {
            while (_magnetActive)
            {
                PullCoins(radius);
                yield return null;
            }
        }

        void PullCoins(float radius)
        {
            var hits = Physics.OverlapSphere(transform.position, radius);
            foreach (var h in hits)
            {
                if (!h.CompareTag("Coin")) continue;
                h.transform.position = Vector3.MoveTowards(
                    h.transform.position, transform.position, 25f * Time.deltaTime);
            }
        }

        // ── Berrante Poderoso — Dash invencível ─────────────────────────
        IEnumerator DashCooldownLoop(float duration)
        {
            while (true)
            {
                yield return new WaitForSeconds(15f);
                StartCoroutine(InvincibleDash(duration));
            }
        }

        IEnumerator InvincibleDash(float duration)
        {
            _dashActive = true;
            _player.ActivateDash();
            dashVFX?.SetActive(true);
            UI.ToastManager.Show("🎺 Berrante Poderoso! INVENCÍVEL!");

            // Durante o dash, anula colisões com obstáculos
            Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacle"), true);
            yield return new WaitForSeconds(duration);
            Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Obstacle"), false);

            _dashActive = false;
            dashVFX?.SetActive(false);
        }

        // ── Arco e Flecha — disparar para frente ─────────────────────────
        // Conecte este método ao botão de flecha na UI
        public void FireArrow()
        {
            if (_equipped?.buffType != RelicBuff.ArrowShot) return;
            if (_arrowTimer > 0)
            {
                UI.ToastManager.Show($"Flecha em {_arrowTimer:F0}s...");
                return;
            }
            _arrowTimer = arrowCooldown;
            StartCoroutine(ArrowRoutine());
        }

        IEnumerator ArrowRoutine()
        {
            // Detecta obstáculos na direção Z+ (frente do runner)
            var hits = Physics.RaycastAll(transform.position, Vector3.forward, arrowRange);
            foreach (var h in hits)
            {
                if (h.collider.CompareTag("Obstacle"))
                {
                    var vfx = Instantiate(arrowVFX, transform.position, Quaternion.identity);
                    Destroy(vfx, 1f);
                    Destroy(h.collider.gameObject, 0.3f);
                }
            }
            yield return new WaitForSeconds(arrowCooldown);
            _arrowTimer = 0;
        }

        void Update()
        {
            if (_arrowTimer > 0) _arrowTimer -= Time.deltaTime;
        }
    }
}
