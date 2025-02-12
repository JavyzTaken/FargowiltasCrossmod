using CalamityMod.Events;
using CalamityMod.Projectiles.Typeless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BossRushMusicChange : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            return BossRushEvent.BossRushActive;
        }
        public override float GetWeight(Player player)
        {
            return 1f;
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh + 1;
        public override int Music {
            get
            {
                int tier = 1;
                if (BossRushEvent.BossRushStage > 0)
                {
                    tier = 3;
                }
                if (BossRushEvent.BossRushStage > 6)
                {
                    tier = 4;
                }
                return ModContent.GetInstance<CalamityMod.CalamityMod>().GetMusicFromMusicMod($"BossRushTier{tier}") ?? 0;
            }
        }
    }
}
