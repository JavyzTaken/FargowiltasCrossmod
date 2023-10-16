using CalamityMod;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
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
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CrystalShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Tiles_" + TileID.Crystals;
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 300;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Projectile.scale = 2;
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
            
            Rectangle source = new Rectangle(0, 0, 16, 16);
            if (Projectile.ai[0] == 1) source.X = 108;
            if (Projectile.ai[0] == 2) source.X = 162;
            if (Projectile.ai[0] == 3) source.X = 216;
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f;
                Color glowColor = Color.Blue with { A = 0 } * 0.9f;


                Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, source, glowColor, Projectile.rotation, new Vector2(8, 8), Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, source, lightColor, Projectile.rotation, new Vector2(8, 8), Projectile.scale, SpriteEffects.None);
            //Projectile.DrawProjectileWithBackglow(Color.LightBlue, new Color(100, 100, 250, 20), 4f);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() / 10;
            Projectile.velocity /= 1.02f;
            base.AI();
        }
    }
}
