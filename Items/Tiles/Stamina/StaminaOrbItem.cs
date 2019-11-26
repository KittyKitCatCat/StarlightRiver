using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Items.Standard;

namespace StarlightRiver.Items.Tiles.Stamina
{
    public sealed class StaminaOrbItem : StandardTileItem
    {
        public StaminaOrbItem() : base("Stamina Orb", "Pass through this to gain stamina!\n5 second cooldown", 16, 96, ModContent.TileType<global::StarlightRiver.Tiles.Interactive.StaminaOrb>(), ItemRarityID.Yellow)
        {
        }


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 6));
        }
    }
}