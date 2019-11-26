using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.Items
{
    public class QuickMaterial : ModItem
    {
        string Matname;
        string Mattooltip;
        int Maxstack;
        int Value;
        int Rare;

        public QuickMaterial(string name, string tooltip, int maxstack, int value, int rare)
        {
            Matname = name;
            Mattooltip = tooltip;
            Maxstack = maxstack;
            Value = value;
            Rare = rare;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(Matname);
            Tooltip.SetDefault(Mattooltip); 
        }
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = Maxstack;
            item.value = Value;
            item.rare = Rare;
        }
    }
}
