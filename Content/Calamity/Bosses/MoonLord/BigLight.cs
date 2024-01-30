using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.MoonLord
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BigLight : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FairyQueenSunDance;
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Main.projFrames[Type] = 2;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 2;
            Projectile.scale = 0;
            Projectile.timeLeft = 300;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, t.Width(), t.Height() / 2), new Color(255, 255, 255), Projectile.rotation, new Vector2(0, t.Height() / 4), Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, t.Height() / 2, t.Width(), t.Height() / 2), new Color(255, 255, 255), Projectile.rotation, new Vector2(0, t.Height() / 4), Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 800);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint5 = 0f;
            _ = Projectile.scale;
            float f4 = Projectile.rotation;
            Vector2 objectPosition = targetHitbox.TopLeft();
            Vector2 objectDimensions = targetHitbox.Size();
            Vector2 vector8 = f4.ToRotationVector2();
            float num14 = Projectile.scale * 0.7f;
            if (Collision.CheckAABBvLineCollision(objectPosition, objectDimensions, Projectile.Center, Projectile.Center + vector8 * Projectile.scale * 510f, num14 * 100f, ref collisionPoint5))
            {
                return true;
            }
            if (Collision.CheckAABBvLineCollision(objectPosition, objectDimensions, Projectile.Center, Projectile.Center + vector8 * Projectile.scale * 660f, num14 * 60f, ref collisionPoint5))
            {
                return true;
            }
            if (Collision.CheckAABBvLineCollision(objectPosition, objectDimensions, Projectile.Center, Projectile.Center + vector8 * Projectile.scale * 800f, num14 * 10f, ref collisionPoint5))
            {
                return true;
            }
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.ai[2];
            if (Projectile.ai[1] >= 0)
            {
                NPC owner = Main.npc[(int)Projectile.ai[1]];
                if (owner != null && owner.active && owner.type == NPCID.MoonLordCore)
                {
                    Projectile.Center = owner.Center;
                }
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 100)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.3f, 0.03f);
            }
            if (Projectile.ai[0] > 100 && Projectile.ai[0] < 250)
            {
                if (Projectile.ai[0] == 101)
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/TeslaCannonFire"), Projectile.Center);
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1.5f, 0.03f);
            }
            if (Projectile.ai[0] > 250)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 0, 0.08f);
            }

            base.AI();
        }
    }
}
