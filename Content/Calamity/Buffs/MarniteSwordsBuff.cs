using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Content.Calamity.Projectiles;


namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteSwordsBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MarniteSword>()] > 0)
            {
                SBDPlayer.aSword = true;

            }
            if (!SBDPlayer.aSword)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}