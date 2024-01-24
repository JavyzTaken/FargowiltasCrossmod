using System.Collections.Generic;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class IceTrident : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Magic/IcicleTrident";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 300;

            Projectile.tileCollide = false;
            Projectile.light = 0.5f;
            //Projectile.coldDamage = true;
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
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2;
                float scale = MathHelper.Lerp(Projectile.scale, Projectile.scale * 0.4f, i / (float)Projectile.oldPos.Length);
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    Color glowColor = Color.Blue with { A = 0 } * 0.9f;


                    Main.EntitySpriteDraw(t.Value, pos + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, t.Size() / 2, scale, SpriteEffects.None);
                }
                Main.EntitySpriteDraw(t.Value, pos - Main.screenPosition, null, new Color(100, 100, 250, 10), Projectile.rotation, t.Size() / 2, scale, SpriteEffects.None);
            }
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                Color glowColor = Color.Blue with { A = 0 } * 0.9f;


                Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, new Color(100, 100, 250, 10), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            //Projectile.DrawProjectileWithBackglow(Color.LightBlue, new Color(100, 100, 250, 20), 4f);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void AI()
        {
            Projectile.velocity *= 1.005f;

            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = 1;
                //Projectile.velocity = new Vector2(0, -Projectile.ai[0]).RotatedBy(Projectile.ai[2]);
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] == 50)
            {
                SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
                Projectile.velocity = new Vector2(18, 0).RotatedBy(Projectile.ai[2]);
            }
            if (Projectile.ai[1] < 50)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.03f);
            if (Projectile.ai[1] > 100 && DLCUtils.HostCheck && Projectile.ai[1] % 10 == 0)
            {
                //Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 1), ModContent.ProjectileType<IceRain>(), Projectile.damage, 0, ai0: 1);
            }
            Projectile.rotation = Projectile.ai[2] + MathHelper.PiOver4;
            base.AI();
        }
    }
}
