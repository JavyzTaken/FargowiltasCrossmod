using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasSouls;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;
using FargowiltasSouls.Core.ModPlayers;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod;
using CalamityMod.Items.Armor.Mollusk;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Accessories;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MolluskEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
            //return true;
        }
        public override Color nameColor => new Color(153, 200, 193);
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 40000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<MolluskEffect>(Item);
        }
        public static void AddEffects(Player player, Item item)
        {
            player.AddEffect<MolluskEffect>(item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<MolluskShellmet>(1);
            recipe.AddIngredient<MolluskShellplate>(1);
            recipe.AddIngredient<MolluskShelleggings>(1);
            recipe.AddIngredient<ClamCrusher>(1);
            recipe.AddIngredient<ShellfishStaff>(1);
            recipe.AddIngredient<DeepDiver>(1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MolluskEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<GaleHeader>(); // TODO: fix
        public override int ToggleItemType => ModContent.ItemType<MolluskEnchant>();
        
        public static void MolluskTrigger(Player player)
        {
            if (player.CalamityAddon().ClamSlamCooldown == 0 && player.velocity.Y != 0)
            {
                //Main.NewText("g");
                player.CalamityAddon().ClamSlamTime = 15;
                player.CalamityAddon().ClamSlamHorizontalSpeed = player.velocity.X * 0.7f;
            }
        }
        public override void PostUpdateEquips(Player player)
        {
            if (player.CalamityAddon().ClamSlamIframes > 0)
            {
               // player.immuneNoBlink = true;
                player.immuneTime = 1;
                player.immune = true;
                player.AddImmuneTime(ImmunityCooldownID.Bosses, 1);
            }
            if (player.CalamityAddon().ClamSlamTime > 0)
            {
                
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ClamSlam>()] == 0)
                {
                    //int damage = player.ForceEffect<AstralEffect>() ? 100 : 50;
                    Projectile p = Projectile.NewProjectileDirect(player.GetSource_EffectItem<MolluskEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<ClamSlam>(), 2000, 2, player.whoAmI);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p.whoAmI);
                    }
                }
                player.maxFallSpeed = 30;
                player.velocity = new Vector2(player.CalamityAddon().ClamSlamHorizontalSpeed, 40);
                player.CalamityAddon().ClamSlamTime--;
                
            }
        }
    }
   
}
