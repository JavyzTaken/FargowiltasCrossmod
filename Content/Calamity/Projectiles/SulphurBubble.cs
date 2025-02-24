using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SulphurBubble : ModProjectile
    {
        
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override string Texture => "CalamityMod/Projectiles/Enemy/SulphuricAcidBubble";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.scale = 3f;
            Projectile.Opacity = 0;
            Projectile.tileCollide = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            Color color = new Color(100, 120, 50);
            Particle p = new TimedSmokeParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(0, 16), 0).RotatedByRandom(MathHelper.PiOver2), Projectile.velocity + new Vector2(0, Main.rand.NextFloat(-2, 0)).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)), Color.Gray, color, 2, 0.7f, 50);
            GeneralParticleHandler.SpawnParticle(p);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 30 * Projectile.frame, 30, 30), lightColor * Projectile.Opacity, 0, new Vector2(15, 15), Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            if (Projectile.localAI[2] == 0) // just spawned
            {
                Projectile.localAI[2] = 1;
                Projectile.ai[1] = 15;
            }
            if (Projectile.ai[0] > 1)
                Projectile.Kill();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame > 6)
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.Opacity < 0.7f) Projectile.Opacity += 0.01f;
            Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -4, 0.01f);
            Projectile.velocity.X *= 0.97f;


            if (Projectile.ai[1] <= 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i] != null && Main.projectile[i].active && Main.projectile[i].damage > 0 && i != Projectile.whoAmI && Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox) && Main.projectile[i].type != ModContent.ProjectileType<SulphurCloud>() && Main.projectile[i].type != ModContent.ProjectileType<SulphurBubble>())
                        {
                            for (int a = 0; a < 2; a++)
                                OnHitEffect(Main.projectile[i].damage);
                            break;
                        }
                    }
                }
            }
            else
            {
                Projectile.ai[1]--;
            }
        }
        public void OnHitEffect(int baseDamage)
        {
            Projectile.ai[1] = 30;
            Projectile.ai[0]++;
            Projectile.scale -= 0.4f;
            int gasSpeedMin = 3;
            int gasSpeedMax = 6;
            int damage = (int)(baseDamage * 0.5f);
            damage += 8;
            int count = 3;


            if (Projectile.ai[0] > 2)
            {
                gasSpeedMin *= 2;
                gasSpeedMax *= 2;
                damage *= 2;
                count *= 2;
            }
            if (damage < 20)
                damage = 20;
            for (int j = 0; j < count; j++)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, Main.rand.NextFloat(gasSpeedMin, gasSpeedMax)).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<SulphurCloud>(), damage, 0, Projectile.owner);
                NetMessage.SendData(MessageID.SyncProjectile, number: proj);
            }
            SoundEngine.PlaySound(SoundID.Item85, Projectile.Center);
            NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.whoAmI);
        }
    }
}
