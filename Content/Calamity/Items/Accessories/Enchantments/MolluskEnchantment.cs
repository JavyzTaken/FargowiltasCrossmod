using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using CalamityMod.Items.Accessories;
using CalamityMod.CalPlayer;
using System.Collections.Generic;
using CalamityMod.Items.Armor.Mollusk;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using Terraria.DataStructures;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MolluskEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(100, 120, 160);
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }
        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {

        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().Mollusk = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MolluskShellmet>());
            recipe.AddIngredient(ModContent.ItemType<MolluskShellplate>());
            recipe.AddIngredient(ModContent.ItemType<MolluskShelleggings>());
            recipe.AddIngredient(ModContent.ItemType<VictideEnchantment>());
            recipe.AddIngredient(ModContent.ItemType<ShellfishStaff>());
            recipe.AddIngredient(ModContent.ItemType<GiantPearl>());
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{

    public partial class CrossplayerCalamity : ModPlayer
    {
        public void MolluskEffects()
        {
            Player.gills = true;
            Player.accFlipper = true;
            Player.ignoreWater = true;
            Player.GetModPlayer<CalamityPlayer>().abyssBreathLossStat -= 0.1f;
            if (!Player.wet)
            {
                Player.moveSpeed -= 0.22f;
                Player.velocity.X *= 0.985f;
            }
            Player.GetModPlayer<CalamityPlayer>().giantPearl = true;
            Lighting.AddLight((int)Player.Center.X / 16, (int)Player.Center.Y / 16, 0.45f, 0.8f, 0.8f);
        }
        public void MolluskClamShot(EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            if (Player.GetModPlayer<CrossplayerCalamity>().Mollusk && Main.rand.NextBool(10))
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Shellclam>(), damage / 3, knockback, Main.myPlayer);
            }
        }
    }
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MolluskGNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.GetGlobalNPC<CalamityGlobalNPC>().shellfishVore > 0)
            {

                int numclams = 0;
                int owner = 255;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.type == ModContent.ProjectileType<Shellclam>() && proj.ai[0] == 1 && proj.ai[1] == npc.whoAmI)
                    {
                        owner = proj.owner;
                        numclams++;
                        if (numclams > 4)
                        {
                            numclams = 4;
                            break;
                        }
                    }
                }
                Player player = Main.player[owner];
                int debuffdamage = (int)player.GetTotalDamage<GenericDamageClass>().ApplyTo(225);
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= numclams * debuffdamage;
                if (damage < numclams * debuffdamage / 5)
                {
                    damage = numclams * debuffdamage / 5;
                }
            }
        }
    }
}
