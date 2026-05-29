// Coloque na Hierarquia: Canvas "ShopPanel" (ativado/desativado via botão MENU).
// Popula automaticamente todos os RelicData de Resources/Relics/ no grid.
// Para cada botão de compra/upgrade gerado, conecte OnBuyClicked() / OnUpgradeClicked().

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivaParintins.Data;

namespace VivaParintins.UI
{
    public class ShopUI : MonoBehaviour
    {
        [Header("HUD de moedas (topo da tela)")]
        public TextMeshProUGUI miçangasDisplay;
        public TextMeshProUGUI penasDisplay;

        [Header("Grid de relíquias")]
        public Transform      relicGrid;       // Content do ScrollView
        public GameObject     relicCardPrefab; // Prefab com: Icon, Name, Owner, Buff, Cost, BuyBtn, UpgradeBtn

        [Header("Painel de detalhe")]
        public GameObject         detailPanel;
        public Image              detailIcon;
        public TextMeshProUGUI    detailName;
        public TextMeshProUGUI    detailOwner;
        public TextMeshProUGUI    detailBuff;
        public TextMeshProUGUI    detailLevel;
        public TextMeshProUGUI    detailCost;
        public Button             detailBuyBtn;
        public Button             detailUpgradeBtn;
        public Button             detailEquipBtn;

        RelicData _selected;

        void OnEnable()
        {
            RefreshCurrencyDisplay();
            PopulateGrid();
        }

        // ── HUD ─────────────────────────────────────────────────────────
        void RefreshCurrencyDisplay()
        {
            miçangasDisplay.text = $"🍄 {GameEconomyManager.Instance.Micangas}";
            penasDisplay.text    = $"🪶 {GameEconomyManager.Instance.PenasOuro}";
        }

        // ── Grid ─────────────────────────────────────────────────────────
        void PopulateGrid()
        {
            foreach (Transform c in relicGrid) Destroy(c.gameObject);

            foreach (var relic in ItemDatabase.Instance.AllRelics)
            {
                var card = Instantiate(relicCardPrefab, relicGrid);
                SetupCard(card, relic);
            }
        }

        void SetupCard(GameObject card, RelicData relic)
        {
            bool owned  = ItemDatabase.Instance.IsOwned(relic);
            int  level  = ItemDatabase.Instance.GetLevel(relic);
            bool maxed  = level >= relic.maxLevel;

            // Ícone
            var icon = card.transform.Find("Icon")?.GetComponent<Image>();
            if (icon && relic.icon) icon.sprite = relic.icon;

            // Textos
            SetText(card, "Name",  relic.relicName);
            SetText(card, "Owner", relic.ownerCharacter);
            SetText(card, "Buff",  BuffDescription(relic));
            SetText(card, "Level", owned ? $"Nível {level}/{relic.maxLevel}" : "Bloqueado");

            // Botão Comprar (visível se não possui)
            var buyBtn = card.transform.Find("BuyButton")?.GetComponent<Button>();
            if (buyBtn)
            {
                buyBtn.gameObject.SetActive(!owned);
                SetText(buyBtn.gameObject, "Cost", $"🪶 {relic.costPenas}");
                buyBtn.onClick.RemoveAllListeners();
                buyBtn.onClick.AddListener(() => OnBuyClicked(relic));
            }

            // Botão Upgrade (visível se possui e não está no max)
            var upBtn = card.transform.Find("UpgradeButton")?.GetComponent<Button>();
            if (upBtn)
            {
                upBtn.gameObject.SetActive(owned && !maxed);
                int upgCost = relic.upgradeCostBase * level;
                SetText(upBtn.gameObject, "Cost", $"🪶 {upgCost}");
                upBtn.onClick.RemoveAllListeners();
                upBtn.onClick.AddListener(() => OnUpgradeClicked(relic));
            }

            // Toque no card → mostra detalhe
            var btn = card.GetComponent<Button>() ?? card.AddComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ShowDetail(relic));
        }

