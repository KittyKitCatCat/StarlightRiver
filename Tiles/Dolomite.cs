﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;

namespace StarlightRiver.Tiles
{
    class Dolomite : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileStone[Type] = true;
            Main.tileBlockLight[Type] = true;
            drop = mod.ItemType("DolomiteItem");
            soundType = SoundID.Tink;
            dustType = DustID.Dirt;
            AddMapEntry(new Color(137, 91, 77));
        }
    }

    public class DolomiteHanging : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.AnchorAlternateTiles = new int[]
            {
                ModContent.TileType<Dolomite>()
            };
            TileObjectData.addTile(Type);
            soundType = SoundID.Tink;
            dustType = DustID.Dirt;
            AddMapEntry(new Color(137, 91, 77));
        }
    }
}
