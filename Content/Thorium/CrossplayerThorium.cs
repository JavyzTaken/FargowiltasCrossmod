using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;

namespace FargowiltasCrossmod.Content.Thorium
{
    [ExtendsFromMod("ThoriumMod")]
    public partial class CrossplayerThorium : ModPlayer
    {
        #region enchants
        public bool EbonEnch;
        public Item EbonEnchItem;
        public bool NoviceClericEnch;
        public Item NoviceClericEnchItem;
        public bool TemplarEnch;
        public Item TemplarEnchItem;
        public bool LivingWoodEnch;
        public Item LivingWoodEnchItem;
        public bool SilkEnch;
        public bool WhiteKnightEnch;
        public bool LodeStoneEnch;
        public Item LodeStoneEnchItem;
        public bool DragonEnch;
        public Item DragonEnchItem;
        public bool SteelEnch;
        public Item SteelEnchItem;
        public bool DarkSteelEnch;
        public bool ValadiumEnch;
        public Item ValadiumEnchItem;
        public bool BerserkerEnch;
        public bool FungusEnch;
        public bool GraniteEnch;
        public Item GraniteEnchItem;
        public bool AstroEnch;
        public Item AstroEnchItem;
        public bool SpiritTrapperEnch;
        public Item SpiritTrapperEnchItem;
        public bool YewWoodEnch;
        public Item YewWoodEnchItem;
        public bool FleshEnch;
        public Item FleshEnchItem;
        public bool DemonBloodEnch;
        public Item DemonBloodEnchItem;
        public bool ConduitEnch;
        public Item ConduitEnchItem;
        public bool DepthDiverEnchant;
        public Item DepthDiverEnchantItem;
        public bool FallenPaladinEnch;
        public Item FallenPaladinEnchItem;
        public bool WarlockEnch;
        public Item WarlockEnchItem;
        public bool SacredEnch;
        public Item SacredEnchItem;
        public bool DreadEnch;
        public Item DreadEnchItem;

        public bool HelheimForce;

        public List<int> LodeStonePlatforms = new();
        public List<int> ActiveValaChunks = new();

        internal int TemplarCD = 360;
        internal int ValadiumCD = 240;
        internal int AstroLaserCD = 60;

        internal int NoviceClericCrosses = 0;
        internal int NoviceClericTimer = 0;
        public Vector2 crossOrbitalRotation = Vector2.UnitY;
        #endregion

        public bool GildedMonicle;
        public bool GildedBinoculars;
        public bool MynaAccessory;

        public Item TempleCoreItem;
        public int TempleCoreCounter;