        // ── Detalhe ──────────────────────────────────────────────────────
        void ShowDetail(RelicData relic)
        {
            _selected = relic;
            bool owned = ItemDatabase.Instance.IsOwned(relic);
            int  level = ItemDatabase.Instance.GetLevel(relic);
            bool maxed = level >= relic.maxLevel;
            float eff  = ItemDatabase.Instance.GetEffectiveBuffValue(relic);

            detailPanel.SetActive(true);
            if (relic.icon) detailIcon.sprite = relic.icon;
            detailName.text  = relic.relicName;
            detailOwner.text = $"{relic.ownerCharacter} ({(relic.faction == Team.Garantido ? "Garantido ❤️" : "Caprichoso ⭐")})";
            detailBuff.text  = $"{BuffDescription(relic)} — Valor efetivo: {eff:F1}";
            detailLevel.text = owned ? $"Nível {level} / {relic.maxLevel}" : "Não adquirida";
            detailCost.text  = owned ? $"Upgrade: 🪶 {relic.upgradeCostBase * level}" : $"Comprar: 🪶 {relic.costPenas}";

            detailBuyBtn.gameObject.SetActive(!owned);
            detailUpgradeBtn.gameObject.SetActive(owned && !maxed);
            detailEquipBtn.gameObject.SetActive(owned);

            detailBuyBtn.onClick.RemoveAllListeners();
            detailBuyBtn.onClick.AddListener(() => OnBuyClicked(_selected));

            detailUpgradeBtn.onClick.RemoveAllListeners();
            detailUpgradeBtn.onClick.AddListener(() => OnUpgradeClicked(_selected));

            detailEquipBtn.onClick.RemoveAllListeners();
            detailEquipBtn.onClick.AddListener(() => OnEquipClicked(_selected));
        }

        // ── Ações de botão ────────────────────────────────────────────────
        // Conecte o botão de compra de Penas de Ouro a BuyPenasOuro()
        public void BuyPenasOuro(int amount)
        {
            GameEconomyManager.Instance.BuyPremiumItem($"penas_{amount}");
            RefreshCurrencyDisplay();
        }

        void OnBuyClicked(RelicData relic)
        {
            if (ItemDatabase.Instance.Purchase(relic))
            {
                RefreshCurrencyDisplay();
                PopulateGrid();
                if (_selected == relic) ShowDetail(relic);
            }
        }

        void OnUpgradeClicked(RelicData relic)
        {
            if (ItemDatabase.Instance.Upgrade(relic))
            {
                RefreshCurrencyDisplay();
                PopulateGrid();
                if (_selected == relic) ShowDetail(relic);
            }
        }

        void OnEquipClicked(RelicData relic)
        {
            Core.GameManager.Instance.equippedRelic = relic;
            ToastManager.Show($"✅ {relic.relicName} equipada!");
            // Atualiza o BuffSystem se o jogador estiver na cena de jogo
            var buff = FindObjectOfType<Player.BuffSystem>();
            buff?.ApplyEquippedRelic();
        }

        // ── Helpers ──────────────────────────────────────────────────────
        string BuffDescription(RelicData r) => r.buffType switch
        {
            RelicBuff.Magnet          => "🎤 Ímã de Miçangas",
            RelicBuff.Shield          => "🛡️ Absorve colisão",
            RelicBuff.Dash            => "🎺 Dash invencível",
            RelicBuff.ScoreMultiplier => "⭐ Multiplicador de Score",
            RelicBuff.Glide           => "☂️ Pulo duplo / Planar",
            RelicBuff.ArrowShot       => "🏹 Flecha destroi obstáculos",
            _                         => ""
        };

        void SetText(GameObject go, string childName, string value)
        {
            var t = go.transform.Find(childName)?.GetComponent<TextMeshProUGUI>();
            if (t) t.text = value;
        }
    }
}
