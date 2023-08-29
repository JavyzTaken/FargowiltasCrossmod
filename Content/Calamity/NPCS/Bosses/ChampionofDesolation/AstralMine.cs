using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDesolation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AstralMine : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/DeusMine";
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.tileCollide = false;
            Projectile.scale = 3;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 200;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;

            base.SetStaticDefaults();
        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.Length());
            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool())
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CalamityMod.Dusts.AstralBlue>());
                else
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CalamityMod.Dusts.AstralOrange>());
                }
            }
            Projectile.velocity /= 1.02f;
            if (Projectile.timeLeft == 1)
            {
                for (int i = 0; i < 360; i++)
                {
                    Vector2 speed = new Vector2(0, Main.rand.Next(1, 10)).RotatedBy(i * 2);
                    if (Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<CalamityMod.Dusts.AstralBlue>(), speed.X, speed.Y);
                        dust.noGravity = true;
                    }
                    else
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<CalamityMod.Dusts.AstralOrange>(), speed.X, speed.Y);
                        dust.noGravity = true;
                    }

                }
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, new Vector2(0, 10).RotatedBy(MathHelper.ToRadians(i * 24)), ModContent.ProjectileType<AstralStar>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(210), 0, Main.myPlayer);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(texture.Value, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2 - Main.screenPosition, null, new Color(200, 200, 200, 100) * (1f / (i + 1)), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, new Color(200, 200, 200), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
