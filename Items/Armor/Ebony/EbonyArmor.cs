using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace StarlightRiver.Items.Armor.Ebony
{
    [AutoloadEquip(EquipType.Head)]
    public class EbonyHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebony Mask");
            Tooltip.SetDefault("3% increased minion damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000;
            item.rare = 2;
            item.defense = 1;
        }
        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.03f;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class EbonyChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebony Breastplate");
            Tooltip.SetDefault("3% increased minion damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 1;
            item.rare = 2;
            item.defense = 1;
        }
        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.03f;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<EbonyHead>() && legs.type == ModContent.ItemType<EbonyLegs>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "summon tagged enemies release bolts of dark energy at a random nearby enemy when taking summon damage for 20% of the hits damage";
            EbonyPlayer ebonyPlayer = player.GetModPlayer<EbonyPlayer>();
        }
    }
    public class EbonyPlayer : ModPlayer
    {
    }
    [AutoloadEquip(EquipType.Legs)]
    public class EbonyLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebony Footgear");
            Tooltip.SetDefault("3% minion damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 1;
            item.rare = 2;
            item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.03f;
        }
    }
}