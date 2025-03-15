using FargowiltasSouls;
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
    public class ToxicGas : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/ToxicCloud";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.scale = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1500;
            Projectile.width = Projectile.height = 30;
            Main.projFrames[Type] = 10;
            Projectile.light = 1f;
            
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 56, 40, 56), lightColor * 0.8f, Projectile.rotation, new Vector2(t.Width(), t.Height()/10)/2, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
        public override void AI()
        {
            //0: just keeps going with its velocity.
            //1: slows down over time
            //2: slows down horizontally, accelerates upward
            int aiStyle = (int)Projectile.ai[0];
            if (Projectile.timeLeft == 1500)
            {
                Projectile.ai[1] = -1;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                if (Projectile.frame >= 4 && Projectile.timeLeft > 4 * 5)
                {
                    Projectile.frame = 0;

                }
                Projectile.frameCounter = 0;
            }
            if (Projectile.timeLeft == 4 * 5)
            {
                Projectile.frame = 6;
                Projectile.frameCounter = 0;
            }



            if (aiStyle == 1)
            {
                Projectile.velocity *= 0.98f;
            }
            else if (aiStyle == 2)
            {
                Projectile.velocity.X *= 0.97f;
                if (Projectile.velocity.Y > -10)
                {
                    Projectile.velocity.Y -= 0.05f;
                }
            }

            if (Projectile.ai[1] > -1)
            {
                Projectile suck = Main.projectile[(int)Projectile.ai[1]];
                if (suck != null && suck.active)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.AngleTo(suck.Center).ToRotationVector2() * 10, 0.03f);
                    
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].Hitbox.Intersects(Projectile.Hitbox))
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }
            base.AI();
        }
    }
}
