using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using StarlightRiver.Codex;
using StarlightRiver.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace StarlightRiver
{
    public static class Helper
    {
        public static void MakeGlow(this Projectile projectile, Vector2 position, Texture2D texture, Color color, float time, float size, bool additive)
        {
            new GlowProjectile(position, time, size, color, texture, additive);
        }
        public static void DrawTrail(this Projectile projectile, SpriteBatch spriteBatch, Texture2D texture, Color color, bool additive, float scaleMultiplier)
        {
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                float progress = (float)(projectile.oldPos.Length - k) / projectile.oldPos.Length;
                float scale = projectile.scale * progress * scaleMultiplier;
                Color newCoolor = color * (progress);
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + projectile.Size / 2 + new Vector2(0f, projectile.gfxOffY);
                if (additive)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(default, BlendState.Additive);
                }
                spriteBatch.Draw(texture, drawPos, null, newCoolor, projectile.oldRot[k], texture.Size() / 2, scale, SpriteEffects.None, 0f);
                if (additive)
                {
                    spriteBatch.End();
                    spriteBatch.Begin();
                }
            }
        }
        /// <summary>
        /// Kills the NPC.
        /// </summary>
        /// <param name="npc"></param>
        public static Vector2 TileAdj { get => Lighting.lightMode > 1 ? Vector2.Zero : Vector2.One * 12; }
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
        /// <summary>
        /// Checks if a target is valid
        /// </summary>
        /// <param name="npc"></param>
        public static bool IsTargetValid(this NPC npc) => npc.active && !npc.friendly && !npc.dontTakeDamage && !npc.immortal; //mostly used for summoner like stuff
        
        public static void PlaceMultitile(Point16 position, int type, int style = 0)
        {
            TileObjectData data = TileObjectData.GetTileData(type, style); //magic numbers and uneccisary params begone!

            if (position.X + data.Width > Main.maxTilesX || position.X < 0) return; //make sure we dont spawn outside of the world!
            if (position.Y + data.Height > Main.maxTilesY || position.Y < 0) return;

            int xVariants = 0;
            int yVariants = 0;
            if (data.StyleHorizontal) xVariants = Main.rand.Next(data.RandomStyleRange);
            else yVariants = Main.rand.Next(data.RandomStyleRange);

            for (int x = 0; x < data.Width; x++) //generate each column
            {
                for (int y = 0; y < data.Height; y++) //generate each row
                {
                    Tile tile = Framing.GetTileSafely(position.X + x, position.Y + y); //get the targeted tile
                    tile.type = (ushort)type; //set the type of the tile to our multitile

                    tile.frameX = (short)((x + data.Width * xVariants) * (data.CoordinateWidth + data.CoordinatePadding)); //set the X frame appropriately
                    tile.frameY = (short)((y + data.Height * yVariants) * (data.CoordinateHeights[y] + data.CoordinatePadding)); //set the Y frame appropriately
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

            for (int k = 1; k <= MaxScan; k++)
            {
                if (Main.tile[(int)start.X, (int)start.Y - k].active()) { clear = false; }
            }
            return clear;
        }
        public static void UnlockEntry<type>(Player player)
        {
            CodexHandler mp = player.GetModPlayer<CodexHandler>();
            CodexEntry entry = mp.Entries.FirstOrDefault(n => n is type);

            if (entry.RequiresUpgradedBook && mp.CodexState != 2) return; //dont give the player void entries if they dont have the void book
            entry.Locked = false;
            entry.New = true;
            if(mp.CodexState != 0) StarlightRiver.Instance.codexpopup.TripEntry(entry.Title);
            Main.PlaySound(SoundID.Item30);
        }
        public static void SpawnGem(int ID, Vector2 position)
        {
            int item = Item.NewItem(position, ModContent.ItemType<Items.StarlightGem>());
            (Main.item[item].modItem as Items.StarlightGem).gemID = ID;
        }
        public static void DrawSymbol(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            Texture2D tex = ModContent.GetTexture("StarlightRiver/Symbol");
            spriteBatch.Draw(tex, position, tex.Frame(), color * 0.8f, 0, tex.Size() / 2, 1, 0, 0);

            Texture2D tex2 = ModContent.GetTexture("StarlightRiver/Tiles/Interactive/WispSwitchGlow2");

            float fade = LegendWorld.rottime / 6.28f;
            spriteBatch.Draw(tex2, position, tex2.Frame(), color * (1 - fade), 0, tex2.Size() / 2f, fade * 1.1f, 0, 0);
        }
        public static bool CheckCircularCollision(Vector2 center, int radius, Rectangle hitbox)
        {
            if (Vector2.Distance(center, hitbox.TopLeft()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.TopRight()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.BottomLeft()) <= radius) return true;
            if (Vector2.Distance(center, hitbox.BottomRight()) <= radius) return true;
            return false;
        }
        public static bool CheckConicalCollision(Vector2 center, int radius, float angle, float width, Rectangle hitbox)
        {
            if (CheckPoint(center, radius, hitbox.TopLeft(), angle, width)) return true;
            if (CheckPoint(center, radius, hitbox.TopRight(), angle, width)) return true;
            if (CheckPoint(center, radius, hitbox.BottomLeft(), angle, width)) return true;
            if (CheckPoint(center, radius, hitbox.BottomRight(), angle, width)) return true;
            return false;
        }
        private static bool CheckPoint(Vector2 center, int radius, Vector2 check, float angle, float width)
        {
            float thisAngle = (center - check).ToRotation() % 6.28f;
            return Vector2.Distance(center, check) <= radius && thisAngle > angle - width && thisAngle < angle + width;
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
        public static void NpcVertical(NPC npc, bool jump, int slot = 1, int jumpheight = 2) //idea: could be seperated farther
        {
            npc.ai[slot] = 0;//reset jump counter
            for (int y = 0; y < jumpheight; y++)//idea: this should have diminishing results for output jump height
            {
                Tile tileType = Framing.GetTileSafely((int)(npc.position.X / 16) + (npc.direction * 2) + 1, (int)((npc.position.Y + npc.height + 8) / 16) - y - 1);
                if ((Main.tileSolid[tileType.type] || Main.tileSolidTop[tileType.type]) && tileType.active()) //how tall the wall is
                {
                    npc.ai[slot] = (y + 1);
                }
                if (y >= npc.ai[slot] + (npc.height / 16) || (!jump && y >= 2)) //stops counting if there is room for the npc to walk under //((int)((npc.position.Y - target.position.Y) / 16) + 1)
                {
                    if (npc.HasValidTarget && jump)
                    {
                        Player target = Main.player[npc.target];
                        if (npc.ai[slot] >= ((int)((npc.position.Y - target.position.Y) / 16) + 1) - ((int)(npc.height / 16) - 1))
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (npc.ai[slot] > 0)//jump and step up
            {
                Tile tileType = Framing.GetTileSafely((int)(npc.position.X / 16) + (npc.direction * 2) + 1, (int)((npc.position.Y + npc.height + 8) / 16) - 1);
                if (npc.ai[slot] == 1 && npc.collideX)
                {
                    if (tileType.halfBrick() || (Main.tileSolid[tileType.type] && (npc.position.Y % 16 + 8) == 0))
                    {
                        npc.position.Y -= 8;//note: these just zip the npc up the block and it looks bad, need to figure out how vanilla glides them up
                        npc.velocity.X = npc.oldVelocity.X;
                    }
                    else if (Main.tileSolid[tileType.type])
                    {
                        npc.position.Y -= 16;
                        npc.velocity.X = npc.oldVelocity.X;
                    }
                }
                else if (npc.ai[slot] == 2 && (npc.position.Y % 16) == 0 && Framing.GetTileSafely((int)(npc.position.X / 16) + (npc.direction * 2) + 1, (int)((npc.position.Y + npc.height) / 16) - 1).halfBrick())
                {//note: I dislike this extra check, but couldn't find a way to avoid it
                    if (npc.collideX)
                    {
                        npc.position.Y -= 16;
                        npc.velocity.X = npc.oldVelocity.X;
                    }
                }
                else if (npc.ai[slot] > 1 && jump == true)
                {

                    npc.velocity.Y = -(3 + npc.ai[slot]);
                    if (!npc.HasValidTarget && npc.velocity.X == 0)
                    {
                        npc.ai[3]++;
                    }
                }
            }
        }       
        public static bool ScanForTypeDown(int startX, int startY, int type, int maxDown = 50)
        {
            for(int k = 0; k >= 0; k++)
            {
                if (Main.tile[startX, startY + k].type == type) return true;
                if (k > maxDown || startY + k >= Main.maxTilesY) break;
            }
            return false;
        }
        public static int SamplePerlin2D(int x, int y, int min, int max)
        {
            Texture2D perlin = TextureManager.Load("Images/Misc/Perlin");

            Color[] rawData = new Color[perlin.Width]; //array of colors
            Rectangle row = new Rectangle(0, y, perlin.Width, 1); //one row of the image
            perlin.GetData<Color>(0, row, rawData, 0, perlin.Width); //put the color data from the image into the array
            return (int)(min + rawData[x % 512].R / 255f * max);
        }
        public static float CompareAngle(float baseAngle, float targetAngle)
        {
            return (baseAngle - targetAngle + (float)Math.PI * 3) % MathHelper.TwoPi - (float)Math.PI;
        }
        public static string WrapString(string input, int length, DynamicSpriteFont font, float scale)
        {
            string output = "";
            string[] words = input.Split();

            string line = "";
            foreach (string str in words)
            {
                if(font.MeasureString(line).X * scale < length)
                {
                    output += (" " + str);
                    line += (" " + str);
                }
                else
                {
                    output += ("\n" + str);
                    line = (str);
                }
            }
            return output;
        }
        public static List<T> RandomizeList<T> (List<T> input)
        {
            int n = input.Count();
            while (n > 1)
            {
                n--;
                int k = Main.rand.Next(n + 1);
                T value = input[k];
                input[k] = input[n];
                input[n] = value;
            }
            return input;
        }
    }
}
