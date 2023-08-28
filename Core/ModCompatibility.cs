using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core;

// TODO: pick a good name before it's too late
public static class ModCompatibility
{
    public static class SoulsMod
    {
        public const string Name = "FargowiltasSouls";
    }
    public static class Calamity
    {
        // Please use this to avoid typo bugs
        public const string Name = "CalamityMod";

        // TODO: cache, lazy property
        public static bool Loaded => ModLoader.HasMod(Name);
    }
    public static class ThoriumMod
    {
        public const string Name = "ThoriumMod";
    }
}