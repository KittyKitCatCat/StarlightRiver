using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Abilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.NPCs.Passive
{
    class DesertWisp : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Wisp");
        }
        public override void SetDefaults()
        {
            npc.width = 18;
            npc.height = 18;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.immortal = true;
            npc.value = 0f;
            npc.knockBackResist = 0f;
            npc.aiStyle = 65;
        }

        bool fleeing = false;
        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            Vector2 distance = player.Center - npc.Center;

            Dust.NewDustPerfect(npc.Center, mod.DustType("Air"), new Vector2(Main.rand.Next(-20, 20) * 0.01f, Main.rand.Next(-20, 20) * 0.01f));
            if (LegendWorld.rottime == 0)
            {
                for (float k = 0; k <= Math.PI * 2; k += (float)Math.PI / 20)
                {
                    Dust.NewDustPerfect(npc.Center, mod.DustType("Air"), new Vector2((float)Math.Cos(k), (float)Math.Sin(k)), 0, default, 0.5f);
                }
            }
            float range = 180;
            if (npc.localAI[2] > 0)
            {
                npc.localAI[2] -= 3.5f;
                return;
            }
            if ((distance.Length() <= range && !(mp.AbilityWisp.Active)) || Main.dayTime)
            {
                fleeing = true;
            }
            if (fleeing)
            {
                npc.velocity.Y = 10;
                npc.velocity.X = 0;
                Dust.NewDustPerfect(npc.Center, mod.DustType("Air"), new Vector2(Main.rand.Next(-10, 10) * 0.1f, Main.rand.Next(-10, 10) * 0.1f));
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.player.ZoneOverworldHeight && !Main.dayTime && spawnInfo.player.ZoneDesert) ? 1f : 0f;
        }
    }

    class DesertWisp2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Desert Wisp");
        }
        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.immortal = true;
            npc.value = 0f;
            npc.knockBackResist = 0f;
            npc.aiStyle = 65;
        }
        bool fleeing = false;
        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();
            Vector2 distance = player.Center - npc.Center;

            Dust.NewDustPerfect(npc.Center, mod.DustType("Air"), new Vector2(Main.rand.Next(-40, 40) * 0.01f, Main.rand.Next(-40, 40) * 0.01f));
            if (LegendWorld.rottime == 0)
            {
                for (float k = 0; k <= Math.PI * 2; k += (float)Math.PI / 20)
                {
                    Dust.NewDustPerfect(npc.Center, mod.DustType("Air"), new Vector2((float)Math.Cos(k), (float)Math.Sin(k)), 0, default, 0.6f);
                }
            }
            float range = 120;
            if (npc.localAI[2] > 0)
            {
                npc.localAI[2] -= 3.5f;
                return;
            }
            if (distance.Length() <= range && !(mp.AbilityWisp.Active))
            {
                fleeing = true;
            }
            else
            {
                fleeing = false;
            }
            if (fleeing)
            {
                npc.velocity += Vector2.Normalize(distance) * -10;
                Dust.NewDustPerfect(npc.Center, mod.DustType("Air"), new Vector2(Main.rand.Next(-10, 10) * 0.1f, Main.rand.Next(-10, 10) * 0.1f));
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.player.ZoneRockLayerHeight && spawnInfo.player.GetModPlayer<BiomeHandler>().ZoneGlass) ? 3f : 0f;
        }
    }
}
