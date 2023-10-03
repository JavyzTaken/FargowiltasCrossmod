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

namespace FargowiltasCrossmod.Content.Calamity
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public partial class CrossplayerCalamity : ModPlayer
    {
        public override void OnEnterWorld()
        {
           
        }
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
            if (BossRushEvent.BossRushActive)
            {
                Player.AddBuff(ModContent.BuffType<MutantPresenceBuff>(), 2);
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
        public override void PostUpdateEquips()
        {
            if (!Player.FargoSouls().TerrariaSoul && Player.FargoSouls().TungstenEnchantItem != null && TungstenExcludeWeapon.Contains(Player.HeldItem.type))
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.5f; //negate attack speed effect
            }
        }

        public override float UseSpeedMultiplier(Item item)
        {
            if (AttackSpeedExcludeWeapons.Contains(item.type))
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
