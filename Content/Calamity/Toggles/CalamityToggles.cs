using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Toggles
{
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
}
