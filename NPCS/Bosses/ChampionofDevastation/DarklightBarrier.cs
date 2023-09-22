using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DarklightBarrier : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/LightBeam";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.width = 3900;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.ai[1] += 1;

            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            for (int i = -20; i < 20; i++)
            {

                Vector2 pos = Projectile.Center + new Vector2(i * 100 + Projectile.ai[1] * 4, 0);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()) * (1 - pos.Distance(Projectile.Center) / 1700f), MathHelper.ToRadians(Projectile.ai[1] * 14.4f), t.Size() / 2, 1f, SpriteEffects.None, 0);

            }
            if (Projectile.ai[1] >= 25)
            {
                Projectile.ai[1] = 0;
            }
            return false;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner.active && owner != null)
            {
                Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.active = false;
            }
            Player target = Main.player[owner.target];
            if (Projectile.Center.Y > owner.Center.Y - 600)
            {
                Projectile.velocity.Y = -3;
            }
            else
            {
                Projectile.velocity.Y = 3;
            }
            Projectile.position.X = owner.position.X - Projectile.width / 2;
            if (target.Center.Y < Projectile.Center.Y)
            {
                Dust.NewDust(target.position, target.width, target.height, DustID.Clentaminator_Purple);
                target.velocity.Y = 3;
            }
        }
    }
}