        public override void ResetEffects()
        {
            EbonEnch = false;
            EbonEnchItem = null;
            NoviceClericEnch = false;
            NoviceClericEnchItem = null;
            TemplarEnch = false;
            TemplarEnchItem = null;
            LivingWoodEnch = false;
            LivingWoodEnchItem = null;
            SilkEnch = false;
            WhiteKnightEnch = false;
            LodeStoneEnch = false;
            LodeStoneEnchItem = null;
            DragonEnch = false;
            DragonEnchItem = null;
            SteelEnch = false;
            SteelEnchItem = null;
            DarkSteelEnch = false;
            ValadiumEnch = false;
            ValadiumEnchItem = null;
            BerserkerEnch = false;
            FungusEnch = false;
            GraniteEnch = false;
            GraniteEnchItem = null;
            AstroEnch = false;
            AstroEnchItem = null;
            SpiritTrapperEnch = false;
            SpiritTrapperEnchItem = null;
            YewWoodEnch = false;
            YewWoodEnchItem = null;
            FleshEnch = false;
            FleshEnchItem = null;
            DemonBloodEnch = false;
            DemonBloodEnchItem = null;
            ConduitEnch = false;
            ConduitEnchItem = null;
            DepthDiverEnchant = false;
            DepthDiverEnchantItem = null;
            FallenPaladinEnch = false;
            FallenPaladinEnchItem = null;
            WarlockEnch = false;
            WarlockEnchItem = null;
            SacredEnch = false;
            SacredEnchItem = null;
            DreadEnch = false;
            DreadEnchItem = null;

            HelheimForce = false;

            GildedMonicle = false;
            GildedBinoculars = false;
            MynaAccessory = false;

            TempleCoreItem = null;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (TemplarEnch && TemplarCD == 0)
            {
                TemplarCD = 360;
                TemplarEnchant.summonHolyFire(Player);
            }
            if (FungusEnch)
            {
                if (target.TryGetGlobalNPC(out FungusEnemy funguy) && !funguy.Infected && !target.boss && Main.rand.NextBool(10))
                {
                    funguy.infectedBy = Player.whoAmI;
                    funguy.Infected = true;
                }
            }
            if (AstroEnch && hit.Crit && AstroLaserCD <= 0)
            {
                SpawnAstroLaser(target);
            }

            if (FleshEnch && Main.rand.NextBool(10))
            {
                SpawnFlesh(target);
            }

            if (hit.Damage >= target.life) // kills
            {
                if (GraniteEnch && proj.type != ModContent.ProjectileType<GraniteExplosion>())
                {
                    Projectile.NewProjectileDirect(Player.GetSource_Accessory(GraniteEnchItem), target.Center, Vector2.Zero, ModContent.ProjectileType<GraniteExplosion>(), 0, 0f, Player.whoAmI);
                }

                if (DemonBloodEnch)
                {
                    SpawnDemonBlood(target.Center);
                }
            }

            // this (should) be true if the hit moved the boss below a 10% hp increment
            // works by checking the current life's distance to the next increment vs the life - damage distance to next incement. 
            // may be unreliable but i think its cool.
            if (target.boss && (target.life % (target.lifeMax / 10)) < ((target.life - hit.Damage) % (target.lifeMax / 10)))
            {
                if (DemonBloodEnch)
                {
                    SpawnDemonBlood(target.Center);
                }
            }

            if (hit.Crit)
            {
                if (WarlockEnch || SacredEnch)
                {
                    int projType = ModContent.ProjectileType<DLCShadowWisp>();

                    if (Player.ownedProjectileCounts[projType] < 15)
                    {
                        int shadowWispType = WarlockEnch ? (SacredEnch ? 2 : 0) : (1);
                        int damage = WarlockEnch ? (SacredEnch ? 24 : 16) : (0);
                        float kb = WarlockEnch ? 1f : 0f;

                        var soulsPlayer = Player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>();
                        if ((WarlockEnch && soulsPlayer.ForceEffect(WarlockEnchItem.type)) || (SacredEnch && soulsPlayer.ForceEffect(SacredEnchItem.type)))
                        {
                            damage = 24;
                            shadowWispType = 2;
                            kb = 1f;
                        }

                        Item itemToUse = WarlockEnch ? WarlockEnchItem : SacredEnchItem;

                        Projectile.NewProjectile(Player.GetSource_Accessory(itemToUse), target.Center, Vector2.Zero, projType, damage, kb, Player.whoAmI, 0, 0, shadowWispType);
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FleshEnch && Main.rand.NextBool(10))
            {
                SpawnFlesh(target);
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ThoriumKeybinds.LivingWoodBind.JustPressed)
            {
                LivingWoodKey();
            }
            if (ThoriumKeybinds.SteelParryBind.JustPressed)
            {
                ParryKey();
            }
            if (ThoriumMod.ThoriumHotkeySystem.AccessoryKey.JustPressed)
            {
                if (FallenPaladinEnch)
                {
                    FallenPaladinEffect();
                }
                if (WarlockEnch || SacredEnch)
                {
                    int type = ModContent.ProjectileType<DLCShadowWisp>();
                    for (int i = 0; i < 1000; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.owner == Main.myPlayer && projectile.type == type)
                        {
                            projectile.localAI[0] = 1f;
                        }
                    }
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Player.HasBuff<LivingWood_Root_B>())
            {
                Player.ClearBuff(ModContent.BuffType<LivingWood_Root_B>());
                LivingWoodEnchant.KillLivingWoodRoots(Player.whoAmI);
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Player.HasBuff<LivingWood_Root_B>())
            {
                Player.ClearBuff(ModContent.BuffType<LivingWood_Root_B>());
                LivingWoodEnchant.KillLivingWoodRoots(Player.whoAmI);
            }
        }

        public override void PreUpdate()
        {
            if (TempleCoreItem != null)
            {
                TempleCoreEffect();
            }
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (YewWoodEnch)
            {
                YewWoodEffect(item, ref position, ref velocity, ref type, ref damage, ref knockback);
            }
        }

        public override void OnEnterWorld()
        {
            //Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.ThoriumBuggyWarning1"), Color.Yellow);
            //Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.ThoriumBuggyWarning2"), Color.Yellow);
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (NoviceClericCrosses > 0) mult = 0f;
        }

        public override void OnConsumeMana(Item item, int manaConsumed)
        {
            NoviceClericOnManaUse();
        }

        public override void PostUpdateEquips()
        {
            NoviceClericEffect();
        }

    }
}
