using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MushroomSpear : ModProjectile
    {

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {

            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.height = 22;
            Projectile.width = 22;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1;
            Projectile.scale = 1.5f;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> head = TextureAssets.Projectile[Type];
            Asset<Texture2D> stick = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/Crabulon/MushroomSpearStick");

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            if (Projectile.Opacity > 0.75f)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f * Projectile.scale;
                    Color glowColor = Color.Cyan * 0.9f;

                    /*
                    for (int i = 0; i < Projectile.Center.Distance(new Vector2(Projectile.Center.X, Projectile.ai[2])) / 12 + 1; i++)
                    {
                        Main.EntitySpriteDraw(stick.Value, Projectile.Center + afterimageOffset - Main.screenPosition + new Vector2(0, i * 12), null, glowColor * Projectile.Opacity, Projectile.rotation, stick.Size() / 2, Projectile.scale, SpriteEffects.None);
                    }
                    */
                    Main.EntitySpriteDraw(head.Value, Projectile.Center + afterimageOffset - Main.screenPosition, null, glowColor * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, head.Size() / 2, Projectile.scale, SpriteEffects.None);
                }
            }
            Main.spriteBatch.ResetToDefault();
            for (int i = 0; i < Projectile.Center.Distance(new Vector2(Projectile.Center.X, Projectile.ai[2])) / 12 + 1; i++)
            {
                Main.EntitySpriteDraw(stick.Value, Projectile.Center - Main.screenPosition + new Vector2(0, i * 12), null, lightColor * Projectile.Opacity, Projectile.rotation, stick.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(head.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, head.Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[1] < 60)
            {
                return false;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MushroomSpray, Alpha: 120, Scale: 2).noGravity = true;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MushroomSpray, Alpha: 120, Scale: 2, SpeedY: Main.rand.Next(-10, 0)).noGravity = true;
            }
            Projectile.Center = new Vector2(Projectile.Center.X, FindGround(Projectile.Center));
            Projectile.ai[2] = Projectile.Center.Y;
            Projectile.Opacity = 0;
        }
        public override void AI()
        {
            if (Projectile.Opacity < 1)
                Projectile.Opacity += 0.05f;
            int vel = WorldSavingSystem.MasochistModeReal ? 40 : 25;
            Projectile.ai[1]++;
            if (Projectile.ai[1] == 60)
            {
                Projectile.velocity.Y = -vel;
                SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot with { Pitch = 0.5f }, Projectile.Center);
            }
            if (Projectile.ai[1] == 70)
            {
                Projectile.velocity.Y = 0;

            }
            if (Projectile.ai[1] == 90) Projectile.velocity.Y = vel;
            if (Projectile.ai[1] == 100) Projectile.Kill();
            int delay = WorldSavingSystem.MasochistModeReal ? 12 : 20;
            if (Main.netMode != NetmodeID.MultiplayerClient && Projectile.ai[1] == delay)
            {
                if (Projectile.ai[0] < 0)
                {
                    Vector2 pos = new Vector2(Projectile.Center.X - 150, Projectile.ai[2]);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, Type, Projectile.damage, 0, ai0: Projectile.ai[0] + 1);
                }
                else if (Projectile.ai[0] > 0)
                {
                    Vector2 pos = new Vector2(Projectile.Center.X + 150, Projectile.ai[2]);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, Type, Projectile.damage, 0, ai0: Projectile.ai[0] - 1);
                }
            }
        }
        public int FindGround(Vector2 pos)
        {
            int escape = 0;
            while (Collision.SolidTiles(pos, 2, 10) && escape < 500)
            {
                escape++;
                pos.Y--;
            }
            while (!Collision.SolidTiles(pos, 2, 10) && escape < 500)
            {
                escape++;
                pos.Y++;
            }
            return (int)pos.Y;
        }
    }
}
