﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using StarlightRiver.Abilities;
using StarlightRiver.Keys;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace StarlightRiver.Tiles.Overgrow
{
    class OvergrowLock : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("");//Map name
            AddMapEntry(new Color(200, 200, 200), name);
            dustType = ModContent.DustType<Dusts.Gold2>();
            disableSmartCursor = true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {

        }

        public override bool NewRightClick(int i, int j)
        {
            if (Key.Use<OvergrowKey>())
            {
                CombatText.NewText(new Rectangle(i * 16, j * 16, 0, 0), Color.White, "Unlocked with Overgrowth Key!");
                WorldGen.KillTile(i, j);
                return true;
            }
            else
            {
                CombatText.NewText(new Rectangle(i * 16, j * 16, 0, 0), Color.Red, "Need: Overgrowth Key");
                return false;
            }
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            player.showItemIcon = true;
            player.showItemIcon2 = -1;
            player.showItemIconText = "Need: Overgrowth Key";

        }
    }
}
