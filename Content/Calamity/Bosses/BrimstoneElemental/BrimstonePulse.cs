using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstonePulse : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/ChlorophyteLifePulse";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 50;
            Projectile.scale = 1f;
            Projectile.width = Projectile.height = 96;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, Color.Red * Projectile.Opacity, 0, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            float x = 1 - (Projectile.timeLeft / 50f);
                
            Projectile.scale = MathHelper.Lerp(0, Projectile.ai[1], 1 - (1 - x) * (1 - x));
            Projectile.Opacity = MathHelper.Lerp(1, 0, 1 - (float)Math.Cos((x * Math.PI) / 2));

            base.AI();
        }
    }
}
