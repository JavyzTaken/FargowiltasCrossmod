using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasSouls;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class VictideSpike : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/UrchinStinger";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.light = 1f;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[Projectile.owner].ForceEffect<VictideEffect>())
            {
                target.AddBuff(BuffID.Venom, 500);
            }
            else
            {
                target.AddBuff(BuffID.Poisoned, 500);
            }
            }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Ice_Purple);
                d.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, t.Size()/2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            if (Projectile.owner < 0)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];
            if (owner == null || !owner.active || owner.dead) {
                Projectile.Kill();
                return;
            }
            if (Projectile.ai[2] == 0)
            {
                Projectile.ai[1] += 0.1f;
                Projectile.Center = owner.Center + new Vector2(0, MathHelper.Lerp(0, 22, Projectile.ai[1])).RotatedBy(Projectile.ai[0]);

                Projectile.rotation = Projectile.AngleFrom(owner.Center) + MathHelper.PiOver2;
                if (Projectile.ai[1] >= 1)
                {
                    Projectile.ai[1] = 0;
                    Projectile.ai[2] = 1;
                }
            }
            if (Projectile.ai[2] == 1)
            {
                Projectile.Center = owner.Center + new Vector2(0, MathHelper.Lerp(22, 14, Projectile.ai[1])).RotatedBy(Projectile.ai[0]);
                
                Projectile.rotation = Projectile.AngleFrom(owner.Center) + MathHelper.PiOver2;
                //Projectile.Center += new Vector2(3, 0);
                if (owner.HeldItem.damage > 0 && owner.controlUseItem && Projectile.ai[1] < 1)
                {
                    Projectile.ai[1] += 0.05f;
                }
                else if (Projectile.ai[1] > 0 && !owner.controlUseItem)
                {
                    Projectile.ai[1] -= 0.1f;
                    
                }
                else if (Projectile.ai[1] < 0)
                {
                    Projectile.ai[1] = 0;
                }
                if (owner.HasEffect<VictideEffect>())
                {
                    Projectile.timeLeft = 2;
                }
                else
                {
                    Projectile.timeLeft = 0;
                }
            }
            if (!owner.controlUseItem && Projectile.ai[1] >= 0.9f && Projectile.ai[2] == 1)
            {
                Projectile.timeLeft = 100;
                Projectile.ai[2] = 2;
                Projectile.tileCollide = true;
                Projectile.penetrate = 2;
                Projectile.velocity = new Vector2(0, -Main.rand.NextFloat(4f, 6f)).RotatedBy(Projectile.rotation) + owner.velocity / 2;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
            }
            if (Projectile.ai[2] == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                if (Projectile.velocity.Y < 10 && !Main.player[Projectile.owner].ForceEffect<VictideEffect>())
                {
                    Projectile.velocity.Y += 0.1f;
                }
            }
        }
    }
}
