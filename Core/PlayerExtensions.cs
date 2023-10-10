using Terraria;

namespace FargowiltasCrossmod.Core
{
    public static class PlayerHelper
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
}