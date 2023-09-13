using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofAnnihilation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CorvidPortal : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_617";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.timeLeft = initialTimeLeft;
            Projectile.scale = 0;
        }
        public override void Kill(int timeLeft)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Color color1 = new Color(100, 100, 100);
            Color color2 = new Color(25, 25, 25);
            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, color1, -Projectile.rotation * 1.2f, t.Size() / 2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, color2, -Projectile.rotation, t.Size() / 2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, color1, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
        int initialTimeLeft = 300;
        public override void AI()
        {
            if (Projectile.timeLeft == initialTimeLeft - 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { Pitch = -2f }, Projectile.Center);
            }
            if (Projectile.timeLeft >= initialTimeLeft - 60)
            {
                float x = (initialTimeLeft - Projectile.timeLeft) / 60f;
                Projectile.scale = MathHelper.Lerp(0, Projectile.ai[0], (float)Math.Sin(x * Math.PI / 2));
                Vector2 position = Projectile.Center + new Vector2(0, Main.rand.Next(1, 100) * Projectile.scale).RotatedByRandom(MathHelper.TwoPi);
                Vector2 speed = (Projectile.Center - position) / 20;
                Dust d = Dust.NewDustDirect(position, 0, 0, DustID.BubbleBurst_Purple, Scale: Projectile.scale / 2, newColor: new Color(50, 50, 50));
                d.velocity = speed;
                d.noGravity = true;
            }
            if (Projectile.timeLeft == initialTimeLeft - 60)
            {
                Vector2 center = Projectile.Center;
                Projectile.width = 30;
                Projectile.height = 30;
                Projectile.width *= (int)Projectile.ai[0];
                Projectile.height *= (int)Projectile.ai[0];
                Projectile.Center = center;
            }
            if (Projectile.timeLeft <= 60)
            {
                float x = 1 - Projectile.timeLeft / 60f;
                Projectile.scale = MathHelper.Lerp(Projectile.ai[0], 0, 1 - (float)Math.Cos(x * Math.PI / 2));
                Vector2 position = Projectile.Center + new Vector2(0, Main.rand.Next(1, 100) * Projectile.scale).RotatedByRandom(MathHelper.TwoPi);
                Vector2 speed = (position - Projectile.Center) / 20;
                Dust d = Dust.NewDustDirect(position, 0, 0, DustID.BubbleBurst_Purple, Scale: Projectile.scale / 2, newColor: new Color(50, 50, 50));
                d.velocity = speed;
                d.noGravity = true;
            }
            if (Projectile.timeLeft == 60)
            {
                Projectile.damage = 0;
            }
            if (Projectile.timeLeft < initialTimeLeft - 60 && Projectile.timeLeft > 60)
            {
                if (Projectile.timeLeft % 3 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Main.player[Player.FindClosest(Projectile.Center, 0, 0)].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, ModContent.ProjectileType<Corvid>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer);
                }
                if (Projectile.timeLeft % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item109 with { Pitch = 0.5f }, Projectile.Center);
                }
            }
            Projectile.rotation -= MathHelper.ToRadians(2);
            if (Projectile.velocity != Vector2.Zero)
            {
                int playerindex = Player.FindClosest(Projectile.Center, 0, 0);
                if (playerindex != -1)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.player[playerindex].Center + Main.player[playerindex].velocity * 20 - Projectile.Center).SafeNormalize(Vector2.Zero) * 15, 0.03f);
                }
            }
        }
    }
}
