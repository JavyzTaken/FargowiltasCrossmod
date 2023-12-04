using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class IceArrow : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Ammo/IcicleArrow";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }
        public override void SetDefaults()
        {
            
            Projectile.width = Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 200;

            Projectile.light = 0.5f;
            Projectile.tileCollide = false;

            //Projectile.coldDamage = true;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(8.5f, 40.5f), Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Dust.NewDustDirect(Projectile.position, 10, 10, DustID.SnowflakeIce).noGravity = true;
            if (Projectile.velocity.Y < 15)
            Projectile.velocity.Y += 0.1f;
            base.AI();
        }
    }
}
