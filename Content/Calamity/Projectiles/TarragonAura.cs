using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria;
using Terraria.ID;


namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class TarragonAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Healing/SilvaOrb";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.height = 22;
            Projectile.width = 22;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.scale = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) < 22 * Projectile.scale;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < 20; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2(0, 22 * Projectile.scale).RotatedBy(MathHelper.ToRadians(360f / 20f * i) + Projectile.rotation), null, lightColor * 0.75f, 0, t.Size() / 2, Projectile.scale / 5, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(3);
            if (Projectile.scale < 10 && Projectile.timeLeft > 50) Projectile.scale += 0.2f;
            else if (Projectile.timeLeft <= 50) Projectile.scale -= 0.2f;
            if (Projectile.owner < 0)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center;
            Lighting.AddLight(Projectile.Center, new Vector3(0, 0.5f * Projectile.scale, 0));
            for (int j3 = 0; j3 < 2; j3++)
            {
                Vector2 pos = Projectile.Center + new Vector2(0, Main.rand.Next(1, 22) * Projectile.scale).RotatedByRandom(MathHelper.TwoPi);
                Dust green = Dust.NewDustDirect(new Vector2(pos.X, pos.Y), Projectile.width, Projectile.height, DustID.ChlorophyteWeapon, (int)owner.velocity.X, (int)owner.velocity.Y, 100, new Color(Main.DiscoR, 203, 103), 2f);
                Dust green2 = green;
                green2.position.X = green2.position.X + Main.rand.Next(-20, 21);
                Dust green3 = green;
                green3.position.Y = green3.position.Y + Main.rand.Next(-20, 21);
                green.velocity *= 0.9f;
                green.noGravity = true;
                green.scale *= 1f + Main.rand.Next(40) * 0.01f;

                if (Main.rand.NextBool(2))
                {
                    green.scale *= 1f + Main.rand.Next(40) * 0.01f;
                }
            }
        }
    }
}
