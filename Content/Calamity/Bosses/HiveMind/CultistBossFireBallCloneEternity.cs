using CalamityMod.NPCs.HiveMind;
using FargowiltasCrossmod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using FargowiltasSouls.Core.Systems;
using FargowiltasCrossmod.Core.Calamity.Systems;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CultistBossFireballCloneEternity : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation)
        {
            return projectile.type == ProjectileID.CultistBossFireBallClone;
        }
        public override bool InstancePerEntity => true;
        private bool FromCreeper = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
            {
                if (source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.type == ModContent.NPCType<DankCreeper>() && npc.TryGetGlobalNPC(out DankCreeperEternity _))
                {
                    FromCreeper = true;
                }
            }
            base.OnSpawn(projectile, source);
        }
        public float timer = 0;
        public override bool PreAI(Projectile projectile)
        {
            if (++timer > 60)
                projectile.velocity *= 1.007f;
            return base.PreAI(projectile);
        }
    }
}
