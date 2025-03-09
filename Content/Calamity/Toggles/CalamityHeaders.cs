using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Toggles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class GaleHeader : EnchantHeader
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override int Item => ModContent.ItemType<GaleForce>();
        public override float Priority => 0.91f;
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamitySoulHeader : SoulHeader
    {
        public override float Priority => 0.99f;
        public override int Item => ModContent.ItemType<Masochist>();
        
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class VagabondsSoulHeader : SoulHeader
    {
        public override float Priority => 5.11f;
        public override int Item => ModContent.ItemType<VagabondsSoul>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityColossusHeader : SoulHeader
    {
        public override float Priority => 5.12f;
        public override int Item => ModContent.ItemType<ColossusSoul>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityBerserkerHeader : SoulHeader
    {
        public override float Priority => 5.13f;
        public override int Item => ModContent.ItemType<BerserkerSoul>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityWizardHeader : SoulHeader
    {
        public override float Priority => 5.14f;
        public override int Item => ModContent.ItemType<ArchWizardsSoul>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamitySniperHeader : SoulHeader
    {
        public override float Priority => 5.15f;
        public override int Item => ModContent.ItemType<SnipersSoul>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityConjuristHeader : SoulHeader
    {
        public override float Priority => 5.16f;
        public override int Item => ModContent.ItemType<ConjuristsSoul>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityTrawlerHeader : SoulHeader
    {
        public override float Priority => 5.17f;
        public override int Item => ModContent.ItemType<TrawlerSoul>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BrandoftheBrimstoneWitchHeader : MasoHeader
    {
        public override float Priority => 6.1f;
        public override int Item => ModContent.ItemType<BrandoftheBrimstoneWitch>();
    }
    /*
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityToggles : ToggleCollection
    {
        public override string Mod => "FargowiltasCrossmod";
        public override string SortCategory => "Enchantments";
        public override int Priority => 1;
        public override bool Active => true;

        public int VagabondsSoulHeader = ModContent.ItemType<VagabondsSoul>();
        public string Nanotech;
        public string EclipseMirror;
        public string DragonScales;
        public string VeneratedLocket;

        public int CalamityColossusHeader = ModContent.ItemType<ColossusSoul>();
        public string Amalgam;
        public string AsgardianAegis;
        public string RampartofDeities;

        public int CalamityBerserkerHeader = ModContent.ItemType<BerserkerSoul>();
        public string ElementalGauntlet;

        public int CalamityWizardHeader = ModContent.ItemType<ArchWizardsSoul>();
        public string EtherealTalisman;

        public int CalamitySniperHeader = ModContent.ItemType<SnipersSoul>();
        public string ElementalQuiver;
        public string QuiverofNihility;
        public string DynamoStemCells;

        public int CalamityConjuristHeader = ModContent.ItemType<ConjuristsSoul>();
        public string Nucleogenesis;

        public int CalamityTrawlerHeader = ModContent.ItemType<TrawlerSoul>();
        public string AbyssalDivingSuit;

        public int BrandoftheBrimstoneWitchHeader = ModContent.ItemType<BrandoftheBrimstoneWitch>();
        public string HeartoftheElements;
        public string OccultSkullCrown;
        public string Purity;
        public string TheSponge;
        public string ChaliceOfTheBloodGod;
        public string NebulousCore;
        public string YharimsGift;
        public string DraedonsHeart;
        public string Calamity;
    }
    */
}
