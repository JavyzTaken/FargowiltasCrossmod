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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class GravityVileClot : VileClot
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/VileClot";
        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            Projectile.velocity.Y += 0.08f;

            int vileDust = Dust.NewDust(Projectile.position,
                Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 100, default, 1.5f);
            Main.dust[vileDust].noGravity = true;

            Projectile.rotation += 0.3f * (float)Projectile.direction;
        }
    }
}
