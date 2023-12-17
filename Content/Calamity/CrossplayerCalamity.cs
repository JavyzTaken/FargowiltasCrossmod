using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Events;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using Fargowiltas.Common.Configs;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common.Systems;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler;
using log4net.Repository.Hierarchy;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

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

            Player.moveSpeed -= 0.4f;
            // Player.statManaMax2 += 100;
            //Player.manaRegenDelay = Math.Min(Player.manaRegenDelay, 30);
            Player.manaRegenBonus -= 5;
            if (DLCCalamityConfig.Instance.BalanceRework)
            {
                if (BossRushEvent.BossRushActive)
                {
                    Player.AddBuff(ModContent.BuffType<CalamitousPresenceBuff>(), 2);
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
            ModContent.ItemType<NebulousCataclysm>(),
            ModContent.ItemType<Eternity>(), //fargo reference
            ModContent.ItemType<Vehemence>(),
            ModContent.ItemType<Phaseslayer>()
        };
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void PostUpdateEquips()
        {
            FargoSoulsPlayer soulsPlayer = Player.FargoSouls();
            CalamityPlayer calamityPlayer = Player.Calamity();

            /*
            if (!soulsPlayer.TerrariaSoul && soulsPlayer.TungstenEnchantItem != null && TungstenExcludeWeapon.Contains(Player.HeldItem.type))
            {
                Player.GetAttackSpeed(DamageClass.Melee) += 0.5f; //negate attack speed effect
            }
            */
            calamityPlayer.profanedCrystalStatePrevious = 0;
            calamityPlayer.pscState = 0;

            if (AdamantiteIgnoreItem.Contains(Player.HeldItem.type))
            {
                soulsPlayer.AdamantiteEnchantItem = null;
            }
            if (soulsPlayer.TinEnchantItem != null)
            {
                calamityPlayer.spiritOrigin = false;
            }

            if (soulsPlayer.noDodge)
            {
                if (!Player.HasCooldown(GlobalDodge.ID))
                {
                    Player.AddCooldown(GlobalDodge.ID, 60, true);
                }

            }
        }
        public bool[] PreUpdateBuffImmune;
        public override void PreUpdateBuffs()
        {
            PreUpdateBuffImmune = Player.buffImmune;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            
            if (CalamityKeybinds.NormalityRelocatorHotKey.JustPressed && WorldSavingSystem.EternityMode && FargoSoulsUtil.AnyBossAlive())
            {
                //copied from vanilla chaos state damage
                Player.statLife -= Player.statLifeMax2 / 7;
                PlayerDeathReason damageSource = PlayerDeathReason.ByOther(13);
                if (Main.rand.NextBool(2))
                {
                    damageSource = PlayerDeathReason.ByOther(Player.Male ? 14 : 15);
                }
                if (Player.statLife <= 0)
                {
                    Player.KillMe(damageSource, 1.0, 0);
                }
                Player.lifeRegenCount = 0;
                Player.lifeRegenTime = 0f;
            }
            if (ModContent.GetInstance<FargoClientConfig>().DoubleTapDashDisabled)
            {
                Player.GetModPlayer<CalamityPlayer>().dashTimeMod = 0;
            }
        }
        public override void PostUpdateMiscEffects()
        {
            FargoSoulsPlayer soulsPlayer = Player.FargoSouls();
            CalamityPlayer calPlayer = Player.Calamity();
            if (CalamitousPresence && !soulsPlayer.MutantPresence)
            {
                Player.statDefense /= 2f;
                Player.endurance /= 2f;
                Player.shinyStone = false;
                Player.Calamity().purity = false;
                if (Player.statLifeMax2 > 1000)
                    Player.statLifeMax2 = 1000;
            }
            const int witherDamageCap = 500000;
            if (calPlayer.witheringDamageDone > witherDamageCap)
                calPlayer.witheringDamageDone = witherDamageCap;

            if (soulsPlayer.MutantFang) //faster life reduction
            {
                soulsPlayer.LifeReductionUpdateTimer++;
            }
            base.PostUpdateMiscEffects();
        }
        public override void PostUpdate()
        {
            FargoSoulsPlayer soulsPlayer = Player.FargoSouls();
            if (CalamitousPresence && !soulsPlayer.MutantPresence)
            {
                Player.Calamity().purity = false;
                if (Player.statLifeMax2 > 1000)
                    Player.statLifeMax2 = 1000;
            }
        }
        public override void UpdateBadLifeRegen()
        {
            
            if (CalamitousPresence)
            {
                const int cap = 2;
                if (Player.lifeRegen > cap)
                    Player.lifeRegen = cap;
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
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            FargoSoulsPlayer modPlayer = Player.FargoSouls();
            if (modPlayer.TungstenEnchantItem != null && modPlayer.Toggler != null && Player.GetToggleValue("Tungsten")
                && (modPlayer.ForceEffect(modPlayer.TungstenEnchantItem.type) || item.shoot == ProjectileID.None))
            {
                TungstenTrueMeleeDamageNerf(Player, ref modifiers, item);
            }
        }
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            FargoSoulsPlayer modPlayer = Player.FargoSouls();
            if (modPlayer.TungstenEnchantItem != null && proj.FargoSouls().TungstenScale != 1)
            {
                TungstenTrueMeleeDamageNerf(Player, ref modifiers);
            }
        }
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public static void TungstenTrueMeleeDamageNerf(Player player, ref NPC.HitModifiers modifiers, Item item = null)
        {
            if (item == null)
                item = player.HeldItem;
            if (item != null && item.DamageType == ModContent.GetInstance<TrueMeleeDamageClass>() || item.DamageType == ModContent.GetInstance<TrueMeleeNoSpeedDamageClass>())
                modifiers.FinalDamage /= 1.15f;
        }
    }
}
