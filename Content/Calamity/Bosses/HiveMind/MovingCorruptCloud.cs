using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MovingCorruptCloud : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/ShadeNimbusCloud";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 3;

        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Main.projFrames[Type] = 4;
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
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, new Rectangle(0, 28 * Projectile.frame, 28, 28), new Color(200, 200, 200, 100) * (2f / (i + 1)), Projectile.rotation, new Vector2(14, 14), Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 28 * Projectile.frame, 28, 28), lightColor, Projectile.rotation, new Vector2(14, 14), Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (DLCUtils.HostCheck)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadeNimbusHostile>(), Projectile.damage, 0);
        }
        public override void AI()
        {
            Projectile.rotation += 0.3f;
            base.AI();
        }
    }
}
