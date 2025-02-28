using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Vortex;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BatMode : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bat;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = Projectile.height = 6;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Main.projFrames[Type] = 4;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.owner < 0 || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = Main.player[Projectile.owner].CalamityAddon().BatTime;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Color color = lightColor;
            //FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, t.Value);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size/2 - Main.screenPosition, new Rectangle(0, t.Height() / 4 * Projectile.frame, t.Width(), t.Height() / 4), lightColor * (1 - i / 3f), Projectile.oldRot[i], new Vector2(t.Width(), t.Height() / 4) / 2, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, t.Height() / 4 * Projectile.frame, t.Width(), t.Height() / 4), lightColor, Projectile.rotation, new Vector2(t.Width(), t.Height() / 4) / 2, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.owner < 0 || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return false;
            }
            return Main.player[Projectile.owner].CalamityAddon().BatHitCD == 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner < 0 || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];
            owner.CalamityAddon().BatHitCD = 20;
            owner.Heal(10);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Smoke, Scale: 1.2f);
                d.velocity *= 0.5f;
            }
        }
        public override void AI()
        {
            

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                }
            } 
            if (Projectile.owner < 0 || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];
            Projectile.direction = owner.direction;
            Projectile.velocity += Projectile.AngleTo(owner.Center).ToRotationVector2();
            
            //not allowed to be too far
            if (Projectile.Distance(owner.Center) > 60)
            {
                Projectile.Center = owner.Center;
                Projectile.velocity = new Vector2(Main.rand.NextFloat(1, 10), 0).RotatedByRandom(MathHelper.TwoPi);
            }
            
        }
    }
}
