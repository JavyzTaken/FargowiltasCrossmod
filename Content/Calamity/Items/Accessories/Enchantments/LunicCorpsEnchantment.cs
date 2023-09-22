using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items.Armor.Prismatic;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using Terraria.Audio;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;
using CalamityMod.Items.Armor.LunicCorps;
using CalamityMod.Items.Weapons.Rogue;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class LunicCorpsEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(75, 101, 5);

        public override void SetStaticDefaults()
        {
            //name and description

        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine tooltip in tooltips)
            {
                int index = tooltip.Text.IndexOf("[button]");
                if (index != -1 && tooltip.Text.Length > 0)
                {
                    tooltip.Text = tooltip.Text.Remove(index, 8);
                    tooltip.Text = tooltip.Text.Insert(index, CalamityKeybinds.SetBonusHotKey.TooltipHotkeyString());
                }
            }
            base.SafeModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.Lunic = true;
            player.Calamity().lunicCorpsSet = true;
        }

        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<LunicCorpsHelmet>(), 1);
            recipe.AddIngredient(ModContent.ItemType<LunicCorpsVest>(), 1);
            recipe.AddIngredient(ModContent.ItemType<LunicCorpsBoots>(), 1);
            recipe.AddIngredient(ModContent.ItemType<P90>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Needler>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ShockGrenade>(), 300);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public int LunicCharge;
        public void LunicAttackEffects(int damage)
        {
            bool charged = LunicCharge >= 25000;
            LunicCharge += damage;
            if (!charged && LunicCharge >= 25000)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustDirect(Player.Center, 0, 0, ModContent.DustType<CalamityMod.Dusts.CosmiliteBarDust>()).noGravity = true;

                }
                SoundEngine.PlaySound(SoundID.Item4 with { Pitch = -1, Volume = 0.5f }, Player.Center);
            }
        }
        public void LunicTrigger()
        {
            if (CalamityKeybinds.SetBonusHotKey.JustPressed && LunicCharge >= 25000 && Main.myPlayer == Player.whoAmI)
            {

                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<PlasmaGrenade>(), 500, 0, Main.myPlayer);
                if (ForceEffect(ModContent.ItemType<LunicCorpsEnchantment>()))
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 10, ModContent.ProjectileType<PlasmaGrenade>(), 500, 0, Main.myPlayer);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 10, ModContent.ProjectileType<PlasmaGrenade>(), 500, 0, Main.myPlayer);
                }
                LunicCharge = 0;
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustDirect(Player.Center, 0, 0, ModContent.DustType<CalamityMod.Dusts.CosmiliteBarDust>()).noGravity = true;

                }
                SoundEngine.PlaySound(SoundID.Item92, Player.Center);
            }
        }
    }
}