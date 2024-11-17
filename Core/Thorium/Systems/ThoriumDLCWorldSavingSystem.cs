using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Core.Thorium.Systems;

[ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
public class ThoriumDLCWorldSavingSystem : ModSystem
{
    public static bool downedGildedLycan = false;
    public static bool downedGildedSlime = false;
    public static bool downedGildedBat = false;
    public static bool downedMyna = false;

    public override void ClearWorld()
    {
        downedMyna = false;
        downedGildedBat = false;
        downedGildedSlime = false;
        downedGildedLycan = false;
        
        base.ClearWorld();
    }

    public override void NetSend(BinaryWriter writer)
    {
        BitsByte downedFlags = new();
        downedFlags[0] = downedGildedLycan;
        downedFlags[1] = downedGildedSlime;
        downedFlags[2] = downedGildedBat;
        downedFlags[3] = downedMyna;
        
        writer.Write(downedFlags);
    }

    public override void NetReceive(BinaryReader reader)
    {
        BitsByte downedFlags = reader.ReadByte();
        
        downedGildedLycan = downedFlags[0];
        downedGildedSlime = downedFlags[1];
        downedGildedBat = downedFlags[2];
        downedMyna = downedFlags[3];
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (WorldGen.generatingWorld) 
            return;

        var downed = new List<string>();
        if (downedGildedLycan) 
            downed.Add(nameof(downedGildedLycan));
        if (downedGildedSlime) 
            downed.Add(nameof(downedGildedSlime));
        if (downedGildedBat) 
            downed.Add(nameof(downedGildedBat));
        if (downedMyna) 
            downed.Add(nameof(downedMyna));
        tag["downedThorium"] = downed;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        var downed = tag.GetList<string>("downedThorium");
        downedGildedLycan = downed.Contains(nameof(downedGildedLycan));
        downedGildedSlime = downed.Contains(nameof(downedGildedSlime));
        downedGildedBat = downed.Contains(nameof(downedGildedBat));
        downedMyna = downed.Contains(nameof(downedMyna));
    }
}