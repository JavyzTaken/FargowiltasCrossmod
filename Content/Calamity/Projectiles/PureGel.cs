using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.SummonItems;
using FargowiltasCrossmod.Core.Common;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PureGel : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override string Texture => "CalamityMod/Items/Materials/PurifiedGel";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 240;
            Projectile.scale = 0.7f;
            Projectile.width = Projectile.height = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft > 210 && Projectile.ai[2] == target.whoAmI)
            {
                return false;
            }
            return base.CanHitNPC(target);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            if (Projectile.timeLeft > 238)
            {
                return false;
            }
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Pink with { A = 70 } * 0.7f * (1 - i / 5f), Projectile.oldRot[i], t.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            DLCUtils.DrawBackglow(t, Color.Pink * 0.7f, Projectile.Center, t.Size() / 2, Projectile.rotation, Projectile.scale, offsetMult:4);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * 0.5f, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            
            return false;
            
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            
            
            base.AI();
        }
    }
}
