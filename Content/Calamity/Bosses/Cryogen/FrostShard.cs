using CalamityMod;
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
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class FrostShard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FrostShard;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 400;

            Projectile.light = 0.5f;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(0, 5);
            base.OnSpawn(source);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            if (Projectile.localAI[0] >= 5) lightColor.A += 50;

            Projectile.DrawProjectileWithBackglow(CalamityMod.NPCs.Cryogen.Cryogen.BackglowColor, lightColor, 4f);
            /*

            Rectangle rect = new Rectangle(0, 30 * Projectile.frame, 12, 30);
            //draw glow
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 0.5f;
                Color glowColor = CalamityMod.NPCs.Cryogen.Cryogen.BackglowColor with { A = 0 } * 0.7f;


                Main.EntitySpriteDraw(t.Value, Projectile.Center + afterimageOffset - Main.screenPosition, rect, glowColor, Projectile.rotation, new Vector2(12, 30) / 2, Projectile.scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, new Vector2(12, 30) / 2, Projectile.scale, SpriteEffects.None);
            */
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 10) Projectile.localAI[0] = 0;
            base.AI();
        }
    }
}
