using CalamityMod.Dusts;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ASGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int Timer = 0;
        public override bool AppliesToEntity(Projectile proj, bool lateInstantiation)
        {
            return proj.type == ModContent.ProjectileType<CrabBoulder>();
        }
        public override void SetDefaults(Projectile proj)
        {
            base.SetDefaults(proj);
            if (Main.npc.Any(n => n.TypeAlive<AquaticScourgeHead>() && n.TryGetDLCBehavior(out ASEternity emode) && emode != null))
            {
                if (proj.light < 1f)
                    proj.light = 1f;
            }
        }
    }
}
