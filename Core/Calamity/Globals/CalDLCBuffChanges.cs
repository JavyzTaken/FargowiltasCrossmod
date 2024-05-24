using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCBuffChanges : GlobalBuff
    {
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            /*
            if (type == ModContent.BuffType<JammedBuff>())
            {
                tip += ("\nAlso applies to Rogue weapons");
            }
            */
            /*
            if (type == BuffID.Warmth && DLCCalamityConfig.Instance.BalanceRework)
            {
                string immunityLine = "\n" + CalamityUtils.GetTextValue("Vanilla.BuffDescription.WarmthExtra");
                if (tip.Contains(immunityLine))
                    tip.Replace(immunityLine, "");
            }
            */
        }
        public override void Update(int type, Player player, ref int buffIndex)
        {
            //Removes buff immunity to given buff ID granted by buffs
            /*
            void PrebuffImmune(int buffID)
            {
                CrossplayerCalamity cdlcPlayer = player.CalamityDLC();
                if (cdlcPlayer.PreUpdateBuffImmune != null && buffID.IsWithinBounds(cdlcPlayer.PreUpdateBuffImmune.Length))
                {
                    player.buffImmune[buffID] = cdlcPlayer.PreUpdateBuffImmune[buffID];
                }
                else
                    player.buffImmune[buffID] = false;
            }
            */
            /*
            if (type == BuffID.Warmth && Core.Calamity.DLCCalamityConfig.Instance.BalanceRework)
            {
                PrebuffImmune(ModContent.BuffType<GlacialState>());
                PrebuffImmune(BuffID.Frozen);
                PrebuffImmune(BuffID.Chilled);
            }
            */
        }
    }
}
