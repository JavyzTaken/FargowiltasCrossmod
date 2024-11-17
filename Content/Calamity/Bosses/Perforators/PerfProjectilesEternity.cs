﻿using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PerfProjectilesEternity : GlobalProjectile
    {
        public override bool TileCollideStyle(Projectile projectile, ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (!CalDLCWorldSavingSystem.E_EternityRev)
            {
                return base.TileCollideStyle(projectile, ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
            }
            if (projectile.type == ModContent.ProjectileType<IchorBlob>())
            {
                fallThrough = true;
            }
            return base.TileCollideStyle(projectile, ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
    }
}
