﻿using Terraria;
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
        public bool ClericEnch;
        public bool TemplarEnch;
        public Item TemplarEnchItem;
        public bool LivingWoodEnch;
        public Item LivingWoodEnchItem;
        public bool SilkEnch;
        public bool WhiteKnightEnch;
        public bool LodeStoneEnch;
        public Item LodeStoneEnchItem;
        public bool DragonEnch;
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


        public List<int> LodeStonePlatforms = new();
        public List<int> ActiveValaChunks = new();
        public List<int> GraniteCores = new();

        internal int TemplarCD = 360;
        internal int ValadiumCD = 240;
        internal int AstroLaserCD = 60;
        #endregion

        public bool GildedMonicle;
        public bool GildedBinoculars;
        public bool MynaAccessory;

        public Item TempleCoreItem;
        public int TempleCoreCounter;

        public override void ResetEffects()
        {
            EbonEnch = false;
            ClericEnch = false;
            TemplarEnch = false;
            TemplarEnchItem = null;
            LivingWoodEnch = false;
            LivingWoodEnchItem = null;
            SilkEnch = false;
            WhiteKnightEnch = false;
            LodeStoneEnch = false;
            LodeStoneEnchItem = null;
            DragonEnch = false;
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
            if (GraniteEnch && hit.Damage >= target.life && proj.type != ModContent.ProjectileType<GraniteExplosion>())
            {
                Projectile.NewProjectileDirect(Player.GetSource_Accessory(GraniteEnchItem), target.Center, Vector2.Zero, ModContent.ProjectileType<GraniteExplosion>(), 0, 0f, Player.whoAmI);
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
    }
}
