using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
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
    public class VampireBat : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
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
            Projectile.width = Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
            Projectile.penetrate = -1;
            Main.projFrames[Type] = 4;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Color color = lightColor;
            if (Projectile.ai[0] == 1)
            {
                color = Color.Red with { A = 200 };
            }
            
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, t.Height() / 4 * Projectile.frame, t.Width(), t.Height() / 4), color, Projectile.rotation, new Vector2(t.Width(), t.Height() / 4) / 2, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = 0;
            Projectile.ai[0] = 1;
            Projectile.velocity *= 0;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
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
            NPC target = Projectile.FindTargetWithinRange(1000, true);
            if (Projectile.owner < 0 || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                int maxSpeed = 10;
                if (Main.player[Projectile.owner].ForceEffect<UmbraphileEffect>())
                {
                    maxSpeed = 15;
                }
                if (target != null && target.active && Projectile.ai[0] == 0)
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * maxSpeed, 0.03f);
                }
                else if (Projectile.ai[0] == 1)
                {
                    Dust d = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.RedTorch, Scale: 1);
                    d.noGravity = true;
                    d.velocity = Vector2.Zero;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (maxSpeed + 2), 0.05f);
                    if (Projectile.Hitbox.Intersects(Main.player[Projectile.owner].Hitbox))
                    {
                        Main.player[Projectile.owner].HealEffect(35);
                        Projectile.Kill();
                    }
                }
            }
            
        }
    }
}
