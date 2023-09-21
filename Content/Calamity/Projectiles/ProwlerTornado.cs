
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ProwlerTornado : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SandnadoHostile;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prowler Tornado");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 96;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.5f;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, 0f, 0f, 0, default, 1.5f);
            if (Main.player[Projectile.owner].GetModPlayer<CrossplayerCalamity>().ForceEffect(ModContent.ItemType<ProwlerEnchantment>()))
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj!= null && proj.active && proj.owner == Projectile.owner && proj.friendly && proj.damage > 0 && proj.Hitbox.Intersects(Projectile.Hitbox) && proj.type != Projectile.type)
                    {
                        NPC npc = proj.FindTargetWithinRange(500);
                        if (npc != null && npc.active)
                        {
                            proj.velocity = (npc.Center - proj.Center).SafeNormalize(Microsoft.Xna.Framework.Vector2.Zero) * proj.velocity.Length();
                        }
                    }
                }
            }
        }
    }
}