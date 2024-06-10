using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SulphurCloud : ModProjectile
    {
        
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override string Texture => "CalamityMod/Projectiles/Magic/MiasmaGas";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1;
            Projectile.Opacity = 0.8f;
            Projectile.timeLeft = 400;
            Projectile.penetrate = 4;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 60 * 4);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            //Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, 0, new Vector2(15, 15), Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            
            if (Projectile.timeLeft <= 10)
            {
                Projectile.Opacity -= 0.1f;
            }
            Projectile.velocity *= 0.98f;
            if (Main.rand.NextBool(5))
            {
                Color color = new Color(100, (int)MathHelper.Lerp(120, 255, Projectile.timeLeft / 400f) + Main.rand.Next(-50, 50), 50);
                Particle p = new TimedSmokeParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(0, 16), 0).RotatedByRandom(MathHelper.PiOver2), Projectile.velocity + new Vector2(0, Main.rand.NextFloat(-2, 0)).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)), Color.Gray, color, 1, 0.7f, 50);
                GeneralParticleHandler.SpawnParticle(p);
            }
            
        }
    }
}
