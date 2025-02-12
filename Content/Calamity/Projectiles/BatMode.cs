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
            Projectile.width = Projectile.height = 60;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Main.projFrames[Type] = 4;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Color color = lightColor;
            
            for (int i = 0; i < 30; i++)
            {
                float offX = (float)Math.Sin((Projectile.timeLeft + i*20) * 0.01f *10) * Projectile.width/2;
                float offY = (float)Math.Cos((Projectile.timeLeft ) * 0.01f + i * 10) * Projectile.width/2;
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2(offX, offY), new Rectangle(0, t.Height() / 4 * Projectile.frame, t.Width(), t.Height() / 4), color, Projectile.rotation, new Vector2(t.Width(), t.Height() / 4) / 2, Projectile.scale, i % 2 == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner < 0 || !Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            Player owner = Main.player[Projectile.owner];
            owner.HealEffect(10);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < Projectile.width; i += 3)
            {
                for (int j = 0; j < Projectile.height; j += 3)
                {
                    Dust d = Dust.NewDustDirect(Projectile.position + new Vector2(i, j), 1, 1, DustID.Smoke, Scale: 1.2f);
                    d.velocity *= 0.5f;
                }
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
            Projectile.Center = owner.Center;
            
        }
    }
}
