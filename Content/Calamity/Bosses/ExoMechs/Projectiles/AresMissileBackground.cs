using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Apollo;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers;
using FargowiltasCrossmod.Core;
using Luminance.Common.DataStructures;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AresMissileBackground : ModProjectile, IProjOwnedByBoss<Apollo>, IPixelatedPrimitiveRenderer, IExoMechProjectile
    {
        public ExoMechDamageSource DamageType => ExoMechDamageSource.Thermal;

        public override string Texture => "FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Projectiles/AresMissile";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.scale = 0.4f;
            Projectile.Opacity = 0.72f;
            Projectile.Calamity().DealsDefenseDamage = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Projectile.scale *= 0.956f;
            Projectile.Opacity *= 0.95f;
            Projectile.velocity *= 1.02f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = AresMissile.MyTexture.Value;
            Texture2D glowmask = AresMissile.Glowmask.Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(Color.DarkGray), Projectile.rotation, origin, Projectile.scale, 0, 0f);
            Main.spriteBatch.Draw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.Gray), Projectile.rotation, origin, Projectile.scale, 0, 0f);
            return false;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader trailShader = ShaderManager.GetShader("FargowiltasCrossmod.MissileFlameTrailShader");
            trailShader.Apply();

            PrimitiveSettings settings = new(c => AresMissile.FlameTrailWidthFunction(c, Projectile.scale), c => AresMissile.FlameTrailColorFunction(c, Projectile.Opacity * 0.4f),
                _ => (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 5f + Projectile.Size * 0.5f, Pixelate: true, Shader: trailShader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 14);
        }
    }
}
