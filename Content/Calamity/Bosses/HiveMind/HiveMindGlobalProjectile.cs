using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HiveMindGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation)
        {
            return projectile.type == ModContent.ProjectileType<ShaderainHostile>();
        }
        public bool Emode = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (CalamityGlobalNPC.hiveMind.IsWithinBounds(Main.maxNPCs) && Main.npc[CalamityGlobalNPC.hiveMind] is NPC hiveMind && hiveMind.TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>() && hiveMind.TryGetDLCBehavior(out HMEternity hmEternity))
            {
                if (source is EntitySource_Parent parentSource && parentSource.Entity is Projectile parent && parent.type == ModContent.ProjectileType<ShadeNimbusHostile>())
                {
                    Emode = true;
                }
            }
        }
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (Emode)
            {
                modifiers.Null();
                target.AddBuff(ModContent.BuffType<BrainRot>(), 360);
                target.AddBuff(BuffID.CursedInferno, 360);
                target.AddBuff(BuffID.Darkness, 360);
            }
        }
    }
}
