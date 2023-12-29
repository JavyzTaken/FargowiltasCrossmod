/*
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using CalamityMod;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using FargowiltasCrossmod.Core.Utils;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.MoonLord
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathMLEye : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.MoonLordHand, NPCID.MoonLordHead);
        
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            NPC owner = Main.npc[(int)npc.ai[3]];
            EDeathMLCore ml = owner.GetGlobalNPC<EDeathMLCore>();
            if (item.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && owner.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 0 && !player.buffImmune[ModContent.BuffType<NullificationCurseBuff>()] && !WorldSavingSystem.SwarmActive)
                return false;
            if (!item.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && ml.roguePhase && owner.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4 && !player.buffImmune[ModContent.BuffType<NullificationCurseBuff>()] && !WorldSavingSystem.SwarmActive)
                return false;

            return base.CanBeHitByItem(npc, player, item);
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (projectile.owner < 0) return base.CanBeHitByProjectile(npc, projectile);
            NPC owner = Main.npc[(int)npc.ai[3]];
            EDeathMLCore ml = owner.GetGlobalNPC<EDeathMLCore>();
            if (projectile.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && !Main.player[projectile.owner].buffImmune[ModContent.BuffType<NullificationCurseBuff>()] && !WorldSavingSystem.SwarmActive)
            {
                if (owner.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4) return true;
                else return false;
            }
            if (!projectile.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && ml.roguePhase && owner.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4 && !Main.player[projectile.owner].buffImmune[ModContent.BuffType<NullificationCurseBuff>()] && !WorldSavingSystem.SwarmActive)
                return false;

            return base.CanBeHitByProjectile(npc, projectile);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            NPC owner = Main.npc[(int)npc.ai[3]];
            Player target = Main.player[npc.target];
            if (owner == null || !owner.active) return true;
            if (owner.GetGlobalNPC<EDeathMLCore>().roguePhase && owner.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4 && owner.GetGlobalNPC<MoonLordCore>().EnteredPhase2 == false)
            {
                if (Main.rand.NextBool(150) && DLCUtils.HostCheck && npc.type != NPCID.MoonLordHead)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 15, ModContent.ProjectileType<StarofDestruction>(), FargoSoulsUtil.ScaledProjectileDamage(200), 0);
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
*/