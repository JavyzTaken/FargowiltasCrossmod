using CalamityMod.NPCs.CalClone;
using FargowiltasSouls;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    public class BrothersEyeFlash : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron Scrap");
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.4f;
            //Projectile.light = 1f;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 1)
            {
                Projectile.ai[1] = Main.rand.NextBool() ? 1 : -1;
                SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
            }
            Projectile.rotation += Projectile.ai[1] * MathHelper.TwoPi / 90;

            if (++Projectile.localAI[0] > 10)
            {
                Projectile.alpha += 5;
                Projectile.scale -= 0.025f;
            }
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.TypeAlive<Cataclysm>() || npc.TypeAlive<Catastrophe>())
            {
                Projectile.Center = npc.Center + Offset(npc);
                Projectile.velocity = npc.velocity;
            }
            if (Projectile.scale <= 0.05)
            {
                Projectile.Kill();
            }

        }
        public static Vector2 Offset(NPC npc) => (npc.rotation + MathHelper.PiOver2).ToRotationVector2() * 40;
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Iron, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color color = Color.Red;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Texture2D star = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/Effects/LifeStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle rect = new(0, 0, star.Width, star.Height);
            float scale = Projectile.scale * Main.rand.NextFloat(1.5f, 3f);
            Vector2 origin = new((star.Width / 2) + scale, (star.Height / 2) + scale);

            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, new Rectangle?(rect), color * Projectile.Opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            DrawData starDraw = new(star, Projectile.Center - Main.screenPosition, new Rectangle?(rect), color * Projectile.Opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            GameShaders.Misc["LCWingShader"].UseColor(color * Projectile.Opacity).UseSecondaryColor(color * Projectile.Opacity);
            GameShaders.Misc["LCWingShader"].Apply(new DrawData?());
            starDraw.Draw(Main.spriteBatch);
            Main.spriteBatch.ResetToDefault();
            return false;
        }
    }
}
