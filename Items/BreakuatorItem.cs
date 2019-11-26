using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using StarlightRiver.Tiles;
using Terraria.ID;
using WebmilioCommons.Items;
using WebmilioCommons.Items.Standard;

namespace StarlightRiver.Items
{
    // TODO Remove UseItem
    public sealed class BreakuatorItem : StandardItem
    {
        public BreakuatorItem() : base("Breakuator", "Breaks tiles when powered", 16, 16, rarity: ItemRarityID.Blue)
        {
        }


        public override void SetDefaults()
        {
            base.SetDefaults();

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = 10;
            item.useTime = item.useAnimation;
        }


        public override bool UseItem(Player player)
        {
            if(Vector2.Distance(player.Center, Main.MouseWorld) <= 500)
                Breakuator.breakuator.Add(new Point16((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16));
            
            return true;
        }
    }
}
