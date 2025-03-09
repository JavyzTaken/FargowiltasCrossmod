using FargowiltasSouls.Content.Items.Accessories.Souls;
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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    public class Tooth : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/SandTooth";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.scale = 1.3f;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 200;
            Projectile.light = 1;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 200) {
                Projectile.ai[0] = Projectile.velocity.Length();
            }
            if (Projectile.timeLeft > 180)
            {
                Projectile.velocity *= 0.93f;
            }
            else
            {
                if (Projectile.velocity.Length() < Projectile.ai[0])
                {
                    Projectile.velocity *= 1.05f;
                }
            }
            if (Projectile.ai[1] == 1)
            {
                Player target = Main.player[(int)Projectile.ai[2]];
                
                if (Projectile.timeLeft < 190 && Projectile.timeLeft > 150 && target != null && target.active)
                {
                    float angleDiff = MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(Projectile.velocity, Projectile.AngleTo(target.Center).ToRotationVector2()));
                    //turning to face the player
                    if (Math.Abs(angleDiff) > 0.5f)
                    {
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(angleDiff > 0 ? 5 : -5));

                    }
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            
            base.AI();
        }
    }
}
