using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Abilities;
using StarlightRiver.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.NPCs.Boss
{
    sealed class VitricBoss : ModNPC
    {
        Vector2 spawnPos;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glass Tax Returns");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.lifeMax = 3500;
            npc.damage = 30;
            npc.defense = 10;
            npc.knockBackResist = 0f;
            npc.width = 140;
            npc.height = 143;
            npc.value = Item.buyPrice(0, 20, 0, 0);
            npc.npcSlots = 15f;
            npc.boss = true;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/GlassBoss");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.625f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.6f);
        }

        public override bool CheckDead()
        {
            if (!LegendWorld.AnyBossDowned)
            {
                LegendWorld.ForceStarfall = true;
            }

            if (!LegendWorld.GlassBossDowned)
            {
                LegendWorld.GlassBossDowned = true;
                LegendWorld.AnyBossDowned = true;
            }

            return true;
        }

        public override void AI()
        {
            //npc.ai Field meanings:
            //0: the action to take (which action method should I be running every tick?)
            //1: timer, increments every frame
            //2: dash counter, makes sure the boss does something other than a dash even if it's too far away
            //3: phase state

            if (npc.ai[3] == 0) { spawnPos = npc.position; npc.ai[3] = 1; }
            Main.NewText(npc.ai[0] + "/" + npc.ai[1] + "/" + npc.ai[2] + "/" + npc.ai[3]);
            Player player = Main.player[npc.target];
            npc.ai[1]++; //keeps the timer ticking

            if (npc.life <= npc.lifeMax / 2 && npc.ai[3] == 1)
            {
                npc.GetGlobalNPC<ShieldHandler>().MaxShield = 0;
                npc.ai[1] = 1;
                npc.ai[0] = 5;
                npc.ai[3] = 2;
            }

            switch (npc.ai[0])
            {
                case 0: CrystalShield(npc, 500, 3, Main.expertMode ? 300 : 150); break;
                case 1: PassiveMovement(0.2f, npc.ai[3] == 1 ? 4.5f : 5.2f, npc.ai[3] == 1 ? 90 : 75); break;
                case 2: Dash(player.Center); break;
                case 3: FallingRockAttack(); break;
                case 4: BombAttack(); break;

                case 5: GoHome(0); break;
            }
        }
        private void GoHome(int poststate)
        {
            npc.scale -= 0.05f;
            if (npc.ai[1] >= 20)
            {
                for(int k = 0; k <= 100; k++)
                {
                    Dust.NewDustPerfect(npc.Center, ModContent.DustType<Dusts.Air>(), Vector2.One.RotatedByRandom(6.28f));
                }
                
                npc.position = spawnPos;

                for (int k = 0; k <= 100; k++)
                {
                    Dust.NewDustPerfect(npc.Center, ModContent.DustType<Dusts.Air>(), Vector2.One.RotatedByRandom(6.28f));
                }

                npc.scale = 1;
                npc.ai[1] = 0;
                npc.ai[0] = poststate;
            }
        }

        private void CrystalShield(NPC npc, int time, int count, int maxShield)
        {
            if (npc.ai[1] == 1)
            {
                for (float rot = 0; rot < 6.28f; rot += 6.28f / count)
                {
                    int ward = NPC.NewNPC((int)npc.Center.X + 8, (int)npc.Center.Y + 12, ModContent.NPCType<Ward>());
                    Main.npc[ward].velocity = (new Vector2(6, 0)).RotatedBy(rot);
                }
                npc.immortal = true;
                npc.velocity *= 0;
                npc.frame.Y = 1 * npc.height;
            }
            if(npc.ai[1] >= time || !Main.npc.Any(T => T.type == ModContent.NPCType<Ward>() && T.active))
            {
                foreach(NPC npc2 in Main.npc.Where(npc2 => npc2.type == ModContent.NPCType<Ward>() && npc2.active))
                {
                    npc.GetGlobalNPC<ShieldHandler>().MaxShield += maxShield / count;
                    npc.GetGlobalNPC<ShieldHandler>().Shield += maxShield / count;
                    Helper.Kill(npc2);
                    for(int k = 0; k <= Vector2.Distance(npc.Center, npc2.Center); k++)
                    {
                        Dust.NewDustPerfect(Vector2.Lerp(npc.Center, npc2.Center, k / Vector2.Distance(npc.Center, npc2.Center)), ModContent.DustType<Dusts.Air>());
                    }
                }
                npc.immortal = false;
                npc.frame.Y = 0 * npc.height;
                npc.ai[1] = 0;
                npc.ai[0] = 1;
            }
        }

        private void PassiveMovement(float acceleration, float maxspeed, float attackspeed)
        {
            npc.TargetClosest();
            Player player = Main.player[npc.target];
            npc.velocity += -Vector2.Normalize(npc.Center - (player.Center + new Vector2(0, -250))) * acceleration;
            if (npc.velocity.Length() >= maxspeed) npc.velocity = Vector2.Normalize(npc.velocity) * maxspeed;

            if(npc.ai[1] >= attackspeed)
            {
                if ((npc.Center - player.Center).Length() >= 300 && npc.ai[2] < 3)
                {
                    npc.velocity = Vector2.Zero;
                    npc.ai[0] = 2;
                }
                else
                {
                    npc.ai[0] = Main.rand.Next(3, 5);
                    npc.ai[2] = 0;
                }
                npc.ai[1] = 0;
            }
        }

        private void Dash(Vector2 target)
        {    
            npc.rotation += (0.05f + npc.velocity.Length() * 0.02f) * npc.direction;

            if (npc.ai[1] == 1)
            {
                for (int k = 0; k <= 100; k++)
                {
                    Dust.NewDustPerfect(npc.Center - Vector2.Normalize(npc.Center - target) * k * 20, ModContent.DustType<Dusts.VitricBossTell>(),
                        Vector2.Normalize(npc.Center - target).RotatedBy((Main.rand.Next(2) == 0 ? 1.58f : -1.58f)) * Main.rand.NextFloat(20f), 0, Color.LightCyan * 0.5f, 3.5f);
                }
            }

            if (npc.ai[1] >= 30 && npc.ai[1] < 45)
            {
                npc.velocity -= Vector2.Normalize(npc.Center - target) * 1.1f;
            }
            if(npc.ai[1] >= 60)
            {
                npc.velocity += Vector2.Normalize(npc.Center - target) * 0.3f;
            }
            if(npc.ai[1] >= 105)
            {
                npc.ai[0] = 1;
                npc.ai[1] = 0;
                npc.ai[2]++;// counts dashes so the boss wont just keep dashing
                npc.velocity = Vector2.Zero;
            }   
            
            if(npc.ai[1] >= 30)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.Air>());
            }
        }

        private void FallingRockAttack()
        {
            Player player = Main.player[npc.target];

            if(npc.ai[1] == 45)
            {
                Main.LocalPlayer.GetModPlayer<StarlightPlayer>().Shake = 40;
            }
            if(npc.ai[1] == 60)
            {
                for(int k = -5; k <= 5; k++)
                {
                    Projectile.NewProjectile(player.position + new Vector2(k * Main.rand.NextFloat(150, 210), -800 + Main.rand.Next(300)), new Vector2(0, 7.2f), ModContent.ProjectileType<Projectiles.GlassSpike>(), 20, 1);
                }
            }
            if(npc.ai[1] >= 90)
            {
                npc.ai[0] = 1;
                npc.ai[1] = 0;
            }
        }

        private void BombAttack()
        {
            Player player = Main.player[npc.target];
            Projectile.NewProjectile(npc.Center, Vector2.Normalize(npc.Center - player.Center), ModContent.ProjectileType<VitricBomb>(), 5, 0);
            npc.ai[0] = 1;
            npc.ai[1] = 0;
        }
    }

    sealed class Ward : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resonant Crystal");
        }
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 48;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 1;
            npc.HitSound = SoundID.Item90;
            npc.DeathSound = SoundID.Item101;
            npc.value = 0f;
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            npc.immortal = true;
            npc.noGravity = true;
        }
        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();

            if (AbilityHelper.CheckDash(player, npc.Hitbox))
            {
                player.velocity *= -1.25f;
                mp.dash.Active = false;
                mp.dash.OnExit();
                Helper.Kill(npc);

                for (int k = 0; k <= 20; k++)
                {
                    Dust.NewDust(npc.position, 32, 46, ModContent.DustType<Dusts.Air>());
                    Dust.NewDust(npc.position, 32, 46, ModContent.DustType<Dusts.Glass2>());
                    Main.PlaySound(SoundID.Shatter, npc.Center);
                }
                for (int k = 0; k <= 3; k++)
                {
                    Gore.NewGore(npc.position, Vector2.Zero, mod.GetGoreSlot("Gores/Ward" + k), 1f);
                }
            }

            npc.velocity *= 0.95f;
        }

        public static Texture2D glow = ModContent.GetTexture("StarlightRiver/NPCs/Boss/CrystalGlow");

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.Draw(glow, npc.position - Main.screenPosition + new Vector2(0, 3), new Color(255, 255, 255) * (float)Math.Sin(LegendWorld.rottime));
        }
    }

    sealed class VitricBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 46;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Volatile Crystal");
            Main.projFrames[projectile.type] = 8;
        }       
        public override void AI()
        {
            projectile.velocity *= 0.9f;
            if(Main.player.Any(player => AbilityHelper.CheckDash(player, projectile.Hitbox)))
            {
                projectile.active = false;
                for (int k = 0; k <= 100; k++)
                {
                    Dust.NewDustPerfect(projectile.Center, ModContent.DustType<Dusts.Air>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(0.8f));
                }
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CounterWisp>(), 150, 1, Main.player.FirstOrDefault().whoAmI);
            }
            if (++projectile.frameCounter >= (projectile.timeLeft > 90 ? 4 : 2))
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 7)
                {
                    projectile.frame = 0;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for(float k = 0; k <= 6.28f; k+= 6.28f / 6)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.One.RotatedBy(k) * 2.2f, ModContent.ProjectileType<Projectiles.GlassSpike>(), 20, 1);
            }
            for (int k = 0; k <= 100; k++)
            {
                Dust.NewDustPerfect(projectile.Center, ModContent.DustType<Dusts.Air>(), Vector2.One.RotatedByRandom(6.28f) * Main.rand.NextFloat(0.8f));
            }
        }
    }

    sealed class CounterWisp : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 16;
            projectile.height = 16;
            projectile.penetrate = -1;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 3;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vengence");
        }
        public override string Texture => "StarlightRiver/Invisible";
        public override void AI()
        {
            projectile.timeLeft = 2;
            projectile.ai[0] += 0.04f;
            Dust.NewDustPerfect(projectile.Center, ModContent.DustType<Dusts.Starlight>());
            Vector2 goal = Main.npc.FirstOrDefault(npc => npc.type == ModContent.NPCType<VitricBoss>() && npc.active).Center;
            projectile.velocity = -Vector2.Normalize(projectile.Center - goal) * projectile.ai[0];
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.timeLeft = 0;
        }
    }
}
