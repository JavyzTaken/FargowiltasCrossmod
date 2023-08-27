using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDesolation
{
    [JITWhenModsEnabled("CalamityMod")]
    [ExtendsFromMod("CalamityMod")]
    public class PlagueScythe : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/SoulScythe";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.scale = 1f;
            Projectile.timeLeft = 180;
            Projectile.Opacity = 0.9f;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(offset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            offset = reader.ReadVector2();
        }
        public Vector2 offset;
        public override void OnSpawn(IEntitySource source)
        {
            offset = new Vector2(0, 200).RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360)));
            SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            Projectile.netUpdate = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.ai[1]; i++)
            {
                Vector2 origin = new Vector2(0, t.Height());
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2 - Main.screenPosition, null, new Color(255, 255, 255, 100) * (Projectile.Opacity / (i + 1f)), Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255) * Projectile.Opacity, Projectile.rotation, new Vector2(0, t.Height()), Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void AI()
        {
            Player player = Main.player[(int)Projectile.ai[0]];
            if (Projectile.timeLeft == 179)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade);
                }
            }
            if (Projectile.timeLeft > 60)
            {
                Projectile.Center = player.Center + offset;
                Projectile.rotation = Projectile.AngleTo(player.Center) - MathHelper.ToRadians(45);
            }
            else if (Projectile.timeLeft == 60)
            {
                Projectile.velocity = -offset / ((Projectile.timeLeft + 1) / 2);
                SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
            }
            if (Projectile.timeLeft <= 60)
            {
                Projectile.rotation += MathHelper.ToRadians(Projectile.timeLeft / 3);
                if (Projectile.ai[1] < 5)
                {
                    Projectile.ai[1]++;
                }
            }
            if (Projectile.timeLeft <= 30)
            {
                Projectile.Opacity -= 0.03f;
            }
        }
    }
}
