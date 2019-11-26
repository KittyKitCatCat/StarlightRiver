using Microsoft.Xna.Framework;
using StarlightRiver.Projectiles.WeaponProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Items;
using WebmilioCommons.Items.Standard;

namespace StarlightRiver.Items
{
    public sealed class DiverStaff : StandardItem
    {
        public DiverStaff() : base("Splitstream", "Shoots a projectile that ascends in blocks", 38, 34, rarity: ItemRarityID.Green)
        {
        }


        public override void SetDefaults()
        {
            base.SetDefaults();

            item.useStyle = ItemUseStyleID.HoldingOut;
            Item.staff[item.type] = true;

            item.useTime = 40;
            item.useAnimation = item.useTime;

            item.shootSpeed = 3f;
            item.knockBack = 2f;
            item.damage = 18;

            item.UseSound = SoundID.Item43;
            item.shoot = ModContent.ProjectileType<DiverProjectile>();

            item.noMelee = true;
            item.magic = true;
            item.autoReuse = true;
        }


        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(.75f);
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }
    }
}
