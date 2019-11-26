using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Items.Standard;

namespace StarlightRiver.Items.Tiles.Stamina
{
    public sealed class StaminaGemItem : StandardTileItem
    {
        public StaminaGemItem() : base("Stamina Gem", "Use any ability on this to gain stamina!\n5 seconds cooldown", 16, 16, ModContent.TileType<global::StarlightRiver.Tiles.Interactive.StaminaGem>(), rarity: ItemRarityID.Yellow)
        {
        }
    }
}