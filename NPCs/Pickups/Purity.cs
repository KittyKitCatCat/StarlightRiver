﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using StarlightRiver.Abilities;
using StarlightRiver.Codex.Entries;
using StarlightRiver.GUI;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.NPCs.Pickups
{
    class Purity : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corona of Purity");
        }
        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 32;
            npc.aiStyle = -1;
            npc.immortal = true;
            npc.lifeMax = 1;
            npc.knockBackResist = 0;
            npc.noGravity = true;
        }
        public override bool CheckActive() { return false; }

        int animate = 0;
        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();

            if (npc.Hitbox.Intersects(player.Hitbox) && mp.pure.Locked && animate == 0)
            {               
                animate = 500;
            }

            if (animate == 100)
            {
                mp.pure.Locked = false;
                mp.StatStaminaMaxPerm += 1;
                for (float k = 3.48f; k >= -0.4f; k -= 0.1f)
                {
                    Dust.NewDustPerfect(player.Center + new Vector2((float)Math.Cos(k) * 32, (float)Math.Sin(k) * 16 - 55), mod.DustType("Purify2"), new Vector2(0, -2), 0, default, 3f);
                }
                for (int k = 0; k <= 10; k++)
                {
                    Dust.NewDustPerfect(player.Center + new Vector2(-5 + k / 2, -k * 3 - 39), mod.DustType("Purify2"), new Vector2(0, -2), 0, default, 3f);
                    Dust.NewDustPerfect(player.Center + new Vector2(5 - k / 2, -k * 3 - 39), mod.DustType("Purify2"), new Vector2(0, -2), 0, default, 3f);
                    Dust.NewDustPerfect(player.Center + new Vector2(-25 + k / 2, -k * 1.2f - 47), mod.DustType("Purify2"), new Vector2(0, -2), 0, default, 3f);
                    Dust.NewDustPerfect(player.Center + new Vector2(25 - k / 2, -k * 1.2f - 47), mod.DustType("Purify2"), new Vector2(0, -2), 0, default, 3f);
                }
                for (int k = 0; k <= 100; k++)
                {
                    float r = Main.rand.NextFloat(0, 6.28f);
                    float r2 = Main.rand.NextFloat(3, 9);
                    Dust.NewDustPerfect(player.Center, mod.DustType("Purify2"), new Vector2((float)Math.Cos(r) * r2, (float)Math.Sin(r) * r2), 0, default, 4f);
                }
            }

            if (animate >= 1)
            {
                player.position = new Vector2(npc.position.X, npc.position.Y - 16);
                player.immune = true;
                player.immuneTime = 5;
                player.immuneNoBlink = true;
                if (animate == 1)
                {
                    player.AddBuff(BuffID.Featherfall, 120);
                    StarlightRiver.Instance.abilitytext.Display("Coronoa of Purity", "Press " + StarlightRiver.Purify.GetAssignedKeys()[0] + " to purify nearby tiles", mp.pure);
                    Helper.UnlockEntry<PureEntry>(player);
                }
            }

            if (animate > 0)
            {
                animate--;
            }

        }

        public static Texture2D wind = ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Purity1");

        float timer = 0;
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (Main.LocalPlayer == Main.player[npc.target])
            {
                //darkness
                if (animate >= 400)
                {
                    spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Overlay"), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, (100 - ((float)animate - 400)) / 100));
                    Lighting.brightness = (float)(animate - 400) / 100 ;
                }

                if (animate >= 30 && animate < 400)
                {
                    spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Overlay"), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, 0.99f));
                    Lighting.brightness = 0f;
                }

                if(animate >= 60 && animate < 400)
                {
                    Overlay.visible = false;
                    Overlay.Bootlegdust.Clear();
                }

                if (animate < 30 && animate > 0)
                {
                    spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Overlay"), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, (float)animate / 30 ));
                    Lighting.brightness = (float)(30 - animate) / 30 ;
                }

                //crown
                if (animate <= 400 && animate > 120)
                {
                    spriteBatch.Draw(wind, Vector2.Lerp(new Vector2(npc.position.X - Main.screenPosition.X, 0), (npc.position - Main.screenPosition) + new Vector2(0, -32), 1 - ((float)animate - 120) / 280), Color.White * (((float)animate - 120) / 30));
                }
            }

            AbilityHandler mp = Main.LocalPlayer.GetModPlayer<AbilityHandler>();

            timer += (float)(Math.PI * 2) / 120;
            if (timer >= Math.PI * 2)
            {
                timer = 0;
            }

            if (mp.pure.Locked && animate == 0)
            {
                spriteBatch.Draw(wind, npc.position - Main.screenPosition + new Vector2(0, (float)Math.Sin(timer) * 4), Color.White);
                Dust.NewDust(npc.position + new Vector2(0, (float)Math.Sin(timer) * 16), npc.width, npc.height, ModContent.DustType<Dusts.Purify>());

                Dust.NewDustPerfect(npc.Center + new Vector2((float)Math.Cos(timer) * 40, (float)Math.Sin(timer) * 20), ModContent.DustType<Dusts.Purify>(), null, 0, default, 2f);
                Dust.NewDustPerfect(npc.Center + new Vector2((float)Math.Cos(timer) * 40, (float)Math.Sin(timer) * 20) * -1, ModContent.DustType<Dusts.Purify>(), null, 0, default, 2f);
            }
        }
    }
}
