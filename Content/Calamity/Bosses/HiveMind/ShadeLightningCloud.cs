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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ShadeLightningCloud : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/ShadeNimbusHostile";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            //Projectile.hostile = true;
            Projectile.timeLeft = 140;
            Projectile.tileCollide = false;
            Main.projFrames[Type] = 6;
            Projectile.scale = 2;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, new Rectangle(0, 28 * Projectile.frame, 54, 28), new Color(200, 200, 200, 200) * (2f / (i + 1)), Projectile.rotation, new Vector2(54, 28) / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 28 * Projectile.frame, 54, 28), lightColor, Projectile.rotation, new Vector2(54, 28) / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame == 3)
                {
                    Projectile.frame = 0;
                }
                if (Projectile.frame == 6)
                {
                    Projectile.frame = 0;
                }
            }
            Player target = Main.player[(int)Projectile.ai[0]];
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 120)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center + new Vector2(target.velocity.X * 26, -400) - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.05f);
            }

            else if (Projectile.ai[1] == 120)
            {
                Projectile.frame = 4;
                Projectile.velocity *= 0;
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 10), ModContent.ProjectileType<CursedLightning>(), Projectile.damage, 0, ai0: MathHelper.PiOver2, ai1: Main.rand.Next(10));
                }
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Thunder_0"), Projectile.Center);
            }
        }
    }
}
