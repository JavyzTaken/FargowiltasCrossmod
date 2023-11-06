using Terraria;
using FargowiltasCrossmod.Content.Thorium;
using Terraria.ModLoader;

namespace FargowiltasCrossmod
{
    public static class DLCExtensionMethods
    {
        public static void KillOwnedProjectilesOfType(this Player player, int type)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].Kill();
                }
            }
        }

    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public static class DLCThoriumExtensionMethods
    {
        public static CrossplayerThorium ThoriumDLC(this Player player) => player.GetModPlayer<CrossplayerThorium>();
        public static ThoriumMod.ThoriumPlayer Thorium(this Player player) => player.GetModPlayer<ThoriumMod.ThoriumPlayer>();
    }
}