using Microsoft.Xna.Framework;
using StarlightRiver.Projectiles.WeaponProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.Items.Misc
{
    public class ThrowawayJoke : ModItem
    {
        public override void SetDefaults()
        {
            item.useStyle = 5;
            item.useAnimation = 10;
            item.useTime = 10;
            item.crit = 20;
            item.knockBack = 1f;
            item.width = 40;
            item.height = 26;
            item.damage = 6;
            item.rare = 3;
            item.value = Item.sellPrice(0, 45, 0, 0);
            item.noMelee = true;
            item.autoReuse = true;
            item.ranged = true;
            item.shoot = 10;
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Bullet;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Throwaway Joke");
            Tooltip.SetDefault("This Leather gripped sub machine gun can hold 30 rounds in its magazine\nWith an empty magazine throw the weapon at an enemy to instantly reload it");
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override float UseTimeMultiplier(Player player)
        {
            SmgPlayer sPlayer = player.GetModPlayer<SmgPlayer>();
            return 1f + ((sPlayer.crit ? 60 : 30) - sPlayer.ammo) / 30f;
        }
        public override bool CanUseItem(Player player)
        {
            SmgPlayer sPlayer = player.GetModPlayer<SmgPlayer>();
            if (sPlayer.ammo <= 1) //one less cause this is dumb
            {
                item.autoReuse = false;
                item.noUseGraphic = true;
            }
            else
            {
                item.autoReuse = true;
                item.noUseGraphic = false;
            }
            return sPlayer.ammo > 0;
        }
        public override bool ConsumeAmmo(Player player)
        {
            SmgPlayer sPlayer = player.GetModPlayer<SmgPlayer>();
            return sPlayer.ammo <= 0;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            SmgPlayer sPlayer = player.GetModPlayer<SmgPlayer>();
            sPlayer.ammo--;
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            if (sPlayer.ammo <= 0)
            {
                Main.PlaySound(2, -1, -1, 1, 1);
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<ThrowawayJokeProjectile>(), damage * 4, knockBack, player.whoAmI);
                return false;
            }
            else
            {
                Main.PlaySound(2, -1, -1, 11, 1);
            }
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(6));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
    }
    public class SmgPlayer : ModPlayer
    {
        public int ammo = 30;
        public int cooldown = 120;
        public bool crit = false;
        public override void UpdateLifeRegen()
        {
            if (ammo <= 0)
            {
                cooldown--;
                if (cooldown <= 0)
                {
                    reload(false);
                }
            }
            base.UpdateLifeRegen();
        }
        public void reload(bool crit)
        {
            cooldown = 120;
            this.crit = crit;
            ammo = crit ? 60 : 30;
            Main.PlaySound(2, -1, -1, 99, 1);
            Main.PlaySound(2, -1, -1, 98, 1);
        }
    }
}
