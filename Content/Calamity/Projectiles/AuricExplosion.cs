
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AuricExplosion : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/GlowRing";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0;
            Projectile.timeLeft = 60;
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.Opacity = 0.75f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Color color = new Color(255, 202, 56);
            if (Projectile.ai[1] == 1) color = new Color(255, 169, 56);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, color * Projectile.Opacity, 0, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) < 60 * Projectile.scale;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.Dragonfire>(), 300);
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/PlasmaBlast"), Projectile.Center);
        }
        public override void AI()
        {
            double x = 1 - Projectile.timeLeft / 60f;
            Projectile.scale = MathHelper.Lerp(Projectile.ai[0], 0, (float)Math.Pow(1 - x, 4));
            if (Projectile.timeLeft < 30)
            {
                float o = 1 - Projectile.timeLeft / 30f;
                Projectile.Opacity = MathHelper.Lerp(0.9f, 0, o);
            }
        }
    }
}
