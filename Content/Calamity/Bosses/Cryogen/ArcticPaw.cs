using CalamityMod;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ArcticPaw : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/ArcticBearPawProj";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 200;
            Projectile.light = 0.5f;
            Projectile.scale = 0.7f;

            Projectile.tileCollide = false;
            //Projectile.coldDamage = true;

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f * Projectile.scale;
                Color glowColor = Color.Blue with { A = 0 } * 0.9f;


                Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * 0.7f, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            //Projectile.DrawProjectileWithBackglow(Color.LightBlue, new Color(100, 100, 250, 20), 4f);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            //ease in bounce out
            if (Projectile.ai[1] < 2) Projectile.ai[1] += 0.02f;
            if (Projectile.ai[1] > 1)
            {
                float x = Projectile.ai[1] - 1;
                float lerper;
                float n1 = 7.5625f;
                float d1 = 2.75f;

                if (x < 1 / d1)
                {
                    lerper = n1 * x * x;
                }
                else if (x < 2 / d1)
                {
                    lerper = n1 * (x -= 1.5f / d1) * x + 0.75f;
                }
                else if (x < 2.5 / d1)
                {
                    lerper = n1 * (x -= 2.25f / d1) * x + 0.9375f;
                }
                else
                {
                    lerper = n1 * (x -= 2.625f / d1) * x + 0.984375f;
                }
                Projectile.scale = MathHelper.Lerp(0.7f, 2.4f, lerper);
                Projectile.Resize((int)(30 * Projectile.scale),(int)( 30 * Projectile.scale));
            }
            
            if (Projectile.ai[1] == 1.4199992f)
            {
                SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, Scale: 2f);
                    d.noGravity = true;
                    d.velocity *= 3;
                }
            }
            if (Projectile.timeLeft == 1)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, Scale:1.5f).noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
            }
            Projectile.velocity *= 0.98f;
            base.AI();
        }
    }
}
