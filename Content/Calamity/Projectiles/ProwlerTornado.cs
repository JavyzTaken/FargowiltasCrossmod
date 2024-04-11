
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ProwlerTornado : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SandnadoHostile;

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
            Projectile.extraUpdates = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.height; i += 4)
            {
                int cutoff = 30;
                float opacity = 0.9f;
                if (i > Projectile.height - cutoff) opacity = MathHelper.Lerp(0.9f, 0, (i- (Projectile.height - cutoff)) / (float)(Projectile.height - (Projectile.height - cutoff)));
                if (i < cutoff) opacity = MathHelper.Lerp(0.9f, 0, 1 - (i/(float)cutoff));
                Main.EntitySpriteDraw(t.Value, Projectile.Center + new Vector2(0, Projectile.height * 0.5f - i) - Main.screenPosition, null, new Color(255, 226, 145, 200) * opacity, Projectile.rotation + Projectile.localAI[0] + MathHelper.ToRadians(i), t.Size() / 2, MathHelper.Lerp(0.7f, 1.3f, (i / (float)Projectile.height)), SpriteEffects.None);
            }
            return false;
        }
        public override void AI()
        {
            Projectile.localAI[0] += 0.1f;
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand, 0f, 0f, 0, default, 1.5f);
            if (Main.player[Projectile.owner].ForceEffect<DesertProwlerEffect>())
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj != null && proj.active && proj.owner == Projectile.owner && proj.friendly && proj.damage > 0 && proj.Hitbox.Intersects(Projectile.Hitbox) && proj.type != Projectile.type)
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