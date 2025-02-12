using CalamityMod.Items;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Luminance.Core.Hooking;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using CalamityMod.BiomeManagers;
using CalamityMod.World;
using FargowiltasSouls.Core.Systems;
using static FargowiltasCrossmod.Core.Calamity.Systems.CalDLCDetours;
using Fargowiltas.Common.Configs;
using Terraria.ModLoader;
using CalamityMod;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCFountainDetours : ICustomDetourProvider
    {
        private static readonly MethodInfo AstralInfectionActiveMethod = typeof(AstralInfectionBiome).GetMethod("IsBiomeActive", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo BrimstoneCragsActiveMethod = typeof(BrimstoneCragsBiome).GetMethod("IsBiomeActive", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo SulphurousSeaActiveMethod = typeof(SulphurousSeaBiome).GetMethod("IsBiomeActive", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo SunkenSeaActiveMethod = typeof(SunkenSeaBiome).GetMethod("IsBiomeActive", LumUtils.UniversalBindingFlags);


        public delegate bool Orig_AstralInfectionActive(AstralInfectionBiome self, Player player);
        public delegate bool Orig_BrimstoneCragsActive(BrimstoneCragsBiome self, Player player);
        public delegate bool Orig_SulphurousSeaActive(SulphurousSeaBiome self, Player player);
        public delegate bool Orig_SunkenSeaActive(SunkenSeaBiome self, Player player);

        void ICustomDetourProvider.ModifyMethods()
        {
            HookHelper.ModifyMethodWithDetour(AstralInfectionActiveMethod, AstralInfectionActive_Detour);
            HookHelper.ModifyMethodWithDetour(BrimstoneCragsActiveMethod, BrimstoneCragsActive_Detour);
            HookHelper.ModifyMethodWithDetour(SulphurousSeaActiveMethod, SulphurousSeaActive_Detour);
            HookHelper.ModifyMethodWithDetour(SunkenSeaActiveMethod, SunkenSeaActive_Detour);
        }
        internal static bool AstralInfectionActive_Detour(Orig_AstralInfectionActive orig, AstralInfectionBiome self, Player player)
        {
            bool result = orig(self, player);
            if (FargoServerConfig.Instance.Fountains && Main.SceneMetrics.ActiveFountainColor == ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater").Slot && Main.hardMode)
                return true;
            return result;
        }
        internal static bool BrimstoneCragsActive_Detour(Orig_BrimstoneCragsActive orig, BrimstoneCragsBiome self, Player player)
        {
            bool result = orig(self, player);
            if (FargoServerConfig.Instance.Fountains && player.Calamity().BrimstoneLavaFountainCounter > 0)
                return true;
            return result;
        }
        internal static bool SulphurousSeaActive_Detour(Orig_SulphurousSeaActive orig, SulphurousSeaBiome self, Player player)
        {
            bool result = orig(self, player);
            string waterColor = Main.zenithWorld ? "CalamityMod/PissWater" : "CalamityMod/SulphuricWater";
            if (FargoServerConfig.Instance.Fountains && Main.SceneMetrics.ActiveFountainColor == ModContent.Find<ModWaterStyle>(waterColor).Slot)
                return true;
            return result;
        }
        internal static bool SunkenSeaActive_Detour(Orig_SunkenSeaActive orig, SunkenSeaBiome self, Player player)
        {
            bool result = orig(self, player);
            if (FargoServerConfig.Instance.Fountains && Main.SceneMetrics.ActiveFountainColor == ModContent.Find<ModWaterStyle>("CalamityMod/SunkenSeaWater").Slot)
                return true;
            return result;
        }
    }
}
