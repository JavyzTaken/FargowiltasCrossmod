using CalamityMod;
using CalamityMod.Buffs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Systems;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCAddonGlobalNPC : GlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override bool InstancePerEntity => true;
        public int WulfrumScanned = -1;
        public int PBGDebuffTag = 0;
        public int taggedByPlayer = -1;

        public override void ResetEffects(NPC npc)
        {
            if (PBGDebuffTag > 0) PBGDebuffTag--;
        }
        //return time buff has left, -1 if doesnt have buff
        public static bool HasAnyDoTDebuff(NPC npc)
        {
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (HasDoTBuff(npc, i) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static int HasDoTBuff(NPC npc, int buffID)
        {
            if (!CalDLCSets.Buffs.DoTDebuff[buffID])
            {
                return -1;
            }
            else if (npc.HasBuff(buffID))
            {
                return npc.buffTime[npc.FindBuffIndex(buffID)];
            }
            if (buffID == ModContent.BuffType<Plague>() && npc.Calamity().pFlames > 0)
                return npc.Calamity().pFlames;
            if (buffID == ModContent.BuffType<BrainRot>() && npc.Calamity().brainRot > 0)
                return npc.Calamity().brainRot;
            if (buffID == ModContent.BuffType<BurningBlood>() && npc.Calamity().bBlood > 0)
                return npc.Calamity().bBlood;
            if (buffID == ModContent.BuffType<Nightwither>() && npc.Calamity().nightwither > 0)
                return npc.Calamity().nightwither;
            if (buffID == ModContent.BuffType<BanishingFire>() && npc.Calamity().banishingFire > 0)
                return npc.Calamity().banishingFire;
            if (buffID == ModContent.BuffType<BrimstoneFlames>() && npc.Calamity().bFlames > 0)
                return npc.Calamity().bFlames;
            if (buffID == ModContent.BuffType<VulnerabilityHex>() && npc.Calamity().vulnerabilityHex > 0)
                return npc.Calamity().vulnerabilityHex;
            if (buffID == ModContent.BuffType<GodSlayerInferno>() && npc.Calamity().gsInferno > 0)
                return npc.Calamity().gsInferno;
            if (buffID == ModContent.BuffType<HolyFlames>() && npc.Calamity().hFlames > 0)
                return npc.Calamity().hFlames;
            if (buffID == ModContent.BuffType<Dragonfire>() && npc.Calamity().dragonFire > 0)
                return npc.Calamity().dragonFire;
            if (buffID == ModContent.BuffType<AbsorberAffliction>() && npc.Calamity().absorberAffliction > 0)
                return npc.Calamity().absorberAffliction;
            if (buffID == ModContent.BuffType<AstralInfectionDebuff>() && npc.Calamity().astralInfection > 0)
                return npc.Calamity().astralInfection;
            if (buffID == ModContent.BuffType<SulphuricPoisoning>() && npc.Calamity().sulphurPoison > 0)
                return npc.Calamity().sulphurPoison;
            if (buffID == ModContent.BuffType<SagePoison>() && npc.Calamity().sagePoisonTime > 0)
                return npc.Calamity().sagePoisonTime;
            if (buffID == ModContent.BuffType<KamiFlu>() && npc.Calamity().kamiFlu > 0)
                return npc.Calamity().kamiFlu;
            if (buffID == ModContent.BuffType<CrushDepth>() && npc.Calamity().cDepth > 0)
                return npc.Calamity().cDepth;
            if (buffID == ModContent.BuffType<RiptideDebuff>() && npc.Calamity().rTide > 0)
                return npc.Calamity().rTide;
            if (buffID == ModContent.BuffType<Irradiated>() && npc.Calamity().irradiated > 0)
                return npc.Calamity().irradiated;
            if (buffID == ModContent.BuffType<MiracleBlight>() && npc.Calamity().miracleBlight > 0)
                return npc.Calamity().miracleBlight;
            if (buffID == ModContent.BuffType<ElementalMix>() && npc.Calamity().elementalMix > 0)
                return npc.Calamity().elementalMix;
            if (buffID == ModContent.BuffType<Vaporfied>() && npc.Calamity().vaporfied > 0)
                return npc.Calamity().vaporfied;
            return -1;
        }
        //Hardmode enchant. not in release.
        public override bool PreAI(NPC npc)
        {
            if (PBGDebuffTag > 0 && FargowiltasCrossmod.EnchantLoadingEnabled)
            {
                int distance = 300;
                if (taggedByPlayer >= 0 && Main.player[taggedByPlayer] != null && Main.player[taggedByPlayer].active && !Main.player[taggedByPlayer].dead)
                {
                    distance = 600;
                }
                foreach (NPC target in Main.ActiveNPCs)
                {

                    if (target != npc && target.Distance(npc.Center) < distance)
                    {

                        for (int i = 0; i < BuffLoader.BuffCount; i++)
                        {

                            if (HasDoTBuff(npc, i) >= 0 && HasDoTBuff(target, i) == -1)
                            {
                                target.AddBuff(i, HasDoTBuff(npc, i));
                            }
                        }
                    }
                }
            }
            if (npc.type == NPCID.KingSlime && BossRushEvent.BossRushActive && npc.GetLifePercent() <= 0.5f)
            {
                if (BossRushDialogueSystem.Phase < CalamityMod.Enums.BossRushDialoguePhase.TierOneComplete)
                    BossRushDialogueSystem.StartDialogue(CalamityMod.Enums.BossRushDialoguePhase.TierOneComplete);
                if (BossRushDialogueSystem.currentSequenceIndex == 2 && BossRushDialogueSystem.Phase == CalamityMod.Enums.BossRushDialoguePhase.TierOneComplete)
                {
                    Projectile.NewProjectileDirect(npc.GetSource_Death(), npc.Center + new Vector2(0, -800), new Vector2(0, 20), ModContent.ProjectileType<PenetratorThrown>(), 20000, 1, 0);
                    npc.StrikeInstantKill();
                }
            }
            return base.PreAI(npc);
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (WulfrumScanned >= 0 && HasAnyDoTDebuff(npc))
            {
                int DoTNormal = 35;
                int DoTForce = 100;
                Player owner = Main.player[Main.projectile[WulfrumScanned].owner];
                if (owner != null && owner.active && !owner.dead && owner.ForceEffect<WulfrumEffect>())
                {
                    npc.lifeRegen -= DoTForce;
                    if (damage < (int)(DoTForce / 10f))
                        damage = (int)(DoTForce / 10f);
                }
                else
                {
                    npc.lifeRegen -= DoTNormal;
                    if (damage < (int)(DoTNormal / 10f))
                        damage = (int)(DoTNormal / 10f);
                }

            }
            WulfrumScanned = -1;
        }
    }

}
