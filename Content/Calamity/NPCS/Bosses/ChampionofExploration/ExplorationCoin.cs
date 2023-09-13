using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationCoin : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/RicoshotCoin";
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 8;
            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 500;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10)
            {
                Projectile.frame++;
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }

            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin);
            }
            Projectile.rotation = MathHelper.ToRadians(Projectile.velocity.X * 2);
            if (Projectile.velocity.Y < 10)
            {
                Projectile.velocity.Y += 0.3f;
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.velocity = Vector2.Zero;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Texture2D texture = t.Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 16 * Projectile.frame, 14, 16), lightColor, Projectile.rotation, new Vector2(14, 16) / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
