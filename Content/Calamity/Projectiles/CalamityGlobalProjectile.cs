using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ID;
using System.Threading;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public partial class CalamityGlobalProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile entity)
        {
            base.SetDefaults(entity);
        }
        public override bool InstancePerEntity => true;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);
        }
        public override void Kill(Projectile projectile, int timeLeft)
        {
            base.Kill(projectile, timeLeft);
        }
        public float AstralTelegraphOpacity = 0;
        public override bool PreDrawExtras(Projectile projectile)
        {
            Player Player = Main.player[Main.myPlayer];
            if (Player.GetModPlayer<CrossplayerCalamity>().Astral && projectile.hostile && projectile.damage > 0 && Player.GetToggleValue("AstralTelegraph"))
            {
                if (AstralTelegraphOpacity < 0.7f)
                {
                    AstralTelegraphOpacity += 0.05f;
                }
                Main.instance.LoadProjectile(ProjectileID.MedusaHeadRay);
                Asset<Texture2D> ray = TextureAssets.Projectile[ProjectileID.MedusaHeadRay];
                Main.EntitySpriteDraw(ray.Value, projectile.Center - Main.screenPosition, null, new Color(250, 250, 250) * AstralTelegraphOpacity, projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(ray.Width() / 2, ray.Height()), new Vector2(projectile.width / 20f, projectile.velocity.Length()/2), SpriteEffects.None);
            }
            else if (AstralTelegraphOpacity > 0)
            {
                AstralTelegraphOpacity -= 0.01f;
                Main.instance.LoadProjectile(ProjectileID.MedusaHeadRay);
                Asset<Texture2D> ray = TextureAssets.Projectile[ProjectileID.MedusaHeadRay];
                Main.EntitySpriteDraw(ray.Value, projectile.Center - Main.screenPosition, null, new Color(250, 250, 250) * AstralTelegraphOpacity, projectile.velocity.ToRotation() + MathHelper.PiOver2, new Vector2(ray.Width() / 2, ray.Height()), new Vector2(projectile.width / 20f, projectile.velocity.Length() / 2), SpriteEffects.None);
            }
            return base.PreDrawExtras(projectile);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            return base.PreDraw(projectile, ref lightColor);
        }
        public override bool PreAI(Projectile projectile)
        {
            return base.PreAI(projectile);
        }
        public override void AI(Projectile projectile)
        {
            if (projectile.owner >= 0 && Main.player[projectile.owner] != null && Main.player[projectile.owner].active && Main.player[projectile.owner].GetToggleValue("ChargeAttacks"))
            GemTechMinionEffect(projectile);
        }
    }
}
