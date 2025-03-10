using System;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    public class IchorShotFast : IchorShot
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/IchorShot";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.extraUpdates = 1;
        }
    }
}
