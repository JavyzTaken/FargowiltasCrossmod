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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    [ExtendsFromMod(Core.ModCompatibility.Calamity.Name)]
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
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return base.CanHitPlayer(target);
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            
            base.AI();
        }
    }
}
