// Coloque em cada GameObject de item no cenário junto com um Collider (Is Trigger = true).
// Tag do objeto: "Item"

using UnityEngine;

namespace VivaParintins.Player
{
    public enum ItemType { Guarana, ShieldPalha, ImaGalera, HealTacaca }

    public class ItemPickup : MonoBehaviour
    {
        public ItemType itemType;

        public void Apply(PlayerMobileController player)
        {
            switch (itemType)
            {
                case ItemType.Guarana:
                    player.ActivateDash();
                    break;
                case ItemType.ShieldPalha:
                    GameEconomyManager.Instance.AddShield();
                    break;
                case ItemType.ImaGalera:
                    GameEconomyManager.Instance.ActivateMagnet();
                    break;
                case ItemType.HealTacaca:
                    // reservado para sistema de vidas futuro
                    break;
            }
        }
    }
}
