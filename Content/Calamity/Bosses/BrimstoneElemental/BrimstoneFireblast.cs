using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneFireblast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/SCalBrimstoneFireblast";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 5;
            Projectile.height = Projectile.width = 28;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
            Projectile.light = 1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.WeakBrimstoneFlames>(), 150);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            BEGlobalProjectile.DrawBlackBorder(t, Projectile.Center, new Vector2(36, 50) / 2, Projectile.rotation, Projectile.scale, SpriteEffects.None, offsetMult: 3, sourceRectangle: new Rectangle(0, 50 * Projectile.frame, 36, 50));
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 50 * Projectile.frame, 36, 50), lightColor, Projectile.rotation, new Vector2(36, 50) / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 120)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                }
                if (Projectile.ai[0] > 1)
                Projectile.timeLeft = (int)Projectile.ai[0];
            }
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                if (Projectile.frame > 4)
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft == 1)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                }
                int numDarts = 14;
                if (DLCUtils.HostCheck)
                {
                    for (int i = 0; i < numDarts; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 4).RotatedBy(MathHelper.TwoPi / numDarts * i), ModContent.ProjectileType<BrimstoneBarrage>(), Projectile.damage, Projectile.knockBack);
                    }
                }
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneFireblastImpact"));
            }
        }
    }
}
