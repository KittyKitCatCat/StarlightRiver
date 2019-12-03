using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using StarlightRiver.Abilities;
using StarlightRiver.Codex.Entries;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.NPCs.Pickups
{
    class Wisp : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Faeflame");
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
        public override bool CheckActive() { return true; }

        int animate = 0;
        float rot = 0;
        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();

            if (npc.Hitbox.Intersects(player.Hitbox) && mp.AbilityWisp.Locked)
            {
                mp.AbilityWisp.Locked = false;
                mp.StatStaminaMaxPerm += 1;
                animate = 300;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Pickups/get"));
                rot = (float)(Math.PI * 2);
            }

            if (animate >= 1)
            {
                player.position = new Vector2(npc.position.X, npc.position.Y - 16);
                player.immune = true;
                player.immuneTime = 5;
                player.immuneNoBlink = true;
                if (animate > 100 && animate < 290)
                {
                    if (animate % 10 == 0)
                    {                     
                        Dust dus = Dust.NewDustPerfect(player.Center, mod.DustType("Gold3"), new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * 10);
                        dus.customData = animate - 50;
                        rot -= (float)(Math.PI * 2) / 18;
                    }
                }
                if (animate == 1)
                {
                    player.AddBuff(BuffID.Featherfall, 120);
                    Achievements.Achievements.QuickGive("Faerie Blaze", player);

                    StarlightRiver.Instance.abilitytext.Display(mp.AbilityWisp, "Faeflame", "Hold " + StarlightRiver.Wisp.GetAssignedKeys()[0] + " to shrink and fly through the air");
                    Helper.UnlockEntry<FaeEntry>(player);
                }

                for (int k = 0; k <= 6000; k++)
                {
                    if (Main.dust[k].type == mod.DustType("Gold3"))
                    { 
                        Dust.NewDustPerfect(Main.dust[k].position, mod.DustType("Gold"),null,0,default, 0.5f);
                    }
                }
            }

            if (animate > 0)
            {
                animate--;
            }

        }

        public static Texture2D wind = ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Wisp1");

        float timer = 0;
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            AbilityHandler mp = Main.LocalPlayer.GetModPlayer<AbilityHandler>();

            timer += (float)(Math.PI * 2) / 120;
            if (timer >= Math.PI * 2)
            {
                timer = 0;
            }

            if (mp.AbilityWisp.Locked)
            {
                spriteBatch.Draw(wind, npc.position - Main.screenPosition + new Vector2(0, (float)Math.Sin(timer) * 4), Color.White);
                Dust.NewDust(npc.position + new Vector2(0, (float)Math.Sin(timer) * 16), npc.width, npc.height, mod.DustType("Gold2"), 0, 0, 0, default, 0.5f);
            }
        }
        public override void DrawEffects(ref Color drawColor)
        {
            AbilityHandler mp = Main.LocalPlayer.GetModPlayer<AbilityHandler>();
            if (mp.AbilityWisp.Locked)
            {
                Dust.NewDustPerfect(npc.Center + new Vector2((float)Math.Cos(timer), (float)Math.Sin(timer)) * 32, ModContent.DustType<Dusts.Gold>(),null,0,default,0.4f);
                Dust.NewDustPerfect(npc.Center + new Vector2((float)Math.Cos(timer + 3) / 2, (float)Math.Sin(timer + 3)) * 32, ModContent.DustType<Dusts.Gold>(), null, 0, default, 0.4f);
                Dust.NewDustPerfect(npc.Center + new Vector2((float)Math.Cos(timer + 2), (float)Math.Sin(timer + 2) / 2) * 32, ModContent.DustType<Dusts.Gold>(), null, 0, default, 0.4f);
            }
        }
    }
}
