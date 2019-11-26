using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace StarlightRiver.Tiles.StarJuice
{
    sealed class Tank : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;

            
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TankEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
            dustType = DustID.Stone;
            disableSmartCursor = true;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Astral Altar");
            AddMapEntry(new Color(163, 163, 163), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<Items.Crafting.OvenItem>());
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            int left = i - (tile.frameX / 18);
            int top = j - (tile.frameY / 18);
            int index = ModContent.GetInstance<TankEntity>().Find(left, top);

            if (index == -1)
            {
                return true;
            }
            TankEntity altarentity = (TankEntity)TileEntity.ByID[index];

            if (Main.tile[i,j].frameX == 2 && Main.tile[i,j].frameY == 1)
            {
                Vector2 pos = (new Vector2(i, j) * 16) - Main.screenPosition;
                spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/Tiles/StarJuice/OrbIn"), new Rectangle((int)pos.X, (int)pos.Y, 32, (int)(altarentity.charge / 5000f * 32)), new Rectangle(0, 0, 32, 32), Color.White, 0, Vector2.Zero, 0, 0);
                spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/Tiles/StarJuice/OrbOut"), pos, Color.White);
            }
            return true;
        }

    }

    sealed class TankEntity : ModTileEntity
    {
        public int charge = 0;

        public override bool ValidTile(int i, int j)
        {
            return Main.tile[i, j].type == ModContent.TileType<Tank>() && Main.tile[i, j].active() && Main.tile[i, j].frameX == 0 && Main.tile[i, j].frameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(87, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override void Update()
        {
            Main.NewText(Main.time + ": " + charge + "/5000");
            if(Main.time >= 20000 /*replace with whatever si nighttime*/ && charge < 5000)
            {
                charge++;
            }
        }
    }
}
