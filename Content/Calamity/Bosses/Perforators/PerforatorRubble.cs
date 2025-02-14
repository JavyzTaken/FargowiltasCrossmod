using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    public class PerforatorRubble : ModProjectile, ILocalizedModType
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;
        public override void AI()
        {
            if (Projectile.velocity.Y > 0)
                Projectile.tileCollide = true;

            Projectile.rotation += (Projectile.velocity.X / 120f + Projectile.velocity.X.NonZeroSign() * 0.04f) * MathHelper.PiOver2;

            int bloody = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 0.5f);
            Main.dust[bloody].noGravity = true;
            Main.dust[bloody].velocity *= 0f;

            if (Projectile.velocity.Y < 12f)
                Projectile.velocity.Y += 0.09f;

            Projectile.velocity.X *= 0.995f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BurningBlood>(), 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[2] == 0)
            {
                Projectile.localAI[2] = 1;
                Projectile.frame = Main.rand.Next(3);
            }
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);

            Texture2D t = TextureAssets.Projectile[Type].Value;
            int width = t.Width / 3;
            Rectangle frame = new(width * Projectile.frame, 0, width, t.Height);


            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
