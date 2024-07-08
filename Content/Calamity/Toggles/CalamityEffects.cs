using CalamityMod.Items.Accessories;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Toggles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AbyssalDivingSuitEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TrawlerHeader>();
        public override int ToggleItemType => ModContent.ItemType<AbyssalDivingSuit>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public abstract class UniverseEffect : AccessoryEffect 
    { 
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>(); 
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NucleogenesisEffect : UniverseEffect
    {
        public override int ToggleItemType => ModContent.ItemType<Nucleogenesis>();
        public override bool IgnoresMutantPresence => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ElementalQuiverEffect : UniverseEffect
    {
        public override int ToggleItemType => ModContent.ItemType<ElementalQuiver>();
        public override bool IgnoresMutantPresence => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class QuiverofNihilityEffect : UniverseEffect
    {
        public override int ToggleItemType => ModContent.ItemType<QuiverofNihility>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ElementalGauntletEffect : UniverseEffect
    {
        public override int ToggleItemType => ModContent.ItemType<ElementalGauntlet>();
        public override bool IgnoresMutantPresence => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EtherealTalismanEffect : UniverseEffect
    {
        public override int ToggleItemType => ModContent.ItemType<EtherealTalisman>();
        public override bool IgnoresMutantPresence => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public abstract class ColossusEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AmalgamEffect : ColossusEffect
    {
        public override int ToggleItemType => ModContent.ItemType<TheAmalgam>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AsgardianAegisEffect : ColossusEffect
    {
        public override int ToggleItemType => ModContent.ItemType<AsgardianAegis>();
        public override bool IgnoresMutantPresence => true;
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class RampartofDeitiesEffect : ColossusEffect
    {
        public override int ToggleItemType => ModContent.ItemType<RampartofDeities>();
        public override bool IgnoresMutantPresence => true;
    }
}
