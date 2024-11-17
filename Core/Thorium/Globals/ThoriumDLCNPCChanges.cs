using FargowiltasCrossmod.Core.Thorium.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.NPCs;

namespace FargowiltasCrossmod.Core.Thorium.Globals;

[ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
public class ThoriumDLCNPCChanges : GlobalNPC
{
    public override bool PreKill(NPC npc)
    {
        bool doDeviText = false;
        
        if (npc.type == ModContent.NPCType<GildedLycan>() && !ThoriumDLCWorldSavingSystem.downedGildedLycan)
        {
            ThoriumDLCWorldSavingSystem.downedGildedLycan = true;
            doDeviText = ThoriumDLCWorldSavingSystem.downedGildedBat && ThoriumDLCWorldSavingSystem.downedGildedSlime;
        }
        else if (npc.type == ModContent.NPCType<GildedSlime>() && !ThoriumDLCWorldSavingSystem.downedGildedSlime)
        {
            ThoriumDLCWorldSavingSystem.downedGildedSlime = true;
            doDeviText = ThoriumDLCWorldSavingSystem.downedGildedBat && ThoriumDLCWorldSavingSystem.downedGildedLycan;
        }
        else if (npc.type == ModContent.NPCType<GildedBat>() && !ThoriumDLCWorldSavingSystem.downedGildedBat)
        {
            ThoriumDLCWorldSavingSystem.downedGildedBat = true;
            doDeviText = ThoriumDLCWorldSavingSystem.downedGildedLycan && ThoriumDLCWorldSavingSystem.downedGildedSlime;
        }
        else if (npc.type == ModContent.NPCType<Myna>() && !ThoriumDLCWorldSavingSystem.downedMyna)
        {
            ThoriumDLCWorldSavingSystem.downedMyna = true;
            doDeviText = true;
        }
        
        if (doDeviText && Main.netMode != NetmodeID.Server)
        {
            Main.NewText("A new item has been unlocked in Deviantt's shop!", Color.HotPink);
        }
        
        return base.PreKill(npc);
    }
}