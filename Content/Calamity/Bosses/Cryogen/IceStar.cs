using CalamityMod;
using FargowiltasCrossmod.Core;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class IceStar : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/IceStar";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 60 * 4 + 30;

            Projectile.light = 0.5f;
            Projectile.tileCollide = false;
            //Projectile.coldDamage = true;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 100; i++)
            {
                Dust.NewDustDirect(Projectile.position, 40, 40, DustID.SnowflakeIce).noGravity = true;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size/2 - Main.screenPosition, null, lightColor * (1- (float)i/Projectile.oldPos.Length), Projectile.oldRot[i], t.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        ref float OwnerID => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        public override bool? CanDamage() => Projectile.Opacity > 0.5f;
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            Player target = Main.player[owner.target];
            if (target == null || owner == null || !owner.active || !target.active || owner.type != ModContent.NPCType<PermafrostBoss>())
            {
                Projectile.Kill();
                return;
            }

            /*
            PermafrostBoss permafrost = owner.ModNPC as PermafrostBoss;
            if (PermafrostBoss.SetupAttacks.Contains((PermafrostBoss.Attacks)permafrost.Attack) && permafrost.Attack != (float)PermafrostBoss.Attacks.IceStar)
            {
                Projectile.Kill();
                return;
            }
            */
            if (Timer > 0) //homing
            {
                Vector2 vectorToIdlePosition = target.Center - Projectile.Center;
                float speed = 18f;
                float inertia = 90f;
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
                const int MaxSpeed = 35;
                Projectile.velocity.ClampMagnitude(0, MaxSpeed);
                /*
                const float MaxSpeed = 30;
                float inertia = 15f;
                float CloseEnough = 0;

                Vector2 toTarget = target.Center - Projectile.Center;
                float distance = toTarget.Length();
                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }

                if (Projectile.timeLeft > 30)
                {
                    Projectile.timeLeft = 30;
                }
                Projectile.velocity = toTarget;
                */
            }
            if (Projectile.timeLeft < 60)
            {
                Projectile.Opacity = MathHelper.Lerp(1f, 0f, Projectile.timeLeft / 60f);
            }
            Timer++;
            Projectile.rotation += 0.2f;
            /*
            int p = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            if (p >= 0)
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Utils.AngleTowards(Projectile.velocity.ToRotation(), Projectile.AngleTo(Main.player[p].Center), 0.04f));
            */
            Dust.NewDustDirect(Projectile.position, 40, 40, DustID.SnowflakeIce).noGravity = true;

            base.AI();
        }
    }
}
