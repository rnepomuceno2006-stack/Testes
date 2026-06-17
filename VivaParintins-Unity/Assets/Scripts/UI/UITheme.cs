// Paleta oficial do Viva Parintins!!! — extraída do UI Spec
// Use UITheme.Cap ou UITheme.Gar para pegar as cores certas por time.

using UnityEngine;
using VivaParintins.Core;

namespace VivaParintins.UI
{
    public static class UITheme
    {
        // ── Caprichoso ────────────────────────────────────────────────
        public static readonly Color Cap900 = Hex("#0A1430");
        public static readonly Color Cap800 = Hex("#0C2A63");
        public static readonly Color Cap700 = Hex("#1B4FA8");
        public static readonly Color Cap500 = Hex("#2E6FD6");
        public static readonly Color Cap300 = Hex("#5AA8FF");
        public static readonly Color Cap100 = Hex("#CFE3FF");

        // ── Garantido ─────────────────────────────────────────────────
        public static readonly Color Gar900 = Hex("#1A0A14");
        public static readonly Color Gar800 = Hex("#5A0D14");
        public static readonly Color Gar700 = Hex("#8A1422");
        public static readonly Color Gar500 = Hex("#C0182A");
        public static readonly Color Gar300 = Hex("#FF6470");
        public static readonly Color Gar100 = Hex("#FFD0D6");

        // ── Ouro & neutros ────────────────────────────────────────────
        public static readonly Color GoldHi   = Hex("#FFF7DF");
        public static readonly Color GoldMid  = Hex("#F6D985");
        public static readonly Color GoldBase = Hex("#E3AB3E");
        public static readonly Color GoldDeep = Hex("#C98A26");
        public static readonly Color GoldText = Hex("#7A4D0E");

        public static readonly Color ParchmentHi   = Hex("#F3E2AD");
        public static readonly Color ParchmentLo   = Hex("#E2C879");
        public static readonly Color ParchmentText = Hex("#5A3A14");

        public static readonly Color InkPill = new Color(0.031f, 0.063f, 0.149f, 0.82f);
        public static readonly Color White   = Color.white;

        // ── Colecionáveis ─────────────────────────────────────────────
        public static readonly Color StarFill   = Hex("#FFCE2E");
        public static readonly Color StarBorder = Hex("#1769C9");
        public static readonly Color HeartFill  = Hex("#FF4D5E");
        public static readonly Color HeartBorder= Hex("#C81F35");

        // ── Helpers ───────────────────────────────────────────────────

        /// Retorna a cor "base" do time (500).
        public static Color TeamColor500(Team team) =>
            team == Team.Garantido ? Gar500 : Cap500;

        /// Retorna a cor "highlight" do time (300).
        public static Color TeamColor300(Team team) =>
            team == Team.Garantido ? Gar300 : Cap300;

        /// Retorna a cor de texto sobre fundo do time (100).
        public static Color TeamColor100(Team team) =>
            team == Team.Garantido ? Gar100 : Cap100;

        /// Retorna a cor de fundo escura do time (900).
        public static Color TeamColor900(Team team) =>
            team == Team.Garantido ? Gar900 : Cap900;

        public static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }
    }
}
