using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core;

public static class ModCompatibility
{
    public static class MutantMod
    {
        public const string Name = "Fargowiltas";
        public static bool Loaded => ModLoader.HasMod(Name);
        public static Mod Mod => ModLoader.GetMod(Name);
    }
    public static class SoulsMod
    {
        public const string Name = "FargowiltasSouls";
        public static bool Loaded => ModLoader.HasMod(Name);
        public static FargowiltasSouls.FargowiltasSouls Mod => ModLoader.GetMod(Name) as FargowiltasSouls.FargowiltasSouls;
    }
    public static class Calamity
    {
        // Please use this to avoid typo bugs
        public const string Name = "CalamityMod";

        // TODO: cache, lazy property
        public static bool Loaded => ModLoader.HasMod(Name);

        public static Mod Mod => ModLoader.GetMod(Name);
    }
    public static class ThoriumMod
    {
        public const string Name = "ThoriumMod";
        public static bool Loaded => ModLoader.HasMod(Name);
        public static Mod Mod => ModLoader.GetMod(Name);
    }
    public static class InfernumMode
    {
        public const string Name = "InfernumMode";
        public static bool Loaded => ModLoader.HasMod(Name);
        public static Mod Mod => ModLoader.GetMod(Name);
        public static bool InfernumDifficulty => (bool)Mod.Call("GetInfernumActive");
    }
    public static class WrathoftheGods
    {
        public const string Name = "NoxusBoss";
        public static bool Loaded => ModLoader.HasMod(Name);
        public static Mod Mod => ModLoader.GetMod(Name);

        public static ModNPC NoxusBoss1 = Mod.Find<ModNPC>("NoxusEgg");
        public static ModNPC NoxusBoss2 = Mod.Find<ModNPC>("EntropicGod");
        public static ModNPC NamelessDeityBoss = Mod.Find<ModNPC>("NamelessDeityBoss");
    }
    public static class BossChecklist
    {
        public static void AdjustValues()
        {
            if (Calamity.Loaded)
            {
                foreach (var entry in SoulsMod.Mod.BossChecklistValues)
                {
                    if (entry.Key.Contains("Champion"))
                    {
                        SoulsMod.Mod.BossChecklistValues[entry.Key] += 1f;
                    }
                }
                SoulsMod.Mod.BossChecklistValues["AbomBoss"] = 22.6f;
                SoulsMod.Mod.BossChecklistValues["MutantBoss"] = 25.8f;
            }
        }
    }
}