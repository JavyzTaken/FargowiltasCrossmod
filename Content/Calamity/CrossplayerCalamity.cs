using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using FargowiltasSouls;
using CalamityMod.World;
using Terraria.GameInput;
using Terraria.DataStructures;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.ModPlayers;
using System.Collections.Generic;
//using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using System;
using FargowiltasCrossmod.Core.Systems;
using CalamityMod.Events;
using FargowiltasSouls.Content.Buffs.Boss;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using CalamityMod.Buffs.StatDebuffs;
using FargowiltasCrossmod.Core.Calamity;
using CalamityMod;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Cooldowns;
using CalamityMod.CalPlayer;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Buffs;

namespace FargowiltasCrossmod.Content.Calamity
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CrossplayerCalamity : ModPlayer
    {
        public bool CalamitousPresence;

        public override void ResetEffects()
        {
            CalamitousPresence = CalamitousPresence && Player.HasBuff(ModContent.BuffType<CalamitousPresenceBuff>());
            base.ResetEffects();
        }
        public override void OnEnterWorld()
        {
           
        }
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void PostUpdateBuffs()
        {
            if (!DLCWorldSavingSystem.EternityRev)
                return;
            //copied from emode player buffs, reverse effects

            //Player.pickSpeed -= 0.25f;

            //Player.tileSpeed += 0.25f;
            //Player.wallSpeed += 0.25f;

            Player.moveSpeed -= 0.25f;
           // Player.statManaMax2 += 100;
            //Player.manaRegenDelay = Math.Min(Player.manaRegenDelay, 30);
            Player.manaRegenBonus -= 5;
            if (DLCCalamityConfig.Instance.BalanceRework)
            {
                if (BossRushEvent.BossRushActive)
                {
                    Player.AddBuff(ModContent.BuffType<MutantPresenceBuff>(), 2);
                }
                if (NPC.AnyNPCs(ModContent.NPCType<MutantBoss>()))
                {
                    Player.ClearBuff(ModContent.BuffType<Enraged>());
                }
            }
            //Player.wellFed = true; //no longer expert half regen unless fed
        }
        public static List<int> TungstenExcludeWeapon = new List<int>
        {
            ModContent.ItemType<OldLordClaymore>(),
            ModContent.ItemType<BladecrestOathsword>()
        };
        public static List<int> AttackSpeedExcludeWeapons = new List<int>
        {
            ModContent.ItemType<ExecutionersBlade>()
        };
        public static List<int> AdamantiteIgnoreItem = new List<int>
        {
            ModContent.ItemType<HeavenlyGale>(),
            ModContent.ItemType<TheSevensStriker>(),
            ModContent.ItemType<Phangasm>(),
            ModContent.ItemType<TheJailor>(),
            ModContent.ItemType<AetherfluxCannon>(),
            ModContent.ItemType<TheAnomalysNanogun>(),
            ModContent.ItemType<ClockworkBow>(),
            ModContent.ItemType<GrandStaffoftheNebulaMage>(),
            ModContent.ItemType<Eternity>(), //fargo reference
            ModContent.ItemType<Vehemence>(),
            ModContent.ItemType<Phaseslayer>()
        };
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void PostUpdateEquips()
        {
            FargoSoulsPlayer fargo = Player.FargoSouls();
            CalamityPlayer calamity = Player.Calamity();

            if (!fargo.TerrariaSoul && fargo.TungstenEnchantItem != null && TungstenExcludeWeapon.Contains(Player.HeldItem.type))
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.5f; //negate attack speed effect
            }
            calamity.profanedCrystalStatePrevious = 0;
            calamity.pscState = 0;

            if (AdamantiteIgnoreItem.Contains(Player.HeldItem.type))
            {
                fargo.AdamantiteEnchantItem = null;
            }
            if (fargo.TinEnchantItem != null)
            {
                calamity.spiritOrigin = false;
            }

            if (fargo.noDodge)
            {
                if (!Player.HasCooldown(GlobalDodge.ID))
                {
                    Player.AddCooldown(GlobalDodge.ID, 60, true);
                }
                
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (CalamitousPresence && !Player.FargoSouls().MutantPresence)
            {
                Player.statDefense /= 2;
                Player.endurance /= 2;
                Player.shinyStone = false;
            }
            base.PostUpdateMiscEffects();
        }
        public override void UpdateBadLifeRegen()
        {
            if (CalamitousPresence)
            {
                if (Player.lifeRegen > 5)
                    Player.lifeRegen = 5;
            }
            base.UpdateBadLifeRegen();
        }
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override float UseSpeedMultiplier(Item item)
        {
            if (item.DamageType == ModContent.GetInstance<RogueDamageClass>() && item.useTime < item.useAnimation)
            {
                bool carryOverAttackSpeedCheck = Player.FargoSouls().HaveCheckedAttackSpeed;
                float soulsAttackSpeed = Player.FargoSouls().UseSpeedMultiplier(item);
                Player.FargoSouls().HaveCheckedAttackSpeed = carryOverAttackSpeedCheck;
                return 1f / soulsAttackSpeed;
            }
            return base.UseSpeedMultiplier(item);
        }
    }
}
