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
    public class Blizzard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.HallowBossDeathAurora;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 60;
            
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.coldDamage = true;
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
            SpriteEffects se = SpriteEffects.None;
            if (Projectile.ai[0] == 1) se = SpriteEffects.FlipHorizontally;
            if (Projectile.ai[0] == 2) se = SpriteEffects.FlipVertically;
            if (Projectile.ai[0] == 3) se = SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
            //for (int i = 0; i < 60; i++)
            //{
            //    SpriteEffects se = SpriteEffects.None;
            //    if (i % 5 == 0) se = SpriteEffects.FlipHorizontally;
            //    if (i % 7 == 0) se = SpriteEffects.FlipVertically;
            //    Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2((float)Math.Cos(i)*200, (float)Math.Sin(Projectile.ai[1] + i / 10f) * 1000).RotatedBy(Projectile.rotation), null, new Color(200, 200, 200, 50) * Projectile.Opacity, Projectile.rotation, t.Size() / 2, Projectile.scale, se);
            //}
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, new Color(200, 200, 200, 50) * Projectile.Opacity * 0.3f, Projectile.rotation, t.Size() / 2, Projectile.scale, se);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            //Projectile.velocity /= 1.02f;
            if (Projectile.timeLeft < 60) Projectile.Opacity -= 1 / 30f;
            //Projectile.ai[1] += 0.01f;
            base.AI();
        }
    }
}
