using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.DataStructures;
using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PlagueCloud : ModProjectile, IAdditiveDrawer
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.timeLeft = 800;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = Projectile.width = 140;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            
        }
        public override string Texture => "CalamityMod/Projectiles/Summon/SmallAresArms/MinionPlasmaGas";
        public float rotation1 = MathHelper.PiOver2;
        public float rotation2 = 0;
        public void AdditiveDraw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            spriteBatch.Draw(t.Value, Projectile.Center - Main.screenPosition, null, Color.Green with {  } * Projectile.Opacity, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(t.Value, Projectile.Center - Main.screenPosition, null, Color.Green with { } * Projectile.Opacity, rotation1, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(t.Value, Projectile.Center - Main.screenPosition, null, Color.Green with { } * Projectile.Opacity, rotation2, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(projHitbox.Center.ToVector2()) < Projectile.width / 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }
        public override void AI()
        {
            float maxPlagueCharge = 400;
            if (Projectile.ai[1] == 0)
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 6, 0.03f);
                if (Projectile.timeLeft > 30)
                {
                    Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0.9f, 0.03f);
                }
                Projectile.velocity *= 0.97f;
                Projectile.rotation += 0.007f;
                rotation1 += 0.01f;
                rotation2 -= 0.005f;
                Projectile.Resize((int)(140 * Projectile.scale), (int)(140 * Projectile.scale));
                if (Main.rand.NextBool(20))
                {
                    Particle part = new TimedSmokeParticle(Projectile.Center + new Vector2(0, Main.rand.NextFloat(0, 75)).RotatedByRandom(MathHelper.TwoPi) * Projectile.scale, Vector2.Zero, Color.Green * 0, Color.LimeGreen, 2f, 0.5f, 100, Main.rand.NextFloat(-0.02f, 0.02f));
                    GeneralParticleHandler.SpawnParticle(part);
                }
                if (Projectile.timeLeft < 30)
                {
                    Projectile.Opacity = MathHelper.Lerp(0.9f, 0, 1 - (Projectile.timeLeft / 30f));
                }
                Projectile.Calamity().timesPierced = 0;
            }
            else
            {
                Player owner = Main.player[(int)Projectile.ai[0]];
                if (owner == null || !owner.active || owner.dead || !owner.HasEffect<PlaguebringerEffect>())
                {
                    Projectile.Kill();
                    return;
                }
                else if (Main.netMode != NetmodeID.Server)
                {
                    Projectile.timeLeft = 2;
                    Projectile.Center = owner.Center;
                    Projectile.scale = MathHelper.Lerp(0.1f, 1, owner.CalamityAddon().PlagueCharge / maxPlagueCharge);
                    Projectile.Resize((int)(140 * Projectile.scale), (int)(140 * Projectile.scale));
                    Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0.9f, 0.03f);
                    Projectile.rotation += 0.01f;
                    rotation1 += 0.01f;
                    rotation2 -= 0.005f;
                    
                    NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.whoAmI);
                    
                }
            }
        }
    }
}
