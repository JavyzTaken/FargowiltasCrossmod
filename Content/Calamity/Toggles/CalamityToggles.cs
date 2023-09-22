using CalamityMod.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.Toggler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FargowiltasCrossmod.Content.Calamity.Toggles
{
    [ExtendsFromMod("CalamityMod")]
    [JITWhenModsEnabled("CalamityMod")]
    public class CalamityToggles : ToggleCollection
    {
        public override string Mod => "CalamityMod";
        public override string SortCatagory => "Enchantments";
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
        public string Purity;

        public int CalamityBerserkerHeader = ModContent.ItemType<BerserkerSoul>();
        public string ElementalGauntlet;

        public int CalamityWizardHeader = ModContent.ItemType<ArchWizardsSoul>();
        public string EtherealTalisman;

        public int CalamitySniperHeader = ModContent.ItemType<SnipersSoul>();
        public string ElementalQuiver;
        public string DaawnlightSpiritOrigin;
        public string QuiverofNihility;
        public string DynamoStemCells;

        public int CalamityConjuristHeader = ModContent.ItemType<ConjuristsSoul>();
        public string Nucleogenesis;

        public int CalamityTrawlerHeader = ModContent.ItemType<TrawlerSoul>();
        public string AbyssalDivingSuit;

        public int AncientSoulHeader = ModContent.ItemType<AncientsSoul>();
        public string DimensionalSoulArtifact;
        public string EldritchSoulArtifact;
        public string ProfanedSoulArtifact;
        public string AuricSoulArtifact;
        public string PhantomicArtifact;
        public string DarkSunRing;

        public int TyrantSoulHeader = ModContent.ItemType<TyrantSoul>();
        public string HeartoftheElements;
        public string OccultSkullCrown;
        public string NebulousCore;
        public string YharimsGift;
        public string DraedonsHeart;
        public string Calamity;

        public int AncestralCharmHeader = ModContent.ItemType<AncestralCharm>();
        public string TrinketofChi;
        public string LuxorsGift;
        public string GladiatorsLocket;
        public string FungalSymbiote;
        public string UnstableGraniteCore;

        public int ElementalOpalHeader = ModContent.ItemType<ElementalOpal>();
        public string AeroStone;
        public string CryoStone;
        public string ChaosStone;
        public string BloomStone;

        public int PolarThingHeader = ModContent.ItemType<PolarThing>();
        public string OceanCrest;
        public string FungalClump;
        public string RottenBrain;
        public string BloodyWormTooth;
        public string ManaPolarizer;

        public int PlastralHideHeader = ModContent.ItemType<PlastralHide>();
        public string FrostFlare;
        public string AquaticEmblem;
        public string VoidofExtinction;
        public string LeviathanAmbergris;
        public string GravistarSabaton;
        public string ToxicHeart;
        public string HideofAstrumDeus;

        public int VoidIconHeader = ModContent.ItemType<VoidIcon>();
        public string WarbanneroftheSun;
        public string BlazingCore;
        public string SpectralVeil;
        public string TheEvolution;
        public string Affliction;
        public string MutatedTruffle;

        public int SomethingMaliciousHeader = ModContent.ItemType<SomethingMalicious>();
        public string EvasionScarf;
        public string TheTransformer;
        public string Regenator;
        public string FlameLickedShell;
        public string DeepDiver;
    }
}
