using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace FargowiltasCrossmod.Assets.Particles
{
    public class ReaverSpark : Particle
    {
        public override string AtlasTextureName => "FargowiltasSouls.SparkParticle";
        public override BlendState BlendState => BlendState.Additive;
        public int FadeTime = 0;
        public int FollowPlayer = -1;
        public int MaxTime = 0;
        public bool Minus = false;
        public Vector2 AddedVelocity = Vector2.Zero;
        public ReaverSpark(Vector2 position, Vector2 velocity, Color color, float scale, int timeLeft, int fadeTime = 10, int followPlayer = -1, bool minus = false)
        {
            Position = position;
            AddedVelocity = velocity;
            DrawColor = color;
            Scale = new(scale);
            Lifetime = timeLeft;
            FadeTime = fadeTime;
            FollowPlayer = followPlayer;
            MaxTime = timeLeft;
            Minus = minus;
            
        }
        public override void Update()
        {
            base.Update();
            float lerper = 1;
            if (Time >= MaxTime - FadeTime)
            {
                lerper = (MaxTime - Time) / (float)FadeTime;
            }else if (Time <= FadeTime)
            {
                lerper = Time / (float)FadeTime;
            }
            Opacity = lerper;
            //Opacity = MathHelper.Lerp(0, 1, -0.5f * (float)Math.Pow(Math.Asin(2 * lerper - 1), 2) + 1);
            if (FollowPlayer >= 0 && Main.player[FollowPlayer] != null && Main.player[FollowPlayer].active && !Main.player[FollowPlayer].dead)
            {
                Velocity = Main.player[FollowPlayer].velocity + AddedVelocity;
            }
            else
            {
                Velocity = AddedVelocity;
            }
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            //base.Draw(spriteBatch);
            Asset<Texture2D> t = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion];
            Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
            if (!Minus)
                spriteBatch.Draw(t.Value, Position - Main.screenPosition, null, DrawColor*Opacity, Rotation, t.Size() / 2, new Vector2(0.6f, 1.2f) * Scale, SpriteEffects.None, 0);
            
            spriteBatch.Draw(t.Value, Position - Main.screenPosition, null, DrawColor*Opacity, Rotation + MathHelper.PiOver2, t.Size() / 2, new Vector2(0.6f, 1.2f) * Scale, SpriteEffects.None, 0);
        }
    }
}
