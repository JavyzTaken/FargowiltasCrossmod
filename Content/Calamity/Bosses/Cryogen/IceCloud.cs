
//using FargowiltasSouls.Common.Graphics.Particles;
using CalamityMod;
using CalamityMod.Particles;
using FargowiltasCrossmod.Core;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class IceCloud : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Empty";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 60 * 4;
            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
        }
        private static readonly Color bloomColor = new Color(75, 177, 250);
        public override void AI()
        {
            ref float spinDir = ref Projectile.localAI[0];
            if (spinDir == 0)
                spinDir = Main.rand.NextBool() ? 1 : -1;
            Projectile.rotation += 0.1f * spinDir; 

            //don't make particles offscreen, to save on particle limit.
            //distance to 1920x1080 screen corner is abt 1100
            int pIndex = Player.FindClosest(Projectile.Center, 0, 0);
            if (!(pIndex.WithinBounds(Main.maxPlayers))) 
                return;

            float distance = Main.player[pIndex].Distance(Projectile.Center);

            if (distance > 2400f)
                Projectile.Kill();

            if (distance > 1200f)
                return;

            if (Projectile.timeLeft % 40 == 0)
            {
                //Color color = Projectile.ai[0] > 0 ? Color.GhostWhite : Color.Blue;
                //Color color = Color.Lerp(Color.Cyan, Color.LightBlue, 0.4f);
                //Particle smoke = new FogPuff(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, smokeColor, 0.5f, 30, 0.8f, Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(-0.2f, 0.2f));
                //Particle smoke = new ExpandingBloomParticle(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Vector2.Zero, Color.Cyan, Vector2.One, Vector2.Zero, 30, true, Color.LightBlue);
                //smoke.Spawn();

                Particle snowflake = new SnowflakeSparkle(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Projectile.velocity + Main.rand.NextVector2Circular(2, 2), Color.White, bloomColor, Main.rand.NextFloat(0.3f, 1.5f), 60, 0.5f);
                GeneralParticleHandler.SpawnParticle(snowflake);

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.SetBlendState(BlendState.Additive);

            const int Spokes = 3;
            Vector2 Position = Projectile.Center;
            float Rotation = Projectile.rotation;
            float opacity = Projectile.Opacity * 0.5f;
            float Scale = Projectile.scale;
            float BloomScale = Scale * 0.5f;
            Color Color = Color.White;
            Color Bloom = bloomColor;

            Texture2D spokesTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/HalfIceStar").Value;
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            // Ajust the bloom's texture to be the same size as the star's.
            float properBloomSize = (float)spokesTexture.Height / (float)bloomTexture.Height;
            float halvedOpacity = opacity * 1f;

            Main.EntitySpriteDraw(bloomTexture, Position - Main.screenPosition, null, Bloom * halvedOpacity, 0, bloomTexture.Size() / 2f, Scale * BloomScale * properBloomSize, SpriteEffects.None, 0);

            Color spokeColor = Color * halvedOpacity;
            Vector2 origin = spokesTexture.Size() / 2f;
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < Spokes; i++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 3f).ToRotationVector2() * 7f * Projectile.scale;
                    Color glowColor = Color.Blue * 0.9f * halvedOpacity * 1.2f;

                    float rotation = Rotation + MathHelper.Lerp(0f, MathHelper.Pi, i / (float)Spokes);
                    Main.EntitySpriteDraw(spokesTexture, Position + afterimageOffset - Main.screenPosition, null, glowColor, Rotation + rotation, origin, Scale, SpriteEffects.None, 0);
                }
            }
            for (int i = 0; i < Spokes; i++)
            {
                float rotation = Rotation + MathHelper.Lerp(0f, MathHelper.Pi, i / (float)Spokes);
                Main.EntitySpriteDraw(spokesTexture, Position - Main.screenPosition, null, spokeColor, Rotation + rotation, origin, Scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ResetToDefault();
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Chilled, 60 * 5);
        }
    }
}
