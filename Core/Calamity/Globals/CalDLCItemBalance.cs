using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Weapons.FinalUpgrades;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Patreon.Volknet;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCItemBalance : GlobalItem
    {
        public static float BalanceChange(Item item)
        {

            if (item.type == ItemType<MechanicalLeashOfCthulhu>())
                return 0.5f;
            if (item.type == ItemType<Blender>())
                return 2f;
            if (item.type == ItemType<NukeFishron>() || item.type == ItemType<GolemTome2>() || item.type == ItemType<DestroyerGun2>())
                return 2f;

            if (item.type == ItemType<UmbraRegalia>())
                return 2f;
            if (item.type == ItemType<GeminiGlaives>())
                return 2f;
            if (DLCSets.GetValue(DLCSets.Items.AbomTierFargoWeapon, item.type))
                return 1.5f;
            if (DLCSets.GetValue(DLCSets.Items.ChampionTierFargoWeapon, item.type))
                return 0.8f;

            if (item.type == ItemType<Ataraxia>())
                return 0.65f;

            //Shadowspec items and such
            // Melee
            if (item.type == ItemType<IridescentExcalibur>()) return 0.6f;
            if (item.type == ItemType<IllustriousKnives>()) return 1f;
            if (item.type == ItemType<NanoCore>() && item.DamageType.CountsAsClass(DamageClass.Melee)) return 2f;
            if (item.type == ItemType<Azathoth>()) return 1f;
            if (item.type == ItemType<RedSun>()) return 0.85f;
            if (item.type == ItemType<GaelsGreatsword>()) return 0.75f;
            // Ranged
            if (item.type == ItemType<SomaPrime>()) return 1.2f;
            if (item.type == ItemType<Svantechnical>()) return 1.1f;
            if (item.type == ItemType<Voidragon>()) return 1.1f;
            // Magic
            if (item.type == ItemType<Apotheosis>()) return 0.75f;
            if (item.type == ItemType<StaffofBlushie>()) return 1f;
            if (item.type == ItemType<Eternity>()) return 0.7f;
            if (item.type == ItemType<TheDanceofLight>()) return 0.8f;
            if (item.type == ItemType<RainbowPartyCannon>()) return 0.7f;
            if (item.type == ItemType<Fabstaff>()) return 1.2f;
            // Summoner
            if (item.type == ItemType<AngelicAlliance>()) return 0.2f;
            if (item.type == ItemType<FlamsteedRing>()) return 0.45f;
            if (item.type == ItemType<TemporalUmbrella>()) return 0.35f;
            if (item.type == ItemType<Endogenesis>()) return 0.35f;
            if (item.type == ItemType<Metastasis>()) return 0.5f;
            if (item.type == ItemType<UniverseSplitter>()) return 0.5f;
            if (item.type == ItemType<ProfanedSoulCrystal>()) return 0.4f;
            // Rogue
            if (item.type == ItemType<NanoblackReaper>()) return 0.4f;
            if (item.type == ItemType<ScarletDevil>()) return 0.4f;
            if (item.type == ItemType<TheAtomSplitter>()) return 0.75f;
            if (item.type == ItemType<Sacrifice>()) return 0.75f;

            //Post-Mutant items
            if (item.type == ItemType<PhantasmalLeashOfCthulhu>()) return 0.5f;
            if (item.type == ItemType<GuardianTome>()) return 0.2f;
            if (item.type == ItemType<SlimeRain>()) return 0.08f;
            if (item.type == ItemType<TheBiggestSting>()) return 0.3f;

            return 1;

        }

        public override void SetDefaults(Item item)
        {
            //Progression balance changes
            float balance = BalanceChange(item);
            if (balance != 1)
            {
                item.damage = (int)(item.damage * balance);
            }
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            switch (item.type)
            {
                //magic dagger not using system because needs to dynamically change and change shootspeed (setdefaults doesnt allow dynamic change)
                case ItemID.MagicDagger:
                    if (!Main.hardMode)
                    {
                        damage *= 0.51f;
                        item.shootSpeed = 12;
                    }
                    else
                    {
                        item.shootSpeed = 30;
                    }
                    break;
                case ItemID.CobaltSword:
                case ItemID.PalladiumSword:
                case ItemID.MythrilSword:
                case ItemID.OrichalcumSword:
                    {
                        player.FargoSouls().AttackSpeed /= 1.5f;
                    }
                    break;
            }
            if (item.type == ItemID.OrichalcumSword || item.type == ItemID.OrichalcumHalberd)
            {
                damage *= 0.725f;
            }
        }
        #region Tooltips
        public enum EModeChange
        {
            None,
            Nerf,
            Buff,
            Neutral
        }
        //Identical to Eternity nerfs from Souls Mod
        void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, int amount = 0, string mod = "FargowiltasSouls")
        {
            string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
            string nerf = Language.GetTextValue($"Mods.{mod}.EModeBalance.{key}", amount == 0 ? null : amount);
            tooltips.Add(new TooltipLine(Mod, $"{change}{key}", $"{prefix}{nerf}"));
        }

        void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, string extra, string mod = "FargowiltasSouls")
        {

            string prefix = Language.GetTextValue($"Mods.{mod}.EModeBalance.{change}");
            string nerf = Language.GetTextValue($"Mods.{mod}.EModeBalance.{key}");
            tooltips.Add(new TooltipLine(Mod, $"{change}{key}", $"{prefix} {nerf} {extra}"));
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            string BalanceLine = Language.GetTextValue($"Mods.FargowiltasCrossmod.EModeBalance.CrossBalanceGeneric");

            if (WorldSavingSystem.EternityMode)
            {
                if (item.type == ItemType<NormalityRelocator>())
                    ItemBalance(tooltips, EModeChange.Nerf, "RodofDiscord");
                if (item.type == ItemType<Laudanum>())
                    ItemBalance(tooltips, EModeChange.Nerf, "Laudanum", mod: "FargowiltasCrossmod");
                if (item.type == ItemType<OceanCrest>() || item.type == ItemType<AquaticEmblem>())
                    ItemBalance(tooltips, EModeChange.Buff, "OceanCrest", mod: "FargowiltasCrossmod");
            }

            float balance = BalanceChange(item);
            string BalanceUpLine = $"[c/00A36C:{BalanceLine}]";
            string BalanceDownLine = $"[c/FF0000:{BalanceLine}]";

            static string BalanceTooltips(string key) => Language.GetTextValue($"Mods.FargowiltasCrossmod.EModeBalance.{key}");
            //void Buff(string key) => tooltips.Add(new TooltipLine(Mod, "BalanceUp", $"{BalanceUpLine}" + BalanceTooltips(key)));
            void NerfTooltip(string key) => tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}" + BalanceTooltips(key)));

            if (balance > 1)
                tooltips.Add(new TooltipLine(Mod, "DamageUp", $"{BalanceUpLine}" + Language.GetText($"Mods.FargowiltasCrossmod.EModeBalance.DamageUpGeneric").Format(Math.Round((balance - 1) * 100))));
            else if (balance < 1)
                tooltips.Add(new TooltipLine(Mod, "DamageDown", $"{BalanceDownLine}" + Language.GetText($"Mods.FargowiltasCrossmod.EModeBalance.DamageDownGeneric").Format(Math.Round((1 - balance) * 100))));

            if (item.type == ItemID.MagicDagger)
                NerfTooltip("MagicDagger");

            if (item.type == ItemType<ProfanedSoulCrystal>())
                NerfTooltip("ProfanedCrystal");

            if (item.type == ItemType<MythrilEnchant>())
                NerfTooltip("MythrilEnch");

            if (item.type == ItemType<OrichalcumEnchant>())
                NerfTooltip("OrichalcumEnch");

            if (item.type == ItemType<DaawnlightSpiritOrigin>())
                NerfTooltip("Daawnlight");

            if (item.type == ItemType<SlimyShield>())
                NerfTooltip("SlimyShield");

            if (item.ModItem != null && item.ModItem is FlightMasteryWings && item.ModItem is not EternitySoul)
                NerfTooltip("FlightMastery");

            if (item.type == ItemType<LifeForce>())
                NerfTooltip("LifeForce");

            if (item.type is ItemID.CobaltSword or ItemID.PalladiumSword or ItemID.OrichalcumSword or ItemID.MythrilSword or ItemID.OrichalcumHalberd)
                NerfTooltip("HardmodeSwords");
            if (item.type == ItemID.ReaverShark)
                tooltips.Add(new TooltipLine(Mod, "PPDown", $"{BalanceDownLine}" + Language.GetText($"Mods.FargowiltasCrossmod.EModeBalance.PickPowerDownGeneric").Format(41)));

            CalamityGlobalItem calItem = item.GetGlobalItem<CalamityGlobalItem>();
            if (!item.IsAir && calItem.AppliedEnchantment.HasValue)
                if (calItem.AppliedEnchantment.Value.ID == 1000)
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown_HealEnchant", $"{BalanceDownLine}" + BalanceTooltips("CalEnch")));
        }
        #endregion
    }
}
