using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class GravityVileClot : VileClotDrop
    {
        public override string Texture => "FargowiltasCrossmod/Content/Calamity/Bosses/HiveMind/VileClotDrop";
        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            Projectile.velocity.Y += 0.08f;

            if (Main.rand.NextBool(6))
            {
                int vileDust = Dust.NewDust(Projectile.position,
                Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, default, 1.5f);
                Main.dust[vileDust].noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            float sin = MathF.Sin(Projectile.timeLeft * MathHelper.Pi / 7f);
            Projectile.Opacity = 0.85f + 0.15f * sin;
            Projectile.scale = 1f + 0.15f * sin;
        }
    }
}
