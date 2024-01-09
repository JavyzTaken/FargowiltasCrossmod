using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GTBSandStorm : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 1500;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 0.1f;
        }

        public override void AI()
        {
            Projectile.position -= Projectile.velocity;
            Vector2 temp = Projectile.Center;
            Projectile.width = (int)(64f * Projectile.scale); 
            Projectile.height = (int)(440 * MathF.Sqrt(Projectile.scale));
            Projectile.Center = temp;

            if (Projectile.scale >= 3f)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (!proj.active || proj.hostile || !proj.friendly || proj.minion || proj.sentry || proj.owner < 0 || proj.owner >= 255) continue;

                    if (MathF.Abs(proj.Center.X - Projectile.Center.X) < 160 * Projectile.scale && MathF.Abs(proj.Center.Y - Projectile.Center.Y) < 480 * Projectile.scale)
                    {
                        proj.velocity = (proj.velocity + proj.Center.DirectionTo(new Vector2(Projectile.Center.X, proj.Center.Y)) * 200f * (Projectile.scale - 2f) / MathF.Max(1f, proj.Distance(Projectile.Center)));
                        //proj.velocity.Y *= 0.90f;
                        if (MathF.Abs(proj.Center.Y - Projectile.Center.Y) > 160f) proj.velocity.Y += proj.Center.Y < Projectile.Center.Y ? 0.5f : -0.5f;

                        if (MathF.Abs(proj.velocity.ToRotation() - proj.Center.AngleTo(Main.player[proj.owner].Center)) < 0.1f)
                        {
                            proj.friendly = false;
                            proj.hostile = true;
                        }
                    }
                }
            }

            if (Projectile.timeLeft <= 90)
            {
                Projectile.scale -= 4f / 90f;
                if (Projectile.scale < 0) Projectile.scale = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // stolen from forbidden sandstorm projectile (https://github.com/Fargowilta/FargowiltasSouls/blob/master/Content/Projectiles/Souls/ForbiddenTornado.cs)
            float halfheight = 220 * MathF.Sqrt(Projectile.scale);
            float density = 50f;
            for (float i = 0; i < (int)density; i++)
            {
                Color color = new(212, 192, 100);
                color.A /= 2;
                float lerpamount = Math.Abs(density / 2 - i) > density / 2 * 0.6f ? Math.Abs(density / 2 - i) / (density / 2) : 0f; //if too low or too high up, start making it transparent
                color = Color.Lerp(color, Color.Transparent, lerpamount);
                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Vector2 offset = Vector2.SmoothStep(Projectile.Center + Vector2.UnitY * halfheight, Projectile.Center - Vector2.UnitY * halfheight, i / density);
                float scale = MathHelper.Lerp(Projectile.scale * 0.8f, Projectile.scale * 2.5f, i / density);
                Main.EntitySpriteDraw(texture, offset - Main.screenPosition,
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    Projectile.GetAlpha(color),
                    i / 6f - Main.GlobalTimeWrappedHourly * 5f + Projectile.rotation,
                    texture.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0);
            }
            return false;
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GTBSandstormGlobalProj : GlobalProjectile
    {
        public int stormTime;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            stormTime = -1;
            base.OnSpawn(projectile, source);
        }

        public override void AI(Projectile projectile)
        {
            if (stormTime == -1) return;
        }
    }
}
