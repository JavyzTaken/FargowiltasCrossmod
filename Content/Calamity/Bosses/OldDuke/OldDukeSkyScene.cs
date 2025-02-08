using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class OldDukeSkyScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override float GetWeight(Player player) => 0.7f;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<CalamityMod.NPCs.OldDuke.OldDuke>()) && CalDLCWorldSavingSystem.E_EternityRev;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Filters.Scene[OldDukeSky.SkyKey] = new Filter(new ScreenShaderData("FilterMiniTower").UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance[OldDukeSky.SkyKey] = new OldDukeSky();
                SkyManager.Instance[OldDukeSky.SkyKey].Load();
            }
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals(OldDukeSky.SkyKey, isActive);
        }
    }
}
