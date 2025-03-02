using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneArtillery : BrimstoneFireball
    {
        public override void Movement()
        {
            Projectile.velocity.Y += 0.68f;
        }
    }
}
