using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using Fargowiltas.Common.Configs;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler;
using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasCrossmod.Core.Calamity.ModPlayers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCPlayer : ModPlayer
    {
        public bool CalamitousPresence;
        //Unique accessories fields

        public override void ResetEffects()
        {
            CalamitousPresence = CalamitousPresence && Player.HasBuff(BuffType<CalamitousPresenceBuff>());
            base.ResetEffects();
        }
        public override void OnEnterWorld()
        {

        }

        public override void UpdateEquips()
        {

        }
        public override void PreUpdate()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (ModCompatibility.Calamity.Loaded) // Necessary here to sync in multiplayer
                {
                    ModCompatibility.SoulsMod.Mod.Call("EternityVanillaBossBehaviour", CalDLCConfig.Instance.EternityPriorityOverRev);
                    if (CalDLCConfig.Instance.EternityPriorityOverRev && WorldSavingSystem.EternityMode)
                        CalamityMod.CalamityMod.ExternalFlag_DisableNonRevBossAI = true;
                }
            }
            //Main.NewText(BossRushEvent.BossRushStage);
        }
        public override void PreUpdateMovement()
        {

        }

        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void PostUpdateBuffs()
        {
            if (!CalDLCWorldSavingSystem.EternityRev)
                return;
            //copied from emode player buffs, reverse effects

            //Player.pickSpeed -= 0.25f;

            //Player.tileSpeed += 0.25f;
            //Player.wallSpeed += 0.25f;
            float nerf = 0.25f;
            if (ModCompatibility.SoulsMod.Mod.Version < Version.Parse("1.6.2.1")) //souls mod was giving Double bonus before
                nerf = 0.4f;
            Player.moveSpeed -= nerf;
            // Player.statManaMax2 += 100;
            //Player.manaRegenDelay = Math.Min(Player.manaRegenDelay, 30);
            Player.manaRegenBonus -= 5;
            if (BossRushEvent.BossRushActive)
            {
                Player.AddBuff(BuffType<CalamitousPresenceBuff>(), 2);
            }
            if (NPC.AnyNPCs(NPCType<MutantBoss>()))
            {
                Player.ClearBuff(BuffType<Enraged>());
            }
            //Player.wellFed = true; //no longer expert half regen unless fed
        }
        [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
        public override void PostUpdateEquips()
        {
            FargoSoulsPlayer soulsPlayer = Player.FargoSouls();
            CalamityPlayer calamityPlayer = Player.Calamity();
            calamityPlayer.profanedCrystalStatePrevious = 0;
            calamityPlayer.pscState = 0;

            AdamantiteEffect adamEffect = GetInstance<AdamantiteEffect>();
            if (Player.HasEffect(adamEffect) && Player.HeldItem != null && CalDLCSets.Items.AdamantiteExclude[Player.HeldItem.type])
            {
                AccessoryEffectPlayer effectsPlayer = Player.AccessoryEffects();
                effectsPlayer.ActiveEffects[adamEffect.Index] = false;
                effectsPlayer.EffectItems[adamEffect.Index] = null;

            }
            if (Player.HasEffect<TinEffect>())
            {
                calamityPlayer.spiritOrigin = false;
            }

            if (soulsPlayer.FlightMasterySoul)
                calamityPlayer.infiniteFlight = true;

            if (soulsPlayer.noDodge)
            {
                if (!Player.HasCooldown(GlobalDodge.ID))
                {
                    Player.AddCooldown(GlobalDodge.ID, 60, true);
                }
                else
                {
                    Player.Calamity().cooldowns[GlobalDodge.ID].timeLeft = 1;
                }
            }
            if (soulsPlayer.MutantPresence)
            {
                calamityPlayer.rampartOfDeities = false;
                calamityPlayer.dAmulet = false;
                calamityPlayer.SpongeShieldDurability = 0;
                calamityPlayer.purity = false;

                Player.ClearBuff(BuffType<DemonshadeSetDevilBuff>());
                calamityPlayer.redDevil = false;
            }

            if (calamityPlayer.luxorsGift && Player.HeldItem != null && Player.HeldItem.type == ItemType<KamikazeSquirrelStaff>())
                calamityPlayer.luxorsGift = false;

        }
        public bool[] PreUpdateBuffImmune;
        public override void PreUpdateBuffs()
        {
            PreUpdateBuffImmune = Player.buffImmune;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {

            if (CalamityKeybinds.NormalityRelocatorHotKey.JustPressed && Player.Calamity().normalityRelocator && WorldSavingSystem.EternityMode && LumUtils.AnyBosses())
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
            if (GetInstance<FargoClientConfig>().DoubleTapDashDisabled)
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
                Player.statDefense -= Player.statDefense / 10;
                Player.statDefense -= 20;
                Player.endurance -= 0.05f;
                Player.shinyStone = false;
            }
            const int witherDamageCap = 500000;
            if (calPlayer.witheringDamageDone > witherDamageCap)
                calPlayer.witheringDamageDone = witherDamageCap;

            if (soulsPlayer.MutantFang) //faster life reduction
            {
                soulsPlayer.LifeReductionUpdateTimer++;
            }

            // Laudanum nerfs
            if (calPlayer.laudanum && WorldSavingSystem.EternityMode)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    for (int l = 0; l < Player.MaxBuffs; l++)
                    {
                        int hasBuff = Player.buffType[l];

                        switch (hasBuff)
                        {
                            // Usual Calamity buffs are commented out; respective nerfs are not; to show them in comparison
                            case BuffID.Obstructed:
                                /*
                                Player.headcovered = false;
                                Player.statDefense += 50;
                                Player.GetDamage<GenericDamageClass>() += 0.5f;
                                Player.GetCritChance<GenericDamageClass>() += 25;
                                */
                                Player.statDefense -= 25;
                                Player.GetDamage<GenericDamageClass>() -= 0.25f;
                                Player.GetCritChance<GenericDamageClass>() -= 12.5f;
                                break;
                            case BuffID.Ichor:
                                //Player.statDefense += 40;
                                Player.statDefense -= 20;
                                break;
                            case BuffID.Chilled:
                                /*
                                Player.chilled = false;
                                Player.moveSpeed *= 1.3f;
                                */
                                break;
                            case BuffID.BrokenArmor:
                                /*
                                Player.brokenArmor = false;
                                Player.statDefense += (int)(Player.statDefense * 0.25);
                                */
                                break;
                            case BuffID.Weak:
                                /*
                                Player.GetDamage<MeleeDamageClass>() += 0.151f;
                                Player.statDefense += 14;
                                Player.moveSpeed += 0.3f;
                                */
                                Player.GetDamage<MeleeDamageClass>() -= 0.075f;
                                Player.statDefense -= 7;
                                Player.moveSpeed -= 0.15f;
                                break;
                            case BuffID.Slow:
                                /*
                                Player.slow = false;
                                Player.moveSpeed *= 1.5f;
                                */
                                break;
                            case BuffID.Confused:
                                /*
                                Player.confused = false;
                                Player.statDefense += 30;
                                Player.GetDamage<GenericDamageClass>() += 0.25f;
                                Player.GetCritChance<GenericDamageClass>() += 10;
                                */
                                Player.statDefense -= 15;
                                Player.GetDamage<GenericDamageClass>() -= 0.125f;
                                Player.GetCritChance<GenericDamageClass>() -= 5;
                                break;
                            case BuffID.Blackout:
                                /*
                                Player.blackout = false;
                                Player.statDefense += 30;
                                Player.GetDamage<GenericDamageClass>() += 0.25f;
                                Player.GetCritChance<GenericDamageClass>() += 10;
                                */
                                Player.statDefense -= 15;
                                Player.GetDamage<GenericDamageClass>() -= 0.15f;
                                Player.GetCritChance<GenericDamageClass>() -= 5;
                                break;
                            case BuffID.Darkness:
                                /*
                                Player.blind = false;
                                Player.statDefense += 15;
                                Player.GetDamage<GenericDamageClass>() += 0.1f;
                                Player.GetCritChance<GenericDamageClass>() += 5;
                                */
                                Player.statDefense -= 7;
                                Player.GetDamage<GenericDamageClass>() -= 0.05f;
                                Player.GetCritChance<GenericDamageClass>() -= 2;
                                break;
                        }
                    }
                }
            }
            base.PostUpdateMiscEffects();
        }
        public override void PostUpdate()
        {
            FargoSoulsPlayer soulsPlayer = Player.FargoSouls();
            if (CalamitousPresence && !soulsPlayer.MutantPresence)
            {
                Player.Calamity().purity = false;
            }
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (ModCompatibility.WrathoftheGods.Loaded && WorldSavingSystem.EternityMode)
            {
                if (Main.npc.Any(n => n.Alive() && 
                n.type == ModCompatibility.WrathoftheGods.NoxusBoss1.Type || 
                n.type == ModCompatibility.WrathoftheGods.NoxusBoss2.Type || 
                n.type == ModCompatibility.WrathoftheGods.NamelessDeityBoss.Type))
                {
                    Player.AddBuff(BuffType<MutantFangBuff>(), 180);
                    Player.AddBuff(BuffType<CurseoftheMoonBuff>(), 600);
                }
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
            if (item.DamageType == GetInstance<RogueDamageClass>() && item.useTime < item.useAnimation)
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
