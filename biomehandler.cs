﻿using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System;
using StarlightRiver.GUI;
using StarlightRiver.Abilities;
using StarlightRiver.Codex.Entries;
using StarlightRiver.Codex;

namespace StarlightRiver
{
    public class BiomeHandler : ModPlayer
    {
        public bool ZoneGlass = false;
        public bool ZoneVoidPre = false;
        public bool ZoneJungleCorrupt = false;
        public bool ZoneJungleBloody = false;
        public bool ZoneJungleHoly = false;
        public bool ZoneOvergrow = false;
        public override void UpdateBiomes()
        {
            ZoneGlass = LegendWorld.vitricBiome.Contains((player.Center / 16).ToPoint());
            ZoneVoidPre = (LegendWorld.voidTiles > 50);
            ZoneJungleCorrupt = (LegendWorld.evilJungleTiles > 50);
            ZoneJungleBloody = (LegendWorld.bloodJungleTiles > 50);
            ZoneJungleHoly = (LegendWorld.holyJungleTiles > 50);
            ZoneOvergrow = Main.tile[(int)(player.Center.X / 16), (int)(player.Center.Y / 16)].wall == ModContent.WallType<Tiles.Overgrow.WallOvergrowGrass>() ||
                Main.tile[(int)(player.Center.X / 16), (int)(player.Center.Y / 16)].wall == ModContent.WallType<Tiles.Overgrow.WallOvergrowBrick>() ||
                Main.tile[(int)(player.Center.X / 16), (int)(player.Center.Y / 16)].wall == ModContent.WallType<Tiles.Overgrow.WallOvergrowInvisible>();
        }

        public override bool CustomBiomesMatch(Player other)
        {
            BiomeHandler modOther = other.GetModPlayer<BiomeHandler>();
            bool allMatch = true;
            allMatch &= ZoneGlass == modOther.ZoneGlass;
            allMatch &= ZoneVoidPre == modOther.ZoneVoidPre;
            allMatch &= ZoneJungleCorrupt == modOther.ZoneJungleCorrupt;
            allMatch &= ZoneJungleBloody == modOther.ZoneJungleBloody;
            allMatch &= ZoneJungleHoly == modOther.ZoneJungleHoly;
            allMatch &= ZoneOvergrow == modOther.ZoneOvergrow;
            return allMatch;
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            BiomeHandler modOther = other.GetModPlayer<BiomeHandler>();
            modOther.ZoneGlass = ZoneGlass;
            modOther.ZoneVoidPre = ZoneVoidPre;
            modOther.ZoneJungleCorrupt = ZoneJungleCorrupt;
            modOther.ZoneJungleBloody = ZoneJungleBloody;
            modOther.ZoneJungleHoly = ZoneJungleHoly;
            modOther.ZoneOvergrow = ZoneOvergrow;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ZoneGlass;
            flags[1] = ZoneVoidPre;
            flags[2] = ZoneJungleCorrupt;
            flags[3] = ZoneJungleBloody;
            flags[4] = ZoneJungleHoly;
            flags[5] = ZoneOvergrow;
            writer.Write(flags);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            ZoneGlass = flags[0];
            ZoneVoidPre = flags[1];
            ZoneJungleCorrupt = flags[2];
            ZoneJungleBloody = flags[3];
            ZoneJungleHoly = flags[4];
            ZoneOvergrow = flags[5];
        }

        public override void PreUpdate()
        {
            float distance = Vector2.Distance(Main.LocalPlayer.Center, LegendWorld.RiftLocation);
            if (distance <= 1500)
            {
                float val = (1500 / distance - 1) * 2;
                if (val <= 1) val = 1;
                if (val >= 2.5f) val = 2.5f;
                Lighting.brightness = 1 / val;
            }

            if (ZoneVoidPre)
            {
                Overlay.state = 1;

                if (player.GetModPlayer<AbilityHandler>().pure.Locked)
                {
                    player.AddBuff(mod.BuffType("DarkSlow"), 5);
                }
            }
            else if (ZoneJungleCorrupt)
            {
                Overlay.state = 2;
            }
            else if (ZoneJungleBloody)
            {
                Overlay.state = 3;
            }
            else if (ZoneJungleHoly)
            {
                Overlay.state = (int)OverlayState.HolyJungle;
            }


            if (ZoneOvergrow && Main.rand.Next(3) == 0)
            {
                Dust.NewDustPerfect(Main.screenPosition - Vector2.One * 100 + new Vector2(Main.rand.Next(Main.screenWidth + 200), Main.rand.Next(Main.screenHeight + 200)),
                ModContent.DustType<Dusts.OvergrowDust>(), Vector2.Zero, 0, new Color(255, 255, 205) * 0.05f, 2);
            }

            //Codex Unlocks
            if (ZoneGlass && player.GetModPlayer<CodexHandler>().Entries.Any(entry => entry is VitricEntry && entry.Locked))
            {
                Helper.UnlockEntry<VitricEntry>(player);
            }
        }
    }

    public partial class LegendWorld
    {
        public static int voidTiles;
        public static int evilJungleTiles;
        public static int bloodJungleTiles;
        public static int holyJungleTiles;
        public override void TileCountsAvailable(int[] tileCounts)
        {
            voidTiles = tileCounts[mod.TileType("Void1")] + tileCounts[mod.TileType("Void2")];
            evilJungleTiles = tileCounts[mod.TileType("GrassJungleCorrupt")];
            bloodJungleTiles = tileCounts[mod.TileType("GrassJungleBloody")];
            holyJungleTiles = tileCounts[mod.TileType("GrassJungleHoly")];
        }
    }

    public partial class StarlightRiver : Mod
    {
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneJungleCorrupt)
            {
                tileColor = tileColor.MultiplyRGB(new Color(130, 100, 145));
                backgroundColor = backgroundColor.MultiplyRGB(new Color(130, 100, 145));
            }

            if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneJungleBloody)
            {
                tileColor = tileColor.MultiplyRGB(new Color(155, 120, 90));
                backgroundColor = backgroundColor.MultiplyRGB(new Color(155, 120, 90));
            }

            if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneJungleHoly)
            {
                tileColor = tileColor.MultiplyRGB(new Color(70, 150, 165));
                backgroundColor = backgroundColor.MultiplyRGB(new Color(70, 150, 165));
            }

            if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneVoidPre)
            {
                tileColor = tileColor.MultiplyRGB(Color.Purple);
                backgroundColor = backgroundColor.MultiplyRGB(Color.Purple);
            }

            if (LegendWorld.starfall)
            {
                tileColor = new Color(20, 50, 60);
                backgroundColor = new Color(10, 15, 20);
            }
        }
    }
}
