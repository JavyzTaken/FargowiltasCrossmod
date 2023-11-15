using FargowiltasCrossmod.Core.Utils;
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
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    public class FrostFlare : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/FrostyFlare";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.scale = 2;
            Projectile.width = Projectile.height = 15;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 140;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
            for (int i = 0; i < 100; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            Projectile.velocity /= 1.03f;
            Projectile.rotation += Projectile.velocity.Length() / 20;
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
            if (Projectile.timeLeft < 30 && DLCUtils.HostCheck)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Main.rand.Next(-50, 50), -900), new Vector2(0, 15), ModContent.ProjectileType<FrostShard>(), Projectile.damage, 0);
            }
        }
    }
}
