// Coloque na Hierarquia: GameObject "ItemDatabase" (DontDestroyOnLoad).
// Carrega todas as RelicData de Resources/Relics/ automaticamente.
// Gerencia o inventário do jogador (quais relíquias foram compradas e seus níveis).

using System.Collections.Generic;
using UnityEngine;

namespace VivaParintins.Data
{
    public class ItemDatabase : MonoBehaviour
    {
        public static ItemDatabase Instance { get; private set; }

        // Todas as relíquias do jogo (carregadas de Resources)
        RelicData[] _allRelics;

        // Chaves PlayerPrefs: "relic_owned_<name>" e "relic_level_<name>"
        const string PrefixOwned = "relic_owned_";
        const string PrefixLevel = "relic_level_";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _allRelics = Resources.LoadAll<RelicData>("Relics");
        }

        // ── Consultas ────────────────────────────────────────────────────

        public RelicData[] AllRelics => _allRelics;

        public bool IsOwned(RelicData relic) =>
            PlayerPrefs.GetInt(PrefixOwned + relic.name, 0) == 1;

        public int GetLevel(RelicData relic) =>
            PlayerPrefs.GetInt(PrefixLevel + relic.name, 1);

        public IEnumerable<RelicData> OwnedRelics()
        {
            foreach (var r in _allRelics)
                if (IsOwned(r)) yield return r;
        }

        // ── Compra / Upgrade ──────────────────────────────────────────────

        // Retorna true se a compra foi bem-sucedida
        public bool Purchase(RelicData relic)
        {
            if (IsOwned(relic))
            {
                UI.ToastManager.Show($"{relic.relicName} já adquirida!");
                return false;
            }
            if (!GameEconomyManager.Instance.SpendPenas(relic.costPenas))
                return false;

            PlayerPrefs.SetInt(PrefixOwned + relic.name, 1);
            PlayerPrefs.SetInt(PrefixLevel + relic.name, 1);
            PlayerPrefs.Save();
            UI.ToastManager.Show($"✨ {relic.relicName} desbloqueada!");
            return true;
        }

        public bool Upgrade(RelicData relic)
        {
            if (!IsOwned(relic))
            {
                UI.ToastManager.Show("Compre a relíquia primeiro!");
                return false;
            }
            int level = GetLevel(relic);
            if (level >= relic.maxLevel)
            {
                UI.ToastManager.Show("Nível máximo atingido!");
                return false;
            }
            int cost = relic.upgradeCostBase * level;
            if (!GameEconomyManager.Instance.SpendPenas(cost))
                return false;

            PlayerPrefs.SetInt(PrefixLevel + relic.name, level + 1);
            PlayerPrefs.Save();
            UI.ToastManager.Show($"⬆️ {relic.relicName} → Nível {level + 1}!");
            return true;
        }

        // Valor efetivo do buff considerando o nível atual (escala linear)
        public float GetEffectiveBuffValue(RelicData relic)
        {
            int level = GetLevel(relic);
            return relic.buffValue * (1f + 0.3f * (level - 1));
        }
    }
}
