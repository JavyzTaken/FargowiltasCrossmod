using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class FearValkyrieBuff : ModBuff
    {
        public override string Texture => "CalamityMod/Buffs/Summon/CorvidHarbringerBuff";
        public override void SetStaticDefaults()
        {

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FearValkyrie>()] > 0)
            {
                SBDPlayer.aScarey = true;

            }
            if (!SBDPlayer.aScarey)
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