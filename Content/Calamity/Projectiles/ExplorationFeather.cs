using CalamityMod;
using FargowiltasCrossmod.Core.Calamity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    public class ExplorationFeather : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/StickyFeather";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.rotation = MathHelper.ToRadians(-90);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + MathHelper.ToRadians(-90), t.Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 3;
            Projectile.ai[1] = 2;
            Vector2 center = Projectile.Center;
            Projectile.width = Projectile.height = 80;
            Projectile.Center = center;
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if ((owner == null || !owner.active || owner.dead) && Projectile.ai[1] == 0)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.timeLeft <= 3)
            {
                Projectile.ai[1] = 2;
            }
            if (Projectile.ai[1] == 0)
            {
                Projectile.timeLeft = 300;
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.ai[0], 0.03f);
                Projectile.Center = owner.Center - Projectile.rotation.ToRotationVector2() * 30;
                if (owner.CalamityAddon().ExploFeatherCount > 40)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * -20;
                    Projectile.ai[1] = 1;
                }
            }
            if (Projectile.ai[1] == 1)
            {
                NPC t = Projectile.FindTargetWithinRange(1000, true);
                if (t != null)
                    CalamityUtils.HomeInOnNPC(Projectile, true, 1000, 10, 15);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180);
            }
            if (Projectile.ai[1] == 2 && Projectile.timeLeft == 2)
            {
                SoundEngine.PlaySound(SoundID.Item147 with { Pitch = 1, Volume = 0.7f }, Projectile.Center);
                Projectile.Opacity = 0;
                Projectile.velocity = Vector2.Zero;
                for (int i = 0; i < 200; i++)
                {
                    Dust.NewDustDirect(Projectile.Center + new Vector2(Main.rand.NextFloat(0, 80), 0).RotatedByRandom(MathHelper.TwoPi), 1, 1, DustID.UnusedWhiteBluePurple, 0, 0, 0);
                    if (Main.rand.NextBool(50))
                    {
                        Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center + new Vector2(Main.rand.NextFloat(0, 40), 0).RotatedByRandom(MathHelper.TwoPi) - new Vector2(15, 15), Vector2.Zero, Main.rand.Next(11, 14));
                    }
                }
            }
            base.AI();
        }
    }
}
