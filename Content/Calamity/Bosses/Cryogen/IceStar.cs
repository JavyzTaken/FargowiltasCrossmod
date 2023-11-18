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
    public class IceStar : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/IceStar";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 300;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 100; i++)
            {
                Dust.NewDustDirect(Projectile.position, 40, 40, DustID.SnowflakeIce).noGravity = true;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size/2 - Main.screenPosition, null, lightColor * (1- (float)i/Projectile.oldPos.Length), Projectile.oldRot[i], t.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            Projectile.rotation += 0.2f;
            int p = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            if (p >= 0)
            Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy(Utils.AngleTowards(Projectile.velocity.ToRotation(), Projectile.AngleTo(Main.player[p].Center), 0.04f));
            Dust.NewDustDirect(Projectile.position, 40, 40, DustID.SnowflakeIce).noGravity = true;

            base.AI();
        }
    }
}
