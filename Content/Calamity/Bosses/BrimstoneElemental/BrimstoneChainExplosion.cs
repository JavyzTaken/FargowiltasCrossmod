using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
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
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneChainExplosion : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.SolarWhipSwordExplosion;
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 5;
            Projectile.height = Projectile.width = 40;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.LifeDrain, Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5), Scale: 2).noGravity = true;
            }
            if (Projectile.ai[0] == 0 && Projectile.ai[1] == 0)
            {
                Vector2 targetPos = Projectile.Center + new Vector2(Main.rand.NextFloat(400, 600), 0 ).RotatedByRandom(MathHelper.TwoPi);
                Projectile.ai[0] = targetPos.X;
                Projectile.ai[1] = targetPos.Y;
            }
            base.OnSpawn(source);
        }
        public override void OnKill(int timeLeft)
        {
            
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            DLCUtils.DrawBackglow(t, Color.Red, Projectile.Center, new Vector2(52, 52) / 2, sourceRectangle: new Rectangle(0, 52 * Projectile.frame, 52, 52), offsetMult: 3);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 52 * Projectile.frame, 52, 52), new Color(255, 0, 0) * 0.7f, Projectile.rotation, new Vector2(52, 52) / 2, Projectile.scale, SpriteEffects.None);
            
            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Red);
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
            }
            if (Projectile.timeLeft == 10)
            {
                Vector2 targetPos = new Vector2(Projectile.ai[0], Projectile.ai[1]);

                if (Projectile.Distance(targetPos) > 40)
                {

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.DirectionTo(targetPos).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-50, 50))).SafeNormalize(Vector2.Zero) * 40, Vector2.Zero, Type, Projectile.damage, 0, ai0: Projectile.ai[0], ai1: Projectile.ai[1]);
                }
            }
            base.AI();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        
    }
}
