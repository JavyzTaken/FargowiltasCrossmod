using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BrimstoneBurnPlayer : ModPlayer
    {
        public static readonly Color BrightColor = new(0.98f * 255, 0.79f * 255, 0.55f);
        public static readonly Color DarkColor = new(0.89f * 255, 0.31f * 255, 0.31f * 255);
        public FireParticleSet BrimstoneBurnEffectDrawer = new(-1, int.MaxValue, BrightColor, DarkColor, 10f, 0.65f);
        public float BurnFadeIntensity;
        public override void UpdateDead()
        {
            BurnFadeIntensity = 0f;
        }
        public override void PostUpdateMiscEffects()
        {
            if (!CalDLCWorldSavingSystem.E_EternityRev)
                return;

            BrimstoneBurnEffectDrawer.Update();
            BrimstoneBurnEffectDrawer.ParticleSpawnRate = int.MaxValue;
            int brimID = ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>();
            if (CalamityGlobalNPC.brimstoneElemental.IsWithinBounds(Main.maxNPCs) && Main.npc[CalamityGlobalNPC.brimstoneElemental] is NPC brimmy && brimmy.TypeAlive(brimID) && brimmy.TryGetDLCBehavior(out BrimstoneEternity brimmyEternity))
            {
                bool burning = Player.Distance(brimmyEternity.auraPos) > 220 * 4 && brimmyEternity.auraOpacity >= 1;
                if (burning)
                {
                    BrimstoneBurnEffectDrawer.ParticleSpawnRate = 1;
                }
                if (burning && BurnFadeIntensity < 1f)
                {
                    BurnFadeIntensity = MathHelper.Clamp(BurnFadeIntensity + 0.015f, 0f, 1f);
                }
                else if (!burning && BurnFadeIntensity > 0f)
                {
                    BurnFadeIntensity = MathHelper.Clamp(BurnFadeIntensity - 0.01f, 0f, 1f);
                }

            }
        }
    }
}
