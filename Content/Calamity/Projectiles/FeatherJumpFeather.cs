using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ID;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class FeatherJumpFeather : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/StickyFeather";
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.width = Projectile.height = 15;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner >= 0)
            {
                Player owner = Main.player[Projectile.owner];
                if (owner != null) {
                    CrossplayerCalamity cplayer = owner.GetModPlayer<CrossplayerCalamity>();
                    if (cplayer.ForceEffect(ModContent.ItemType<AerospecEnchantment>()) && cplayer.AeroCritBoost < 50f)
                    {
                        cplayer.AeroCritBoost += 10f;
                    }else if (cplayer.AeroCritBoost < 20f)
                    {
                        cplayer.AeroCritBoost += 5f;
                    }
                }
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += 0.1f;
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.velocity.X, Projectile.velocity.Y, Scale: 0.5f);
            base.AI();
        }
    }
}
