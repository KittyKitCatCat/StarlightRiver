﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Codex;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace StarlightRiver
{
    public static class Helper
    {
        /// <summary>
        /// Kills the NPC.
        /// </summary>
        /// <param name="npc"></param>

        public static Vector2 TileAdj { get  =>  Lighting.lightMode > 1 ? Vector2.Zero : Vector2.One * 12; }
        public static void Kill(this NPC npc)
        {
            bool modNPCDontDie = npc.modNPC != null && !npc.modNPC.CheckDead();
            if (modNPCDontDie)
                return;

            npc.life = 0;
            npc.checkDead();
            npc.HitEffect();
            npc.active = false;
        }

        public static void PlaceMultitile(Point16 position, int type, int style = 0)
        {
            TileObjectData data = TileObjectData.GetTileData(type, style); //magic numbers and uneccisary params begone!

            if (position.X + data.Width > Main.maxTilesX || position.X < 0) return; //make sure we dont spawn outside of the world!
            if (position.Y + data.Height > Main.maxTilesY || position.Y < 0) return;

            for (int x = 0; x < data.Width; x++) //generate each column
            {
                for (int y = 0; y < data.Height; y++) //generate each row
                {
                    Tile tile = Framing.GetTileSafely(position.X + x, position.Y + y); //get the targeted tile
                    tile.type = (ushort)type; //set the type of the tile to our multitile
                    tile.frameX = (short)(x * (data.CoordinateWidth + data.CoordinatePadding)); //set the X frame appropriately
                    tile.frameY = (short)(y * (data.CoordinateHeights[y] + data.CoordinatePadding)); //set the Y frame appropriately
                    tile.active(true); //activate the tile
                }
            }
        }
        public static bool CheckAirRectangle(Point16 position, Point16 size)
        {
            if (position.X + size.X > Main.maxTilesX || position.X < 0) return false; //make sure we dont check outside of the world!
            if (position.Y + size.Y > Main.maxTilesY || position.Y < 0) return false;

            for (int x = position.X; x < position.X + size.X; x++)
            {
                for (int y = position.Y; y < position.Y + size.Y; y++)
                {
                    if (Main.tile[x, y].active()) return false; //if any tiles there are active, return false!
                }
            }
            return true;
        }
        public static bool AirScanUp(Vector2 start, int MaxScan)
        {
            if (start.Y - MaxScan < 0) { return false; }

            bool clear = true;

            for (int k = 0; k <= MaxScan; k++)
            {
                if (Main.tile[(int)start.X, (int)start.Y - k].active()) { clear = false; }
            }
            return clear;
        }
        public static void UnlockEntry<type>(Player player)
        {
            player.GetModPlayer<CodexHandler>().Entries.FirstOrDefault(entry => entry is type).Locked = false;
            GUI.Codex.NewEntry = true;
        }
        public static void SpawnGem(int ID, Vector2 position)
        {
            int item = Item.NewItem(position, ModContent.ItemType<Items.StarlightGem>());
            (Main.item[item].modItem as Items.StarlightGem).gemID = ID;
        }
        public static void DrawSymbol(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            Texture2D tex = ModContent.GetTexture("StarlightRiver/Symbol");
            float scale = 0.9f + (float)Math.Sin(LegendWorld.rottime) * 0.1f;
            spriteBatch.Draw(tex, position, tex.Frame(), color * 0.8f * scale, 0, tex.Size() * 0.5f, scale * 0.8f, 0, 0);

            Texture2D tex2 = ModContent.GetTexture("StarlightRiver/Tiles/Interactive/WispSwitchGlow2");
            float fade = LegendWorld.rottime / 6.28f;
            spriteBatch.Draw(tex2, position, tex2.Frame(), color * (1 - fade), 0, tex2.Size() / 2f, fade * 0.6f, 0, 0);
        }
        public static bool CheckCircularCollision(Vector2 center, int radius, Rectangle hitbox)
        {
            if (Vector2.Distance(center, hitbox.TopLeft()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.TopRight()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.BottomLeft()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.BottomRight()) <= radius) return true;
            return false;
        }
        public static string TicksToTime(int ticks)
        {
            int sec = ticks / 60;
            return (sec / 60) + ":" + (sec % 60 < 10 ? "0" + sec % 60 : "" + sec % 60);
        }
        public static void DrawElectricity(Vector2 point1, Vector2 point2, int dusttype, float scale = 1)
        {
            int nodeCount = (int)Vector2.Distance(point1, point2) / 30;
            Vector2[] nodes = new Vector2[nodeCount + 1];

            nodes[nodeCount] = point2; //adds the end as the last point

            for (int k = 1; k < nodes.Count(); k++)
            {
                //Sets all intermediate nodes to their appropriate randomized dot product positions
                nodes[k] = Vector2.Lerp(point1, point2, k / (float)nodeCount) + (k == nodes.Count() - 1 ? Vector2.Zero : Vector2.Normalize(point1 - point2).RotatedBy(1.58f) * Main.rand.NextFloat(-18, 18));

                //Spawns the dust between each node
                Vector2 prevPos = k == 1 ? point1 : nodes[k - 1];
                for (float i = 0; i < 1; i += 0.05f)
                {
                    Dust.NewDustPerfect(Vector2.Lerp(prevPos, nodes[k], i), dusttype, Vector2.Zero, 0, default, scale);
                }
            }
        }

        private static int tiltTime;
        private static float tiltMax;
        public static void DoTilt(float intensity) { tiltMax = intensity; tiltTime = 0; }
        public static void UpdateTilt()
        {
            if (Math.Abs(tiltMax) > 0)
            {
                tiltTime++;
                if (tiltTime >= 1 && tiltTime < 40)
                {
                    float tilt = tiltMax - tiltTime * tiltMax / 40f;
                    StarlightRiver.Rotation = tilt * (float)Math.Sin(Math.Pow(tiltTime / 40f * 6.28f, 0.9f));
                }

                if (tiltTime >= 40) { StarlightRiver.Rotation = 0; tiltMax = 0; }
            }
        }
        public static bool HasEquipped(Player player, int ItemID)
        {
            for (int k = 3; k < 7 + player.extraAccessorySlots; k++) if (player.armor[k].type == ItemID) return true;
            return false;
        }
    }
}
