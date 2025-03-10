using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
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
    public class GaleSpine : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/SandTooth";
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
            SpriteEffects effects = SpriteEffects.None;
            float drawRotation = -135;
            if (Projectile.ai[1] == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
                drawRotation = -45;
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + MathHelper.ToRadians(drawRotation), t.Size() / 2, Projectile.scale, effects);

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.timeLeft = 3;
            Projectile.ai[2] = 2;
            Vector2 center = Projectile.Center;
            Projectile.width = Projectile.height = 80;
            Projectile.Center = center;
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 60);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[2] == 0)
                return false;
            return base.CanHitNPC(target);
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if ((owner == null || !owner.active || owner.dead) && Projectile.ai[2] == 0 || !owner.HasEffect<GaleSpineEffect>())
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.timeLeft <= 3)
            {
                Projectile.ai[2] = 2;
            }
            if (Projectile.ai[2] == 0)
            {
                //Main.NewText(owner.CalamityAddon().ExploFeatherCount);
                Projectile.timeLeft = 300;
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.ai[0] + (Projectile.ai[1] == 1 ? 0 : MathHelper.ToRadians(180)), 0.03f);
                Projectile.Center = owner.Center - Projectile.rotation.ToRotationVector2() * 20;
                if (owner.CalamityAddon().ExploFeatherCount >= 7)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * -20;
                    Projectile.ai[2] = 1;
                }
            }
            if (Projectile.ai[2] == 1)
            {
                NPC t = Projectile.FindTargetWithinRange(1000, true);
                if (t != null)
                    CalamityUtils.HomeInOnNPC(Projectile, true, 1000, 10, 15);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180);
            }
            if (Projectile.ai[2] == 2 && Projectile.timeLeft == 2)
            {
                SoundEngine.PlaySound(SoundID.Item147 with { Pitch = 1, Volume = 0.7f }, Projectile.Center);
                Projectile.Opacity = 0;
                Projectile.velocity = Vector2.Zero;
                for (int i = 0; i < 200; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.Center + new Vector2(Main.rand.NextFloat(0, 80), 0).RotatedByRandom(MathHelper.TwoPi), 1, 1, DustID.CursedTorch, 0, 0, 0);
                    d.noGravity = true;
                    d.scale *= Main.rand.NextFloat(1, 2);
                }
            }
            base.AI();
        }
    }
}
