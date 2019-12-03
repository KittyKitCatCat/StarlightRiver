using System.IO;
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
        public override void UpdateBiomes()
        {
            ZoneGlass = (player.Hitbox.Intersects(LegendWorld.VitricBiome));
            ZoneVoidPre = (LegendWorld.voidTiles > 50);
            ZoneJungleCorrupt = (LegendWorld.evilJungleTiles > 50);
            ZoneJungleBloody = (LegendWorld.bloodJungleTiles > 50);
            ZoneJungleHoly = (LegendWorld.holyJungleTiles > 50);
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
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = ZoneGlass;
            flags[1] = ZoneVoidPre;
            flags[2] = ZoneJungleCorrupt;
            flags[3] = ZoneJungleBloody;
            flags[4] = ZoneJungleHoly;
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
        }

        public override void PreUpdate()
        {
            if (ZoneVoidPre)
            {
                Overlay.visible = true;
                Overlay.state = 1;

                if (player.GetModPlayer<AbilityHandler>().AbilityPure.Locked)
                {
                    player.AddBuff(mod.BuffType("DarkSlow"), 5);
                }
            }
            else if (ZoneJungleCorrupt)
            {
                Overlay.visible = true;
                Overlay.state = 2;
            }
            else if (ZoneJungleBloody)
            {
                Overlay.visible = true;
                Overlay.state = 3;
            }
            else if (ZoneJungleHoly)
            {
                Overlay.visible = true;
                Overlay.state = 4;
            }
            else
            {
                Overlay.visible = false;
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

            if (LegendWorld.starfall)
            {
                tileColor = new Color(20, 50, 60);
                backgroundColor = new Color(10, 15, 20);
            }
        }
    }
}
