using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
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
        const int MaxTime = 60 * 5;
        public override void SetDefaults()
        {
            Projectile.scale = 2;
            Projectile.width = Projectile.height = 15;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = MaxTime;

            Projectile.light = 0.5f;

            Projectile.tileCollide = false;
            Projectile.coldDamage = true;
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
            //Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void AI()
        {
            float decel = 30 / 90f;
            float newVel = MathHelper.Clamp(Projectile.velocity.Length() - decel, 0, 100);
            Projectile.velocity = Utils.SafeNormalize(Projectile.velocity, Vector2.Zero) * newVel;
            //Projectile.rotation += Projectile.velocity.Length() / 80f;

            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 vel = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            
            /*
            if (Projectile.timeLeft < 180f && Projectile.timeLeft <= 120)
            {
                for (int i = -3; i < 4; i += 2)
                {
                    Vector2 dVel = (vel * 7 * i * Main.rand.NextFloat()).RotatedByRandom(MathHelper.PiOver2 / 10f);
                    dVel += Projectile.velocity;
                    dVel *= 3;
                    Particle p = new FogPuff(Projectile.Center, dVel, Color.GhostWhite, 0.2f, 30, Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(-0.2f, 0.2f));
                    p.Spawn();
                    //Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, dVel.X, dVel.Y).noGravity = true;
                }
            }
            */
            const int ActivationTime = MaxTime - 60;

            if (Projectile.timeLeft == ActivationTime)
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            if (Projectile.timeLeft < ActivationTime && DLCUtils.HostCheck && Projectile.timeLeft % 2 == 0)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 dVel = (vel * 14 * i * Main.rand.NextFloat()).RotatedByRandom(MathHelper.PiOver2 / 10f);
                    //dVel += Projectile.velocity;
                    dVel *= 3;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dVel, ModContent.ProjectileType<IceCloud>(), Projectile.damage, 0, ai0: 1);
                    if (p.IsWithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].timeLeft = 60;
                    }
                }
                    /*
                    for (int i = -1; i < 2; i += 2)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (vel * Main.rand.NextFloat(30, 40) * i).RotatedByRandom(MathHelper.PiOver2 / 20f), ModContent.ProjectileType<FrostShard>(), Projectile.damage, 0);
                    */
            }
        }
    }
}
