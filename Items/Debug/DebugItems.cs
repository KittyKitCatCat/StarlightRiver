﻿using Microsoft.Xna.Framework;
using System.Linq;
using StarlightRiver.Abilities;
using StarlightRiver.Codex;
using StarlightRiver.Gases;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using Terraria.IO;
using StarlightRiver.Dragons;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Tiles.Vitric;

namespace StarlightRiver.Items.Debug
{
    public class Kill : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 512;
            item.height = 512;
            item.damage = 1000;
            item.useStyle = 1;
            item.useAnimation = 10;
            item.useTime = 10;
            item.rare = 1;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kill");
            Tooltip.SetDefault("Big Damag");
        }
        public override bool CanUseItem(Player player)
        {
            if (player.name == "a")
            {
                player.KillMeForGood();
            }
            if (player.altFunctionUse == 2)
            {
                if (player.controlDown)
                {
                    item.damage -= 100;
                }
                if (player.controlUp)
                {
                    item.damage += 100;
                }
                Main.NewText(item.damage);
                return false;
            }
            return base.CanUseItem(player);
        }
    }
    public class GrassJungleCorrupt : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Transforms Jungles Corrupt");
            DisplayName.SetDefault("Evil");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.maxStack = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 5;
            item.useStyle = 1;
            item.consumable = false;
            item.shoot = mod.ProjectileType("Clentam");
            item.shootSpeed = 8f;
        }
    }
    public class GrassJungleCorrupt2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Transforms Jungles Crimson");
            DisplayName.SetDefault("Blood");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.maxStack = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 5;
            item.useStyle = 1;
            item.consumable = false;
            item.shoot = mod.ProjectileType("Clentam2");
            item.shootSpeed = 8f;
        }
    }
    public class GrassJungleCorrupt3 : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Transforms Jungle Hallow");
            DisplayName.SetDefault("Fae");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.maxStack = 1;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 5;
            item.useStyle = 1;
            item.consumable = false;
            item.shoot = mod.ProjectileType("Clentam3");
            item.shootSpeed = 8f;
        }
    }
    public class SealPlacer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seal Placer");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 2;
            item.useTime = 1;
            item.useStyle = 1;
            //item.consumable = true;
            item.autoReuse = true;
            item.createWall = ModContent.WallType<Tiles.Overgrow.WallOvergrowBrick>();
        }
    }
    public class FleshPlacer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Potion of Debugging 2");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = false;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.Rift.MainRift>();
        }

        public override bool UseItem(Player player)
        {
            int i = 0;
            string types = "Types: ";
            foreach (NPC npc in Main.npc.Where(npc => npc.type != 0)) { i++; types += npc.type + ", "; }

            int j = 0;
            foreach (NPC npc in Main.npc.Where(npc => npc.active)) { j++; }
           
            Main.NewText("Extant NPCs: " + i + "/201");
            Main.NewText("Active NPCs: " + j + "/201");
            Main.NewText(types);
            return true;
        }
    }
    public class DebugPlacer1 : QuickTileItem
    {
        public override string Texture => "StarlightRiver/Items/Debug/DebugPotion";
        public DebugPlacer1() : base("Debug Placer 1", "Suck my huge dragon dong", ModContent.TileType<Tiles.Overgrow.SetpieceAltar>(), 0) { }
    }
    public class DebugPlacer2 : QuickTileItem
    {
        public override string Texture => "StarlightRiver/Items/Debug/DebugPotion";
        public DebugPlacer2() : base("Debug Placer 2", "Suck my huge dragon dong", ModContent.TileType<AncientSandstone>(), 0) { }
    }
    public class DebugPlacer3 : QuickTileItem
    {
        public override string Texture => "StarlightRiver/Items/Debug/DebugPotion";
        public DebugPlacer3() : base("Debug Placer 3", "Suck my huge dragon dong", ModContent.TileType<VitricSpike>(), 0) { }
    }

    public class DebugPotion : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 64;
            item.height = 64;
            item.useStyle = 5;
            item.useAnimation = 10;
            item.useTime = 10;
            item.rare = 2;
            item.autoReuse = true;
        }
        public override string Texture => "StarlightRiver/MarioCumming";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Potion of Debugging");
            Tooltip.SetDefault("Effects vary");
        }

        public override bool UseItem(Player player)
        {
            CrystalGen();
            return true;
        }
        public static void CrystalGen()
        {
            Texture2D Altar = ModContent.GetTexture("StarlightRiver/Structures/Crystal");
            Vector2 spawn = Main.MouseWorld / 16;

            for (int y = 0; y < Altar.Height; y++) // for every row
            {
                Color[] rawData = new Color[Altar.Width]; //array of colors
                Rectangle row = new Rectangle(0, y, Altar.Width, 1); //one row of the image
                Altar.GetData(0, row, rawData, 0, Altar.Width); //put the color data from the image into the array

                for (int x = 0; x < Altar.Width; x++) //every entry in the row
                {
                    Main.tile[(int)spawn.X + x, (int)spawn.Y + y].ClearEverything(); //clear the tile out
                    Main.tile[(int)spawn.X + x, (int)spawn.Y + y].liquidType(0); // clear liquids

                    ushort placeType = 0;

                    switch (rawData[x].R) //select block
                    {
                        case 10: placeType = (ushort)ModContent.TileType<VitricCrystalCollision>(); break;
                        case 20: placeType = (ushort)ModContent.TileType<VitricCrystalBig>(); break;
                    }

                    switch (rawData[x].G)
                    {
                        case 10: Main.tile[(int)spawn.X + x, (int)spawn.Y + y].slope(1); break;
                        case 20: Main.tile[(int)spawn.X + x, (int)spawn.Y + y].slope(2); break;
                        case 30: Main.tile[(int)spawn.X + x, (int)spawn.Y + y].slope(3); break;
                        case 40: Main.tile[(int)spawn.X + x, (int)spawn.Y + y].slope(4); break;
                        case 50: Main.tile[(int)spawn.X + x, (int)spawn.Y + y].slope(5); break;
                    }

                    if (placeType != 0) { WorldGen.PlaceTile((int)spawn.X + x, (int)spawn.Y + y, placeType, true, true); } //place block
                }
            }
        }
    }

    public class DebugPotion2 : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 64;
            item.height = 64;
            item.useStyle = 5;
            item.useAnimation = 10;
            item.useTime = 10;
            item.rare = 2;
            item.createTile = ModContent.TileType<Tiles.Vitric.DenialAura>();
            item.noUseGraphic = true;
        }
        public override string Texture => "StarlightRiver/MarioCumming";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Potion of Debugging 2");
            Tooltip.SetDefault("Effects vary");
        }

        public override bool UseItem(Player player)
        {
            LegendWorld.OvergrowBossOpen = true;
            return true;
        }
        public override void HoldItem(Player player)
        {
            //StarlightRiver.Rotation = (player.Center - Main.MouseWorld).ToRotation() - 1.58f;
        }
    }
    public class RedKiller : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
            item.damage = 10;
            item.useStyle = 1;
            item.useAnimation = 10;
            item.useTime = 10;
            item.rare = 7;
            item.GetGlobalItem<ShieldBreakingItem>().RedHurting = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("RedKiller");
            Tooltip.SetDefault("Debugging Item");
        }
    }
}

