using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarlightRiver.Players;
using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Items.CursedAccessories
{
    class AnthemDagger : CursedAccessory
    {
        public AnthemDagger() : base(ModContent.GetTexture("StarlightRiver/Items/CursedAccessories/AnthemDaggerGlow"))
        {

        }
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Consume mana to absorb damage\n90% Reduced defense");
            DisplayName.SetDefault("Anthem Dagger");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.accessory = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.statDefense /= 10;
            player.manaFlower = false;
            player.GetModPlayer<StarlightPlayer>().AnthemDagger = true;
        }
    }
}
