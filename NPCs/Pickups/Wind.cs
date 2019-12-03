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
    class Wind : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Winds");
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
        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();

            if (npc.Hitbox.Intersects(player.Hitbox) && mp.AbilityDash.Locked)
            {
                mp.AbilityDash.Locked = false;
                mp.StatStaminaMaxPerm += 1;
                animate = 300;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Pickups/get"));
            }

            if (animate >= 1)
            {
                player.position = new Vector2(npc.position.X, npc.position.Y - 16);
                player.immune = true;
                player.immuneTime = 5;
                player.immuneNoBlink = true;
                if (animate > 100 && animate < 290)
                {
                    float rot = Main.rand.NextFloat(0, (float)Math.PI * 2);
                    Dust dus = Dust.NewDustPerfect(player.Center, mod.DustType("Air2"), new Vector2((float)Math.Cos(rot) * 5, (float)Math.Sin(rot) * 5));
                    dus.customData = animate - 50;
                }
                if(animate == 1)
                {
                    player.AddBuff(BuffID.Featherfall, 120);
                    Achievements.Achievements.QuickGive("Stormcaller", player);

                    StarlightRiver.Instance.abilitytext.Display(mp.AbilityDash, "Forbidden Winds", "Press " + StarlightRiver.Dash.GetAssignedKeys()[0] + " + A/W/S/D to dash");
                    Helper.UnlockEntry<WindsEntry>(player);
                }
            }

            if(animate > 0)
            {
                animate--;
            }
            
        }
        float timer = 0;
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            AbilityHandler mp = Main.LocalPlayer.GetModPlayer<AbilityHandler>();

            timer += (float)(Math.PI * 2) / 120;
            if(timer >= Math.PI * 2)
            {
                timer = 0;
            }

            if (mp.AbilityDash.Locked)
            {
                Vector2 pos = npc.position - Main.screenPosition - (new Vector2((int)((Math.Cos(timer * 3) + 1) * 4f), (int)((Math.Sin(timer * 3) + 1) * 4f)) / 2) + new Vector2(0, (float)Math.Sin(timer) * 4);

                spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Wind1"), npc.position + new Vector2(0, (float)Math.Sin(timer) * 4) - Main.screenPosition, Color.White);

                Dust.NewDustPerfect(new Vector2(npc.Center.X + (float)Math.Sin(timer) * 30, npc.Center.Y - 20), ModContent.DustType<Dusts.Air>(), Vector2.Zero);
                Dust.NewDustPerfect(new Vector2(npc.Center.X + (float)Math.Cos(timer) * 15, npc.Center.Y), ModContent.DustType<Dusts.Air>(), Vector2.Zero);

                if (Main.rand.Next(5) == 0) { Dust.NewDust(npc.Center, 1, 1, ModContent.DustType<Dusts.Air>()); }
            }           
        }
    }
}
