using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.IO;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;


namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AuricLightning : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossLightningOrbArc;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 300;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5000;
            Projectile.extraUpdates = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 200;


        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.localAI[0] == 1)
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft = 300;
                Projectile.localAI[0] = 2;
            }
            else if (Projectile.localAI[0] == 0)
            {
                Projectile.velocity = oldVelocity;
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Distance(Projectile.oldPos[100] + new Vector2(11, 11) * Projectile.scale) < 11 * Projectile.scale) return true;
            if (targetHitbox.Distance(Projectile.oldPos[200] + new Vector2(11, 11) * Projectile.scale) < 11 * Projectile.scale) return true;
            if (targetHitbox.Distance(Projectile.oldPos[299] + new Vector2(11, 11) * Projectile.scale) < 11 * Projectile.scale) return true;
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Color color = new Color(161, 255, 236) * 0.2f;
            Color color1 = new Color(161, 255, 255) * 0.5f;
            Color color2 = new Color(255, 255, 255);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, color, 0, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, color1, 0, t.Size() / 2, Projectile.scale * 0.66f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, color2, 0, t.Size() / 2, Projectile.scale * 0.33f, SpriteEffects.None, 0);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] - Main.screenPosition + new Vector2(11, 11) * Projectile.scale, null, color, 0, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);


            }
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] - Main.screenPosition + new Vector2(11, 11) * Projectile.scale, null, color1, 0, t.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0);
            }
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] - Main.screenPosition + new Vector2(11, 11) * Projectile.scale, null, color2, 0, t.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] >= 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Thunder_0") with { Volume = 0.5f }, Projectile.Center);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
            }
            Projectile.scale = Projectile.ai[1];
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }
        public override void AI()
        {
            if (Projectile.owner == -1)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];
            if (Collision.CanHitLine(Projectile.Center, 0, 0, owner.Center, 0, 0) && Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
            }
            Projectile.localAI[1]++;
            if (Projectile.localAI[1] >= 30 && Projectile.localAI[0] != 2)
            {
                Projectile.rotation = Projectile.ai[0] + MathHelper.ToRadians(Main.rand.Next(-20, 20));
                Projectile.localAI[1] = 0;
                Projectile.velocity = new Vector2(4, 0).RotatedBy(Projectile.rotation);
            }
        }
    }
}
