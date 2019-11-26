using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Items.Standard;

namespace StarlightRiver.Items
{
	public sealed class GreenhouseGlassItem : StandardTileItem
	{
        public GreenhouseGlassItem() : base("Greenhouse Glass", "Speeds up the growth the plant below it\nNeeds a clear area above it", 16, 16, ModContent.TileType<Tiles.Herbology.GreenhouseGlass>(), rarity: ItemRarityID.Blue) { }
        
        
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(ItemID.Glass, 10);
            recipe.AddIngredient(ModContent.ItemType<Items.Crafting.AluminumBar>(), 1);

            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 10);

            recipe.AddRecipe();
        }
    }
}
