using FargowiltasSouls.Core.ItemDropRules.Conditions;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Items.Consumable;
using ThoriumMod.Items.Depths;
using ThoriumMod.Items.ThrownItems;
using ThoriumMod.NPCs;
using ThoriumMod.NPCs.Depths;

namespace FargowiltasCrossmod.Core.Thorium.Globals;

public class ThoriumDLCNPCDrops : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        void TimsConcoctionDrop(IItemDropRule rule)
        {
            TimsConcoctionDropCondition dropCondition = new();
            IItemDropRule conditionalRule = new LeadingConditionRule(dropCondition);
            conditionalRule.OnSuccess(rule);
            npcLoot.Add(conditionalRule);
        }

        if (npc.type == ModContent.NPCType<GildedBat>() || npc.type == ModContent.NPCType<GildedSlime>() || npc.type == ModContent.NPCType<GildedLycan>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<GlowingPotion>(), 1, 1, 6));
        }

        if (npc.type == NPCID.Pixie)
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<HolyPotion>(), 1, 0, 3));
        }
        if (npc.type == ModContent.NPCType<Hammerhead>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<AquaPotion>(), 1, 1, 6));
        }
        if (npc.type == ModContent.NPCType<Spectrumite>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<ArcanePotion>(), 1, 1, 6));
        }
        if (npc.type == ModContent.NPCType<MoltenMortar>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<ArtilleryPotion>(), 1, 1, 3));
        }
        if (npc.type == ModContent.NPCType<AncientArcher>() || npc.type == ModContent.NPCType<AncientCharger>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<AssassinPotion>(), 1, 1, 3));
        }
        if (npc.type == ModContent.NPCType<LifeCrystalMimic>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<BloodPotion>(), 1, 1, 8));
        }
        if (npc.type == ModContent.NPCType<BoneFlayer>() || npc.type == ModContent.NPCType<InfernalHound>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<ConflagrationPotion>(), 1, 1, 3));
        }
        if (npc.type == ModContent.NPCType<UnderworldPot1>() || npc.type == ModContent.NPCType<UnderworldPot2>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<BouncingFlamePotion>(), 1, 1, 6));
        }
        if (npc.type == ModContent.NPCType<GoblinDrummer>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<CreativityPotion>(), 1, 1, 6));
        }
        if (npc.type == NPCID.GiantWormHead || npc.type == NPCID.DiggerHead)
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<EarwormPotion>(), 1, 1, 6));
        }
        if (npc.type == ModContent.NPCType<HellBringerMimic>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<WarmongerPotion>(), 1, 1, 3));
        }
        if (npc.type == ModContent.NPCType<Barracuda>() || npc.type == ModContent.NPCType<Sharptooth>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<FrenzyPotion>(), 1, 1, 6));
        }
        if (npc.type == ModContent.NPCType<SnowBall>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<HydrationPotion>(), 1, 1, 3));
        }
        if (npc.type == ModContent.NPCType<SeaShantySinger>())
        {
            TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<InspirationReachPotion>(), 1, 1, 6));
        }
    }
}