using CalamityMod.NPCs.AquaticScourge;
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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    public class SuckedRock : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/CrabBoulder";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1000;
            Projectile.light = 1f;

            base.SetDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor * (1 - (float)i / ProjectileID.Sets.TrailCacheLength[Type]), t.Value, Projectile.oldPos[i] + Projectile.Size/2, Projectile.oldRot[i]);
            }
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, t.Value);
            
            return false;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<AquaticScourgeHead>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation += MathHelper.ToRadians(5);
            if (Projectile.ai[1] != 3)
            {
                Projectile.velocity = Projectile.AngleTo(owner.Center).ToRotationVector2() * 10;

                
                if (Projectile.Hitbox.Intersects(owner.Hitbox) || Math.Abs(MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(owner.velocity, Projectile.velocity))) < 90)
                {
                    if (Projectile.ai[1] == 1)
                    {
                        SoundEngine.PlaySound(SoundID.Item2 with { Pitch = -0.75f, PitchVariance = 0.2f, MaxInstances = 10 }, owner.Center);
                    }
                    Projectile.Kill();
                }
            }
            base.AI();
        }
    }
}
