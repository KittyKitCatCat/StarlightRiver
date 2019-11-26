using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework.Graphics;
using System;
using StarlightRiver.Structures;

namespace StarlightRiver
{
    
    public partial class LegendWorld : ModWorld
    {
        public static Vector2 BookSP;
        public static Vector2 DashSP;
        public static Vector2 WispSP;
        public static Vector2 PureSP;
        public static Vector2 SmashSP;

        public static bool ForceStarfall = false; 

        //Boss Flags
        public static bool AnyBossDowned = false;
        public static bool GlassBossDowned = false;

        public static bool SealOpen = false;

        //Voidsmith
        public static int[] NPCUpgrades = new int[] { 0,0 };

        public static List<Vector2> PureTiles = new List<Vector2> { };
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            int SurfaceIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));
            int HellIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Underworld"));
            int DesertIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            int TrapsIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Traps"));

            if (ShiniesIndex != -1)
            {
                tasks.Insert(DesertIndex + 1, new PassLegacy("Starlight River Vitric Desert", GenerateCrystalCaverns));
                tasks.Insert(DesertIndex + 2, new PassLegacy("Starlight River Codex", GenHelper.BookAltarGen));
                tasks.Insert(DesertIndex + 3, new PassLegacy("Starlight River Dash", GenHelper.WindsAltarGen));

                tasks.Insert(ShiniesIndex + 1, new PassLegacy("Starlight River Ores", EbonyGen));
                //tasks.Insert(ShiniesIndex + 2, new PassLegacy("Starlight River Caves", DolomiteGen));
                //tasks.Insert(HellIndex + 1, new PassLegacy("Starlight River Void Altar", GenHelper.VoidAltarGen));

                tasks.Insert(TrapsIndex + 1, new PassLegacy("Starlight Traps", GenHelper.BoulderSlope));

                tasks.Insert(SurfaceIndex + 1, new PassLegacy("Starlight River Ruins", GenHelper.RuinsGen));
            }
        }
        public override void PostWorldGen()
        {
        }
        
        public int MaxCrystalCaveDepth = 0;
        public static Vector2 vitricTopLeft = new Vector2();
        public static Vector2 vitricSize = new Vector2();
        public static Rectangle VitricBiome => new Rectangle((int)vitricTopLeft.X, (int)vitricTopLeft.Y, (int)vitricSize.X, (int)vitricSize.Y);
        
        /// <summary>
        /// Generates a crystal cavern at position topCentre, where topCentre is exactly what it is called.
        /// </summary>
        /// <param name="centre">The top centre point of the cavern.</param>
        private void GenerateCrystalCaverns(GenerationProgress progress)
        {
            progress.Message = "Vitrifying Desert...";
            int size = (int)(Main.maxTilesX / 26f); //Width of the cavern; value shown here is half the size. So, functional size is actually size * 2.
            Point centre = new Point(WorldGen.UndergroundDesertLocation.X + size, WorldGen.UndergroundDesertLocation.Y + 400);
            float depth = 0; //Depth of the cave
            float height = 0; //Height of cave
            int minHeight = 0;
            for (int i = -size; i < size; ++i) //Digs out a cave, mayhaps placeholder
            {
                int x = i + centre.X;

                if (i < -size / 2.5f) //Dig up
                {
                    height += WorldGen.genRand.Next(10, 20) * 0.1f;
                    depth += 1f;
                }
                if (i > size / 2.5f) //Dig back down
                {
                    height -= WorldGen.genRand.Next(10, 20) * 0.1f;
                    depth -= 1f;
                }

                if (height > minHeight)
                    minHeight = (int)height;
                if (depth > MaxCrystalCaveDepth)
                    MaxCrystalCaveDepth = (int)depth;

                int minHei = -(int)(height / 2.5f);
                if (minHei < -56f) //Clamp height
                    minHei = -56;
                int maxHei = (int)(depth / 2.5f) + 20;
                if (maxHei > 74) //Clamp height
                    maxHei = 74;
                for (int j = minHei; j < maxHei; ++j) //Place walls
                {
                    int y = centre.Y + j;
                    Main.tile[x, y].active(false);
                    if ((j > maxHei - 8 || j < minHei + 8 || i < -size + 8 || i > size - 8) && WorldGen.genRand.Next(4) == 0)
                    {
                        WorldGen.TileRunner(x, y, 7, 2, j < maxHei - 8 ? TileID.HardenedSand : ModContent.TileType<Tiles.Vitric.VitricSand>(), true, 0, 0, false, true);
                        continue;
                    }
                    WorldGen.KillWall(x, y, false);
                    Main.tile[x, y].liquid = 0;
                }
            }
            for (int i = 0; i < 4; ++i) //Places platforms
            {
                Main.tile[centre.X - 12 + i, centre.Y + (MaxCrystalCaveDepth / 5)].active(false);
                WorldGen.PlaceTile(centre.X - 12 + i, centre.Y + (MaxCrystalCaveDepth / 5), TileID.Platforms, true, true, 2);
            }
            for (int i = -4 - WorldGen.genRand.Next(7); i < 10 + WorldGen.genRand.Next(7); ++i) //Digs opening
            {
                for (int j = -12 - WorldGen.genRand.Next(5); j < 14 + WorldGen.genRand.Next(5); ++j)
                {
                    Main.tile[centre.X + i, centre.Y - 20 - j].active(false);
                }
            }
            Vector2 sandMid = GetGroundDirectional(new Vector2(0, 1), centre.ToVector2(), TileID.Platforms);
            GenerateSandDunes(new Point((int)sandMid.X, (int)sandMid.Y + 0), (int)(size * 1.15f)); //Generates SAND under the crystals. Wacky!
            GenerateCrystals(centre); //I wonder what this does
            GenerateMiniCrystals(centre, 30, (int)(size * 0.7f));

            vitricTopLeft = centre.ToVector2() - new Vector2(size, 100);
            vitricSize = new Vector2(size * 2, 200);
        }

        ///TODO: Add crystal stairs

        private void GenerateMiniCrystals(Point centre, int reps, int width)
        {
            for (int i = 0; i < reps; ++i)
            {
                if (WorldGen.genRand.Next(3) == 0)
                {
                    Vector2 pos = centre.ToVector2() + new Vector2(WorldGen.genRand.Next(-width, width), WorldGen.genRand.Next((int)(MaxCrystalCaveDepth * 0.8f)));
                    while (Main.tile[(int)pos.X, (int)pos.Y].active())
                        pos = centre.ToVector2() + new Vector2(WorldGen.genRand.Next(-width, width), WorldGen.genRand.Next((int)(MaxCrystalCaveDepth * 0.8f)));
                    Vector2 offset = pos + new Vector2(WorldGen.genRand.Next(1, 20) * (WorldGen.genRand.NextBool() ? 1 : -1), WorldGen.genRand.Next(1, 20) * (WorldGen.genRand.NextBool() ? 1 : -1));
                    Vector2 plPos = pos;
                    int siz = WorldGen.genRand.Next(7, 12);
                    for (int j = 0; j < siz; ++j)
                    {
                        WorldGen.PlaceTile((int)plPos.X, (int)plPos.Y, ModContent.TileType<Tiles.Vitric.VitricGlassCrystal>(), true, true);
                        WorldGen.KillTile((int)plPos.X, (int)plPos.Y, false, true);
                        int wid = WorldGen.genRand.Next(1, 4);
                        for (int k = 0; k < wid; ++k)
                        {

                        }
                        plPos += Vector2.Normalize(pos - offset) * 0.75f;
                    }
                }
            }
        }

        private void GenerateSandDunes(Point midPoint, int size)
        {
            for (int i = (int)(-size * 0.85f); i < (int)(size * 0.86f); ++i) //Places sand.
            {
                int sHei = (int)(Math.Sin((i / 18f) + (3.14f / 3)) * 8.0f); //Sin wave placement for Y height
                for (int j = sHei; j < 10 + WorldGen.genRand.Next(12, 19); ++j)
                {
                    int off = (int)(i / 5f) * ((i >= 0) ? -1 : 1);
                    if (Main.tile[midPoint.X + i, (midPoint.Y + j) + off].type == (ushort)ModContent.TileType<Tiles.Vitric.VitricSand>())
                        continue;
                    WorldGen.KillTile(midPoint.X + i, (midPoint.Y + j) + off, true, false, true);
                    WorldGen.PlaceTile(midPoint.X + i, (midPoint.Y + j) + off, ModContent.TileType<Tiles.Vitric.VitricSand>(), true, true, -1, 0);
                }
            }
        }

        private void GenerateCrystals(Point tC)
        {
            float rot = 0f; //Rotation of crystal/placement used later
            int totalReps = (int)(70 * (Main.maxTilesX / 4000f)); //Total repeats
            float shortTau = 6.28f; //Helper variable
            float side = shortTau / 4; //Helper variable
            for (int i = 0; i < totalReps; ++i)
            {
                rot += shortTau / totalReps; //Increases rotation uniformly

                if (rot > shortTau) //Caps angle
                    rot = 0;
                Vector2 randomWallLocation = GetGroundDirectional(new Vector2(0, -1f).RotatedBy(rot), tC.ToVector2(), ModContent.TileType<Tiles.Vitric.VitricGlassCrystal>())
                    + (new Vector2(0, -1).RotatedBy(rot) * 3); //Position of a wall. Starts off going UP, then goes clockwise.

                int runs = 0;

                while (true)
                {
                    runs++;
                    if (runs > 20)
                        break;

                    //Size of the crystal, going into the wall(s)
                    int dist = WorldGen.genRand.Next(8, 14);
                    Vector2 placePosition = randomWallLocation - (Vector2.Normalize(randomWallLocation - tC.ToVector2()) * (dist * 2.5f)); //Position used for placement base

                    Point nearestWallPositionDifference = new Point(-20, -20);
                    Vector2 wallPos = placePosition + nearestWallPositionDifference.ToVector2();
                    int max = 50;
                    for (int j = -max; j < max; ++j)
                    {
                        if (Main.tile[(int)wallPos.X, (int)wallPos.Y].active() && Math.Abs(nearestWallPositionDifference.X) > Math.Abs(j) && Main.tile[(int)wallPos.X, (int)wallPos.Y].type != ModContent.TileType<Tiles.Vitric.VitricGlassCrystal>())
                            nearestWallPositionDifference.X = j;
                        for (int k = -max; k < max; ++k)
                        {
                            if (Main.tile[(int)wallPos.X, (int)wallPos.Y].active() && Math.Abs(nearestWallPositionDifference.Y) > Math.Abs(k) && Main.tile[(int)wallPos.X, (int)wallPos.Y].type != ModContent.TileType<Tiles.Vitric.VitricGlassCrystal>())
                                nearestWallPositionDifference.Y = k;
                        }
                        wallPos = placePosition + nearestWallPositionDifference.ToVector2();
                    }

                    Vector2 dir = (Vector2.Normalize(randomWallLocation - wallPos));//.RotatedBy(WorldGen.genRand.Next(-60, 70) * 0.005f);
                    if (WorldGen.genRand.Next(Math.Abs((int)placePosition.X - tC.X)) < Main.maxTilesX / 80) //Biases crystals towards the sides
                        continue;
                    for (int j = 0; j < dist * 3; ++j)
                    {
                        Vector2 negDir = Vector2.Normalize(new Vector2(1 / dir.X, 1 / -dir.Y));
                        Vector2 actualPlacePos = placePosition - (negDir * 2);
                        for (int k = -5; k < 5; ++k)
                        {
                            Main.tile[(int)actualPlacePos.X, (int)actualPlacePos.Y].active(false);
                            WorldGen.PlaceTile((int)actualPlacePos.X, (int)actualPlacePos.Y, ModContent.TileType<Tiles.Vitric.VitricGlassCrystal>());
                            actualPlacePos += (negDir / 4);
                        }
                        placePosition += dir;
                    }
                    break;
                }

                //PlaceCrystal(tC, randomWallLocation, (Math.Abs((int)randomWallLocation.X - tC.X) / 6) + (WorldGen.genRand.Next(8, 23)));
            }
        }
        
        /// <summary>
        /// Moves from starting point p to the first solid block it touches according to direction dir. Skips tiles of types included the ignoredTileIDs array.
        /// </summary>
        /// <returns></returns>
        private Vector2 GetGroundDirectional(Vector2 dir, Vector2 p, params int[] ignoredTileIDs)
        {
            Vector2 actualPos = p;
            while (!Main.tile[(int)actualPos.X, (int)actualPos.Y].active() || ignoredTileIDs.Any(x => x == Main.tile[(int)actualPos.X, (int)actualPos.Y].type))
                actualPos += dir;
            return actualPos;
        }
        
        private void EbonyGen(GenerationProgress progress)
        {
            progress.Message = "Making the World Impure...";

            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * .0015); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(0, (int)WorldGen.worldSurfaceHigh);

                if (Main.tile[x, y].type == TileID.Dirt  && Math.Abs(x - Main.maxTilesX / 2) >= Main.maxTilesX / 6)
                {
                    WorldGen.TileRunner(x, y, (double)WorldGen.genRand.Next(10, 11), 1, ModContent.TileType<Tiles.OreEbony>(), false, 0f, 0f, false, true);
                }
            }
        }

        private void DolomiteGen(GenerationProgress progress)
        {
            progress.Message = "Shifting Tectonic Plates...";
            ushort Dolomite = (ushort)ModContent.TileType<Tiles.Dolomite.Dolomite>();

            for (int k = 0; k < (Main.rand.Next(4,8)); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)WorldGen.rockLayer, Main.maxTilesY);

                for(int i = x - 200; i <= x + 200; i++)
                {
                    for (int j = y - 100; j <= y + 100; j++)
                    {
                        if(i > 20 && j > 20 && i < Main.maxTilesX - 20 && j < Main.maxTilesY - 20)
                        {
                            if (Main.tile[i, j].type is TileID.Stone) { Main.tile[i, j].type = Dolomite; }

                            if (!Main.tile[i, j + 2].active() && Main.tile[i, j].type == Dolomite) { Main.tile[i, j + 1].type = (ushort)ModContent.TileType<Tiles.Dolomite.DolomiteHanging>(); }

                            if (i % 15 == 0 && Main.tile[i, j].active() && Main.tile[i, j].type == Dolomite && !Main.tile[i, j - 1].active() && Main.rand.Next(3) == 0)
                            {
                                GenerateBeam(i, j);
                            }
                        }
                    }
                }
            }
        }

        private void GenerateBeam(int x, int y)
        {
            for (int j = y - 1; j >= y - 100; j--)
            {
                if (j % 10 == 0 && Main.rand.Next(2) == 0)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        if (!Main.tile[x + k, j].active()) { Main.tile[x + k, j].type = TileID.WoodenBeam; }
                    }
                }

                if (x > 0 && j > 0 && x < Main.maxTilesX && j < Main.maxTilesY)
                {
                    Main.tile[x, j].wall = WallID.Wood;
                    if (Main.tile[x, j].active() || j == y - 100)
                    {
                        for (int k = -2; k <= 2; k++)
                        {
                            Main.tile[x + k, j].type = TileID.WoodBlock;
                        }
                        return;
                    }
                }
            }           
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Overgrow Generation
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        public void GenerateOvergrowRoom(int i, int j)
        {
            Texture2D Rooms = ModContent.GetTexture("StarlightRiver/Structures/Ruins");
            int variant = Main.rand.Next(5);

            // Generation Block
            for (int y = 0; y < 80; y++) // for every row
            {
                Color[] rawData = new Color[8]; //array of colors
                Rectangle row = new Rectangle(80 * variant, y, 80, 1); //one row of the image
                Rooms.GetData<Color>(0, row, rawData, 0, 80); //put the color data from the image into the array

                for (int x = 0; x < 80; x++) //every entry in the row
                {
                    Main.tile[i + x, j + y].ClearEverything();
                    Main.tile[i + x, j + y].slope(0);

                    ushort placeType = 0;
                    ushort wallType = 0;
                    switch (rawData[x].R) //select block
                    {
                        case 10: placeType = TileID.GrayBrick; break;
                        case 20: placeType = TileID.LeafBlock; break;
                    }
                    switch (rawData[x].B) //select wall
                    {
                        case 10: wallType = WallID.GrayBrick; break;
                    }

                    if (placeType != 0) { WorldGen.PlaceTile(i + x, j + y, placeType, true, true); } //place block
                    if (wallType != 0) { WorldGen.PlaceWall(i + x, j + y, wallType, true); } //place wall
                }
            }
        }

        public static float rottime = 0;
        public static bool starfall = false;
        public override void PreUpdate()
        {
            rottime += (float)Math.PI / 60;
            if (rottime >= Math.PI * 2)
            {
                rottime = 0;
            }
            if(Main.time == 12 && ((AnyBossDowned && !Main.bloodMoon && Main.rand.Next(11) == 0) || ForceStarfall))
            {
                starfall = true;
                Main.bloodMoon = false;
                ForceStarfall = false;
                Main.NewText("The Starlight River is Passing Through!",120, 241, 255);
            }

            if (starfall)
            {
                Player player = Main.LocalPlayer;
                
                if (Main.time % 2 == 0)
                {
                    Projectile.NewProjectile(new Vector2(Main.rand.Next(0, Main.maxTilesX) * 16 + Main.rand.Next(-16, 16), 100), Vector2.Zero, mod.ProjectileType("StarShard"), 500, 0.5f);
                }
                if(Main.dayTime)
                {
                    starfall = false;
                }
            }
        }

        public override void PostUpdate()
        {
            if (!Main.projectile.Any(proj => proj.type == ModContent.ProjectileType<Projectiles.Ability.Purifier>()) && PureTiles != null)
            {
                PureTiles.Clear();
            }

            if (!Main.npc.Any(n => n.type == ModContent.NPCType<NPCs.Pickups.Wind>() && n.active == true))
            {
                NPC.NewNPC((int)DashSP.X, (int)DashSP.Y, ModContent.NPCType<NPCs.Pickups.Wind>());
            }

            if (!Main.npc.Any(n => n.type == ModContent.NPCType<NPCs.Pickups.Lore>() && n.active == true))
            {
                NPC.NewNPC((int)BookSP.X, (int)BookSP.Y, ModContent.NPCType<NPCs.Pickups.Lore>());
            }
        }

        public override void Initialize()
        {
            vitricTopLeft = Vector2.Zero;

            AnyBossDowned = false;
            GlassBossDowned = false;
            SealOpen = false;

            ForceStarfall = false;

            NPCUpgrades = new int[] { 0, 0 };
            PureTiles = new List<Vector2>();

            BookSP = Vector2.Zero;
            DashSP = Vector2.Zero;
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                [nameof(vitricTopLeft)] = vitricTopLeft,
                [nameof(vitricSize)] = vitricSize,

                [nameof(AnyBossDowned)] = AnyBossDowned,
                [nameof(GlassBossDowned)] = GlassBossDowned,
                [nameof(SealOpen)] = SealOpen,

                [nameof(ForceStarfall)] = ForceStarfall,

                [nameof(NPCUpgrades)] = NPCUpgrades,

                [nameof(PureTiles)] = PureTiles,

                [nameof(BookSP)] = BookSP,
                [nameof(DashSP)] = DashSP             
            };
        }
        public override void Load(TagCompound tag)
        {
            vitricTopLeft = tag.Get<Vector2>(nameof(vitricTopLeft));
            vitricSize = tag.Get<Vector2>(nameof(vitricSize)); 

            AnyBossDowned = tag.GetBool(nameof(AnyBossDowned));
            GlassBossDowned = tag.GetBool(nameof(GlassBossDowned));
            SealOpen = tag.GetBool(nameof(SealOpen));

            ForceStarfall = tag.GetBool(nameof(ForceStarfall));

            NPCUpgrades = tag.GetIntArray(nameof(NPCUpgrades));           

            PureTiles = (List<Vector2>)tag.GetList<Vector2>(nameof(PureTiles));

            BookSP = tag.Get<Vector2>(nameof(BookSP));
            DashSP = tag.Get<Vector2>(nameof(DashSP));


            for (int k = 0; k <= PureTiles.Count - 1;  k++)
            {
                for(int i = (int)PureTiles[k].X - 16; i <= (int)PureTiles[k].X + 16; i++)
                {
                    for (int j = (int)PureTiles[k].Y - 16; j <= (int)PureTiles[k].Y + 16; j++)
                    {
                        Tile target = Main.tile[i, j];
                        if (target != null)
                        {
                            if (target.type == (ushort)mod.TileType("StonePure")) { target.type = TileID.Stone; }
                            if (target.type == (ushort)mod.TileType("OreIvory")) { target.type = (ushort)mod.TileType("OreEbony"); }
                            if (target.type == (ushort)mod.TileType("VoidDoorOff")) { target.type = (ushort)mod.TileType("VoidDoorOn"); }
                        }
                    }
                }
            }

            foreach(NPC npc in Main.npc)
            {
                if (npc.townNPC)
                {
                    npc.life = 250 + NPCUpgrades[0] * 50;
                }
            }
            PureTiles.Clear();
            PureTiles = new List<Vector2> { };
        }   
    }
}
