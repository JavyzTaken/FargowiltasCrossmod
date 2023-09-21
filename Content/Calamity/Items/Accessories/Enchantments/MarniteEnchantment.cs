using Terraria.ModLoader;
using FargowiltasCrossmod.Core;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using CalamityMod.Items.Armor.MarniteArchitect;
using CalamityMod.Items.Accessories;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(153, 200, 193);
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().Marnite = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MarniteArchitectHeadgear>());
            recipe.AddIngredient(ModContent.ItemType<MarniteArchitectToga>());
            recipe.AddIngredient(ModContent.ItemType<MarniteRepulsionShield>());
            recipe.AddIngredient(ModContent.ItemType<UnstableGraniteCore>());
            recipe.AddIngredient(ModContent.ItemType<GladiatorsLocket>());
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }

    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public int usedWeaponTimer;
        public void MarniteEffects()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                if (Player.GetToggleValue("BuildBuff"))
                {
                    Player.tileRangeX += 9;
                    Player.tileRangeY += 9;
                    Player.tileSpeed += 0.5f;
                }
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<MarniteSword>()] < 2 && Player.GetToggleValue("MarniteSwords"))
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<MarniteSword>(), 10, 0f, Main.myPlayer);
                    Player.AddBuff(ModContent.BuffType<MarniteSwordsBuff>(), 18000);
                }
                if (ForceEffect(ModContent.ItemType<MarniteEnchantment>()))
                {
                    if (Player.controlUseItem && Player.HeldItem.damage > 0)
                    {
                        usedWeaponTimer = 300;
                    }
                    if (usedWeaponTimer > 0) usedWeaponTimer--;
                    if (usedWeaponTimer == 0)
                    {
                        for (int i = 0; i < 180; i++)
                        {
                            Dust.NewDustPerfect(Player.Center + new Vector2(0, 200).RotatedBy(MathHelper.ToRadians(i * 2)), DustID.BlueFlare).noGravity = true ;
                        }
                        for (int i = 0; i < Main.npc.Length; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.Distance(Player.Center) < 200 && npc.knockBackResist > 0)
                            {
                                npc.velocity += (npc.Center - Player.Center).SafeNormalize(Vector2.Zero) * 0.5f;

                            }
                        }
                    }
                }
            }
        }
    }
}
