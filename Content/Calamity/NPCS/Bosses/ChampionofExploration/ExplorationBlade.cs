using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationBlade : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.GladiusStab;
        public override void SetDefaults()
        {
            Projectile.timeLeft = 800;
            Projectile.scale = 2;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.friendly = false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[ProjectileID.GladiusStab];
            Texture2D texture = TextureAssets.Projectile[ProjectileID.GladiusStab].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center + new Vector2(0, 0) - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[1]];
            if ((Projectile.timeLeft + Projectile.ai[0]) % 60 == 0)
            {
                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 30;
            }
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.02f);
            Projectile.rotation = Projectile.velocity.Length();
        }
    }
}
