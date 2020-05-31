﻿using Microsoft.Xna.Framework;
using StarlightRiver.Projectiles.WeaponProjectiles.Summons;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace StarlightRiver.Items.Armor.Ebony
{
    [AutoloadEquip(EquipType.Head)]
    public class EbonyHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebony Mask");
            Tooltip.SetDefault("3% increased minion damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 10000;
            item.rare = 2;
            item.defense = 1;
        }
        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.03f;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class EbonyChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebony Breastplate");
            Tooltip.SetDefault("3% increased minion damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 1;
            item.rare = 2;
            item.defense = 1;
        }
        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.03f;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<EbonyHead>() && legs.type == ModContent.ItemType<EbonyLegs>();
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "summon tagged enemies release bolts of dark energy at a random nearby enemy when taking summon damage for 20% of the hits damage";
            EbonyPlayer ebonyPlayer = player.GetModPlayer<EbonyPlayer>();
            player.maxMinions++;
            ebonyPlayer.HasEbonyArmor = true;
        }
    }
    public class EbonyPlayer : ModPlayer
    {
        public bool HasEbonyArmor = false;
        public override void ResetEffects()
        {
            HasEbonyArmor = false;
            base.ResetEffects();
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (HasEbonyArmor)
            {
                if (proj.minion)
                {
                    if (target == proj.OwnerMinionAttackTargetNPC)
                    {
                        for (int k = 0; k < Main.npc.Length; k++)
                        {
                            if (Main.npc[k] != target)
                            {
                                if (Helper.IsTargetValid(Main.npc[k]))
                                {
                                    if (Vector2.Distance(Main.npc[k].Center, proj.Center) <= 124)
                                    {
                                        target.StrikeNPC(damage / 2, knockback / 2, proj.direction, crit);
                                    }

                                }
                            }
                        }
                        proj.MakeGlow(target.Center, ModContent.GetTexture("StarlightRiver/VFX/Glow0"), Color.Red, 40, 2f, true);
                    }
                }
            }
            base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class EbonyLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebony Footgear");
            Tooltip.SetDefault("3% minion damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 1;
            item.rare = 2;
            item.defense = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.03f;
        }
    }
}