﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace StarlightRiver.Structures
{
    public partial class GenHelper
    {
        public static void VoidAltarGen(GenerationProgress progress)
        {
            progress.Message = "Opening the Gates...";

            Texture2D Courtyard = ModContent.GetTexture("StarlightRiver/Structures/VoidAltar");
            Vector2 spawn = new Vector2(Main.maxTilesX / 2, Main.maxTilesY - 200);

            for (int y = 0; y < Courtyard.Height; y++) // for every row
            {
                Color[] rawData = new Color[Courtyard.Width]; //array of colors
                Rectangle row = new Rectangle(0, y, Courtyard.Width, 1); //one row of the image
                Courtyard.GetData<Color>(0, row, rawData, 0, Courtyard.Width); //put the color data from the image into the array

                for (int x = 0; x < Courtyard.Width; x++) //every entry in the row
                {
                    Main.tile[(int)spawn.X + x, (int)spawn.Y + y].ClearEverything(); //clear the tile out
                    Main.tile[(int)spawn.X + x, (int)spawn.Y + y].liquidType(0); // clear liquids

                    ushort placeType = 0;
                    ushort wallType = 0;
                    switch (rawData[x].R) //select block
                    {
                        case 10: placeType = TileID.Ash; break;
                        case 20: placeType = (ushort)ModContent.TileType<Tiles.Void1>(); break;
                        case 30: placeType = (ushort)ModContent.TileType<Tiles.Void2>(); break;
                        case 40: placeType = (ushort)ModContent.TileType<Tiles.VoidDoorOn>(); break;
                    }
                    switch (rawData[x].B) //select wall
                    {
                        case 10: wallType = (ushort)ModContent.WallType<Tiles.VoidWall>(); break;
                        case 20: wallType = (ushort)ModContent.WallType<Tiles.VoidWallPillar>(); break;
                        case 30: wallType = (ushort)ModContent.WallType<Tiles.VoidWallPillarS>(); break;
                    }

                    if (placeType != 0) { WorldGen.PlaceTile((int)spawn.X + x, (int)spawn.Y + y, placeType, true, true); } //place block
                    if (wallType != 0) { WorldGen.PlaceWall((int)spawn.X + x, (int)spawn.Y + y, wallType, true); } //place wall
                }
            }
        }
    }
}