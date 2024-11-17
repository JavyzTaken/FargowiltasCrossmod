using CalamityMod;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles.Souls;
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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    public class FlamableGas : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/RancorFog";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.scale = 2;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1000;
            Projectile.Opacity = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.localAI[0] += 0.025f;
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            float distMult = 30 * Projectile.scale;
            for (int i = 0; i < 10; i ++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2((float)Math.Sin(Projectile.localAI[0] + i) * distMult, (float)Math.Cos(Projectile.localAI[0] + i * 2) * distMult), null, Color.SeaGreen * 0.6f * Projectile.Opacity, Projectile.localAI[0] + i, t.Size() / 2, 0.4f * Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2((float)Math.Sin(Projectile.localAI[0]+ i * 2) * distMult, (float)Math.Cos(Projectile.localAI[0] + i) * distMult), null, Color.SeaGreen * 0.6f * Projectile.Opacity, Projectile.localAI[0] - i, t.Size() / 2, 0.4f * Projectile.scale, SpriteEffects.None);
            }
            if (Projectile.ai[0] == 1)
            {
                Asset<Texture2D> exp = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Souls/CobaltExplosion");
                Main.instance.LoadProjectile(ModContent.ProjectileType<CobaltExplosion>());
                Main.EntitySpriteDraw(exp.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 120 * (int)MathHelper.Lerp(0, 4, 1 - Projectile.timeLeft / 30f), 120, 120), Color.YellowGreen, Projectile.rotation, new Vector2(exp.Width(), exp.Height() / 5) / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            
            return Projectile.ai[0] == 1;
        }
        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            if (Projectile.Opacity < 1 && Projectile.timeLeft > 30 && Projectile.ai[0] != 1)
            {
                Projectile.Opacity += 0.05f;
            }else if ((Projectile.timeLeft < 30 || Projectile.ai[0] == 1) && Projectile.Opacity > 0)
            {
                Projectile.Opacity -= 0.05f;
            }
            if (Projectile.ai[0] == 1 && Projectile.timeLeft == 9)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
            if (Projectile.timeLeft == 20 && Projectile.ai[0] == 1)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type == Projectile.type &&  Main.projectile[i].Distance(Projectile.Center) < 200 && Main.projectile[i].ai[0] == 0)
                    {
                        Main.projectile[i].ai[0] = 1;
                        Main.projectile[i].timeLeft = 30;
                    }
                }
            }

            base.AI();
        }
    }
}
