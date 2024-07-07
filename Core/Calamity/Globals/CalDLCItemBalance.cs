using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Ammos;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Patreon.DemonKing;
using FargowiltasSouls.Content.Patreon.Duck;
using FargowiltasSouls.Content.Patreon.GreatestKraken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using FargowiltasSouls.Content.Items.Weapons.FinalUpgrades;
using Terraria.ID;
using Terraria;
using FargowiltasSouls.Content.Items.Armor;
using CalamityMod;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.Localization;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using CalamityMod.UI.CalamitasEnchants;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCItemBalance : GlobalItem
    {

        public static float BalanceChange(Item item)
        {

            if (item.type == ModContent.ItemType<MechanicalLeashOfCthulhu>())
                return 0.5f;
            if (item.type == ModContent.ItemType<Blender>())
                return 1f;
            if (item.type == ModContent.ItemType<NukeFishron>() || item.type == ModContent.ItemType<GolemTome2>() || item.type == ModContent.ItemType<DestroyerGun2>() || item.type == ModContent.ItemType<RefractorBlaster2>())
                return 2f;
            if (DLCSets.GetValue(DLCSets.Items.AbomTierFargoWeapon, item.type))
                return 1.5f;
            if (DLCSets.GetValue(DLCSets.Items.ChampionTierFargoWeapon, item.type))
            {
                return 0.8f;
            }

            //Shadowspec items
            if (item.type == ModContent.ItemType<IridescentExcalibur>()) return 0.5f;
            if (item.type == ModContent.ItemType<IllustriousKnives>()) return 0.8f;
            if (item.type == ModContent.ItemType<Azathoth>()) return 0.9f;
            if (item.type == ModContent.ItemType<RedSun>()) return 1.5f;
            if (item.type == ModContent.ItemType<SomaPrime>()) return 1.2f;
            if (item.type == ModContent.ItemType<Svantechnical>()) return 1.1f;
            if (item.type == ModContent.ItemType<Voidragon>()) return 1.1f;
            if (item.type == ModContent.ItemType<StaffofBlushie>()) return 0.7f;
            if (item.type == ModContent.ItemType<Eternity>()) return 0.4f;
            if (item.type == ModContent.ItemType<TheDanceofLight>()) return 0.5f;
            if (item.type == ModContent.ItemType<RainbowPartyCannon>()) return 0.6f;
            if (item.type == ModContent.ItemType<NanoblackReaper>()) return 0.4f;
            if (item.type == ModContent.ItemType<ScarletDevil>()) return 0.4f;
            if (item.type == ModContent.ItemType<TemporalUmbrella>()) return 0.35f;
            if (item.type == ModContent.ItemType<Endogenesis>()) return 0.35f;
            if (item.type == ModContent.ItemType<UniverseSplitter>()) return 0.5f;
            if (item.type == ModContent.ItemType<Metastasis>()) return 0.5f;
            if (item.type == ModContent.ItemType<FlamsteedRing>()) return 0.45f;
            if (item.type == ModContent.ItemType<AngelicAlliance>()) return 0.2f;
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>()) return 0.4f;
            if (item.type == ModContent.ItemType<Fabstaff>()) return 0.6f;

            //Post-Mutant items
            if (item.type == ModContent.ItemType<PhantasmalLeashOfCthulhu>()) return 0.2f;
            if (item.type == ModContent.ItemType<GuardianTome>()) return 0.2f;
            if (item.type == ModContent.ItemType<SlimeRain>()) return 0.08f;
            if (item.type == ModContent.ItemType<TheBiggestSting>()) return 0.3f;

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
        public static float TrueMeleeTungstenScaleNerf(Player player)
        {
            FargoSoulsPlayer soulsPlayer = player.FargoSouls();
            return player.HasEffect<TungstenEffect>() && soulsPlayer.ForceEffect<TungstenEnchant>() ? 1.5f : 1.5f;
        }
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            FargoSoulsPlayer soulsPlayer = player.FargoSouls();

            #region Tungsten balance changes/fixes
            if (player.HasEffect<TungstenEffect>() &&
                    !item.IsAir && item.damage > 0 && (!item.noMelee || FargoGlobalItem.TungstenAlwaysAffects.Contains(item.type)) && item.pick == 0 && item.axe == 0 && item.hammer == 0)
            {
                if (DLCSets.GetValue(CalDLCSets.Items.TungstenExclude, item.type))
                {
                    float tungScale = 1f + (soulsPlayer.ForceEffect<TungstenEnchant>() ? 2f : 1f);
                    scale /= tungScale;
                }
                else if (item != null && (item.DamageType == ModContent.GetInstance<TrueMeleeDamageClass>() || item.DamageType == ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>()))
                {
                    scale /= TrueMeleeTungstenScaleNerf(player);
                }
            }
            #endregion
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
        void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, int amount = 0)
        {
            string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
            string nerf = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{key}", amount == 0 ? null : amount);
            tooltips.Add(new TooltipLine(Mod, $"{change}{key}", $"{prefix} {nerf}"));
        }

        void ItemBalance(List<TooltipLine> tooltips, EModeChange change, string key, string extra)
        {
            string prefix = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{change}");
            string nerf = Language.GetTextValue($"Mods.FargowiltasSouls.EModeBalance.{key}");
            tooltips.Add(new TooltipLine(Mod, $"{change}{key}", $"{prefix} {nerf} {extra}"));
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            const string BalanceLine = "Cross-mod Balance: ";

            if (WorldSavingSystem.EternityMode)
            {
                if (item.type == ModContent.ItemType<NormalityRelocator>())
                {
                    ItemBalance(tooltips, EModeChange.Nerf, "RodofDiscord");
                }
            }

            float balance = BalanceChange(item);
            const string BalanceUpLine = $"[c/00A36C:{BalanceLine}]";
            const string BalanceDownLine = $"[c/FF0000:{BalanceLine}]";
            if (balance > 1)
            {
                tooltips.Add(new TooltipLine(Mod, "DamageUp", $"{BalanceUpLine}Damage increased by {Math.Round((balance - 1) * 100)}%."));
            }
            else if (balance < 1)
            {
                tooltips.Add(new TooltipLine(Mod, "DamageDown", $"{BalanceDownLine}Damage decreased by {Math.Round((1 - balance) * 100)}%."));
            }
            if (item.type == ItemID.MagicDagger)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Damage decreased by 50% in Pre-Hardmode."));
            }
            if (item.type == ModContent.ItemType<ProfanedSoulCrystal>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Massively reduced damage with any minions active"));
            }
            if (item.type == ModContent.ItemType<TungstenEnchant>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Less effective on true melee weapons"));
            }
            if (item.type == ModContent.ItemType<MythrilEnchant>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Less effective on rogue weapons"));
            }
            if (item.type == ModContent.ItemType<OrichalcumEnchant>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Reduced effectiveness"));
            }
            if (item.type == ModContent.ItemType<AdamantiteEnchant>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Does not work with any Calamity projectiles, due to a massive amount of unintended interactions/bugs\nWill be fixed in the future"));
            }
            if (item.type == ModContent.ItemType<DaawnlightSpiritOrigin>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Effect disabled while Tin Enchantment effect is active"));
            }
            if (item.type == ModContent.ItemType<SlimyShield>())
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Does not inflict Oiled"));
            }
            if (item.ModItem != null && item.ModItem is FlightMasteryWings)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Flight stats decreased when fighting non-Souls Mod bosses"));
            }

            if (item.type == ItemID.CobaltSword || item.type == ItemID.PalladiumSword ||
                item.type == ItemID.OrichalcumSword || item.type == ItemID.MythrilSword ||
                item.type == ItemID.OrichalcumHalberd)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Stat buffs decreased"));
            }

            CalamityGlobalItem calItem = item.GetGlobalItem<CalamityGlobalItem>();
            if (!item.IsAir && calItem.AppliedEnchantment.HasValue)
            {
                if (calItem.AppliedEnchantment.Value.ID == 1000)
                {
                    tooltips.Add(new TooltipLine(Mod, "BalanceDown_HealEnchant", $"{BalanceDownLine}Accumulated damage capped at 500.000"));
                }
            }
            /*
            if (item.type == ItemID.WarmthPotion)
            {
                tooltips.Add(new TooltipLine(Mod, "BalanceDown", $"{BalanceDownLine}Does not grant buff immunities"));
            }
            */
        }
        #endregion
    }
}
