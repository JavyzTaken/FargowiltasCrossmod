using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.MarniteArchitect;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using CalamityMod;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Items.Tools;
using FargowiltasCrossmod.Core.Calamity;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [AutoloadEquip(EquipType.Back)]
    [LegacyName("MarniteEnchantment")]
    public class MarniteEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Color nameColor => new Color(153, 200, 193);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = 15000;
            Item.defense = 2;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AddEffects(player, Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<MarniteRepulsionEffect>(item);
            player.AddEffect<MarniteLasersEffect>(item);
            player.tileSpeed += 0.1f;
            player.blockRange += 5;
            if (player.FargoSouls().ForceEffect(item.type))
            {
                player.tileSpeed += 0.15f;
                player.blockRange += 5;
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MarniteArchitectHeadgear>());
            recipe.AddIngredient(ModContent.ItemType<MarniteArchitectToga>());
            recipe.AddIngredient(ModContent.ItemType<MarniteRepulsionShield>());
            recipe.AddIngredient(ModContent.ItemType<MarniteDeconstructor>());
            recipe.AddIngredient(ModContent.ItemType<MarniteObliterator>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteRepulsionEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<MarniteEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            MarniteRepulsionShieldPlayer modPlayer = player.GetModPlayer<MarniteRepulsionShieldPlayer>();
            modPlayer.shieldEquipped = true;

            if (player.whoAmI == Main.myPlayer)
            {
                int baseDamage = player.ApplyArmorAccDamageBonusesTo(5);
                var source = player.GetSource_EffectItem<MarniteRepulsionEffect>();
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MarniteRepulsionHitbox>()] < 1)
                {
                    var hitbox = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, ModContent.ProjectileType<MarniteRepulsionHitbox>(), baseDamage, 10f, Main.myPlayer);
                    hitbox.originalDamage = baseDamage;
                }
            }
        }

    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteLasersEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<MarniteEnchant>();
        public override bool ExtraAttackEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            var addonPlayer = player.CalamityAddon();
            Item item = player.HeldItem;
            bool marniteExclusion = item != null && player.itemAnimation > 0 && CalDLCSets.Items.MarniteExclude[item.type];
            if (item != null && item.IsWeapon() && player.FargoSouls().WeaponUseTimer > 0 || marniteExclusion) // using weapon or boss viable tool
            {
                addonPlayer.MarniteTimer = 0;
            }
            else
            {
                addonPlayer.MarniteTimer++;
                if (addonPlayer.MarniteTimer > 25)
                {
                    addonPlayer.MarniteTimer = 0;

                    int nearestNPCID = FargoSoulsUtil.FindClosestHostileNPC(player.Center, 490, true, true);
                    if (nearestNPCID.IsWithinBounds(Main.maxNPCs))
                    {
                        NPC nearestNPC = Main.npc[nearestNPCID];
                        if (nearestNPC.Alive())
                        {
                            Vector2 pos = Main.rand.NextVector2FromRectangle(player.Hitbox);
                            Vector2 vel = pos.DirectionTo(nearestNPC.Center) * 2;

                            float damage = player.ForceEffect<MarniteLasersEffect>() ? 150 : 30;

                            int index = Projectile.NewProjectile(player.GetSource_EffectItem<MarniteLasersEffect>(), pos, vel, ModContent.ProjectileType<MarniteLaser>(), (int)damage, 1, player.whoAmI);
                            if (index.IsWithinBounds(Main.maxProjectiles) && Main.projectile[index] is Projectile proj)
                            {
                                proj.knockBack += 10;
                            }
                            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, index);
                        }
                    }
                }
            }
        }
    }
}
