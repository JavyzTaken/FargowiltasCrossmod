using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.World;
using CalamityMod;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using CalamityMod.Items;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls;
using Luminance.Core.Hooking;
using Fargowiltas.NPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Content.Projectiles;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Ranged;
using Fargowiltas.Items;
using CalamityMod.Items.Potions;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using CalamityMod.CalPlayer;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Systems;
using CalamityMod.Enums;
using Fargowiltas.Items.Vanity;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.UI.Elements;
using Terraria.Localization;
using CalamityMod.Skies;
using Terraria.Graphics.Effects;
using FargowiltasSouls.Content.UI;
using CalamityMod.NPCs.Perforator;
using FargowiltasCrossmod.Content.Calamity.Bosses.Perforators;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.WrathoftheGods.Name)]
    [JITWhenModsEnabled(ModCompatibility.WrathoftheGods.Name)]
    public class WrathDLCDetours : ModSystem, ICustomDetourProvider
    {

        public delegate bool Orig_KillOldDukeWrapper(object self, NPC npc);

        public override void Load()
        {
            
        }
        void ICustomDetourProvider.ModifyMethods()
        {
            var KillOldDukeWrapper_Method = ModCompatibility.WrathoftheGods.Mod.Find<ModSystem>("FUCKYOUOLDDUKESystem").GetType().GetMethod("KillOldDukeWrapper", LumUtils.UniversalBindingFlags);
            HookHelper.ModifyMethodWithDetour(KillOldDukeWrapper_Method, KillOldDukeWrapper_Detour);
        }
        internal static bool KillOldDukeWrapper_Detour(Orig_KillOldDukeWrapper orig, object self, NPC npc)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
                return true;
            return orig(self, npc);
        }
    }
}