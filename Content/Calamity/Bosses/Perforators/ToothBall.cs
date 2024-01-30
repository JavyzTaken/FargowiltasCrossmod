using System;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ToothBall : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ToothBall";
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.timeLeft = 250;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];

            //draw glow
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                Color glowColor = Color.Red with { A = 0 } * 0.7f;


                Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
            if (DLCUtils.HostCheck)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, new Vector2(2, -1), ModContent.ProjectileType<BloodGeyser>(), Projectile.damage, 0);
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, new Vector2(-2, -1), ModContent.ProjectileType<BloodGeyser>(), Projectile.damage, 0);
                Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, new Vector2(0, -4), ModContent.ProjectileType<BloodGeyser>(), Projectile.damage, 0);
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.X * 3 + Math.Abs(Projectile.velocity.Y));
            Projectile.velocity.Y += 0.15f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.9f;
            return false;
        }
    }
}
