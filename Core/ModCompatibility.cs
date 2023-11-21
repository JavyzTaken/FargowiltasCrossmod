using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core;

public static class ModCompatibility
{
    public static class SoulsMod
    {
        public const string Name = "FargowiltasSouls";
        public static bool Loaded => ModLoader.HasMod(Name);
        public static Mod Mod => ModLoader.GetMod(Name);
    }
    public static class MutantMod
    {
        public const string Name = "Fargowiltas";
        public static bool Loaded => ModLoader.HasMod(Name);
        public static Mod Mod => ModLoader.GetMod(Name);
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
}