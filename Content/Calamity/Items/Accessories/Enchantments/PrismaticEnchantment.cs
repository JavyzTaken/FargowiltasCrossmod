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

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{

    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class PrismaticEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(108, 66, 166);

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
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();
            SBDPlayer.Prismatic = true;
        }

        public override void AddRecipes()
        {
            //recipe
            Recipe recipe = CreateRecipe();
            
            recipe.AddIngredient(ModContent.ItemType<PrismaticHelmet>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PrismaticRegalia>(), 1);
            recipe.AddIngredient(ModContent.ItemType<PrismaticGreaves>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DarkSpark>(), 1);
            recipe.AddIngredient(ModContent.ItemType<HandheldTank>(), 1);
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyRailguns");
            recipe.AddIngredient(ModContent.ItemType<LunicCorpsEnchantment>());
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void PrismaticAttackEffects(int damage)
        {
            bool charged = PrismaticCharge >= 50000;
            PrismaticCharge += damage;
            if (!charged && PrismaticCharge >= 50000)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustDirect(Player.Center, 0, 0, ModContent.DustType<CalamityMod.Dusts.CosmiliteBarDust>()).noGravity = true;

                }
                SoundEngine.PlaySound(SoundID.Item4 with { Pitch = -1, Volume = 0.5f }, Player.Center);
            }
        }
        public void PrismaticTrigger()
        {
            if (CalamityKeybinds.SetBonusHotKey.JustPressed && PrismaticCharge >= 50000 && Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<PrisMissile>(), 500, 0, Main.myPlayer);
                PrismaticCharge = 0;
                for (int i = 0; i < 15; i++)
                {
                    Dust.NewDustDirect(Player.Center, 0, 0, ModContent.DustType<CalamityMod.Dusts.CosmiliteBarDust>()).noGravity = true;

                }
                SoundEngine.PlaySound(SoundID.Item92, Player.Center);
            }
        }
        public int PrismaticeHitEffects(int damage, NPC npc)
        {
            if (npc.lifeMax < 500)
            {
                return 0;
            }
            return damage;
        }
    }
}