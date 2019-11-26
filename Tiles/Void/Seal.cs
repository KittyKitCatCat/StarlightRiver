using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Abilities;
using System;
using System.Linq;
using StarlightRiver.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace StarlightRiver.Tiles.Void
{
    class Seal : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.Width = 22;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(0, 0);

            TileObjectData.addTile(Type);

            drop = mod.ItemType("Bounce");
            AddMapEntry(new Color(50, 50, 50));
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i,j].frameX == 0 && Main.tile[i, j].frameY == 0)
            {
                foreach (Player player in Main.player)
                {
                    if (Collision.CheckAABBvAABBCollision(player.position, player.Size, new Vector2(i * 16, j * 16 - 45), new Vector2(368, 64)) && 
                        player.GetModPlayer<AbilityHandler>().smash.Active && !LegendWorld.SealOpen)
                    {
                        LegendWorld.SealOpen = true;
                        player.GetModPlayer<AbilityHandler>().smash.OnExit();
                        player.GetModPlayer<AbilityHandler>().smash.Active = false;
                        player.GetModPlayer<StarlightPlayer>().Shake = 80;

                        Main.PlaySound(SoundID.NPCDeath59);
                        Main.PlaySound(SoundID.Item123);

                        for(float k = 0; k <= 3.14f; k += 0.02f)
                        {
                            Dust.NewDustPerfect(new Vector2(i + 11, j) * 16, DustID.Stone, new Vector2(-1, 0).RotatedBy(k) * Main.rand.Next(0, 150) * 0.1f, 0, default, 2.5f);
                            Dust.NewDustPerfect(new Vector2(i + 11, j) * 16, DustID.WitherLightning, new Vector2(-1, 0).RotatedBy(k) * Main.rand.Next(0, 150) * 0.2f, 0, Color.Blue, 2.5f);
                        }

                        player.velocity.Y = -20;
                    }
                }
                if (LegendWorld.SealOpen)
                {
                    Main.tileSolid[Type] = false;
                }
                else
                {
                    Main.tileSolid[Type] = true;
                }
            }
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
        {
            if (Main.tile[i, j].frameX == 0 && Main.tile[i, j].frameY == 0)
            {
                Vector2 Seal = new Vector2((i + 12) * 16, (j + 12) * 16);
                if (!LegendWorld.SealOpen)
                {
                    spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/Tiles/SealClosed"), Seal - Main.screenPosition, Color.White);
                }
                else
                {
                    spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/Tiles/SealOpen"), Seal - Main.screenPosition, Color.White);
                }
            }
        }
    }
}
