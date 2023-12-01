using FargowiltasCrossmod.Core;
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
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
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
            Projectile.timeLeft = 180;

            Projectile.light = 0.5f;

            Projectile.tileCollide = false;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
            for (int i = 0; i < 100; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
            }
        }
        public override bool? CanDamage() => false;
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
            float decel = 30 / 180f;
            float newVel = MathHelper.Clamp(Projectile.velocity.Length() - decel, 0, 100);
            Projectile.velocity = Utils.SafeNormalize(Projectile.velocity, Vector2.Zero) * newVel;
            Projectile.rotation += Projectile.velocity.Length() / 80f;

            Vector2 vel = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            for (int i = -3; i < 4; i+= 2)
            {
                Vector2 dVel = vel * 6 * i;
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, dVel.X, dVel.Y).noGravity = true;
            }

            if (Projectile.timeLeft == 10)
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            if (Projectile.timeLeft < 10 && DLCUtils.HostCheck)
            {
                for (int i = -1; i < 2; i += 2)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (vel * Main.rand.NextFloat(13, 16) * i).RotatedByRandom(MathHelper.PiOver2 / 10f), ModContent.ProjectileType<FrostShard>(), Projectile.damage, 0);
            }
        }
    }
}
