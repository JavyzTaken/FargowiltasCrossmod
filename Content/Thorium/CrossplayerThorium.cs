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
        public bool ClericEnch;
        public bool TemplarEnch;
        public bool LivingWoodEnch;
        public bool SilkEnch;
        public bool WhiteKnightEnch;
        public bool LodeStoneEnch;
        public bool DragonEnch;
        public bool SteelEnch;
        public bool DarkSteelEnch;
        public bool ValadiumEnch;
        public bool BerserkerEnch;
        public bool FungusEnch;
        public bool GraniteEnch;
        public bool AstroEnch;

        public Item TemplarEnchItem;
        public Item LivingWoodEnchItem;
        public Item SteelEnchItem;
        public Item ValadiumEnchItem;
        public Item GraniteEnchItem;
        public Item AstroEnchItem;

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
            LivingWoodEnch = false;
            SilkEnch = false;
            WhiteKnightEnch = false;
            LodeStoneEnch = false;
            DragonEnch = false;
            SteelEnch = false;
            DarkSteelEnch = false;
            ValadiumEnch = false;
            BerserkerEnch = false;
            FungusEnch = false;
            GraniteEnch = false;
            AstroEnch = false;

            TemplarEnchItem = null;
            LivingWoodEnchItem = null;
            SteelEnchItem = null;
            ValadiumEnchItem = null;
            GraniteEnchItem = null;
            AstroEnchItem = null;

            GildedMonicle = false;
            GildedBinoculars = false;
            MynaAccessory = false;

            TempleCoreItem = null;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
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
            if (GraniteEnch && hit.Crit)
            {
                SpawnGraniteCore(proj.Center);
            }
            if (AstroEnch && hit.Crit && AstroLaserCD <= 0)
            {
                SpawnAstroLaser(target);
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
                if (GraniteEnch)
                {
                    if (GraniteCores.Count != 0) 
                        Main.projectile[GraniteCores[0]].Kill();
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

        public override void OnEnterWorld()
        {
            Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.ThoriumBuggyWarning1"), Color.Yellow);
            Main.NewText(Language.GetTextValue($"Mods.{Mod.Name}.Message.ThoriumBuggyWarning2"), Color.Yellow);
        }
    }
}
