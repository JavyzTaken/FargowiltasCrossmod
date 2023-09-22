using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod;
using Terraria.Audio;
using System.Collections.Generic;
using CalamityMod.World;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Rarities;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.NPCS;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DemonshadeEnchantment : BaseEnchant
    {
        
        protected override Color nameColor => new Color(204, 42, 60);
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<HotPink>();
        }


        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            base.SafeModifyTooltips(tooltips);

            foreach (TooltipLine tooltip in tooltips)
            {
                int index = tooltip.Text.IndexOf("[button]");
                if (index != -1 && tooltip.Text.Length > 0)
                {
                    tooltip.Text = tooltip.Text.Remove(index, 8);
                    tooltip.Text = tooltip.Text.Insert(index, CalamityKeybinds.RageHotKey.TooltipHotkeyString());
                }
            }
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerCalamity>().Demonshade = true;

        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<DemonshadeHelm>());
            recipe.AddIngredient(ModContent.ItemType<DemonshadeBreastplate>());
            recipe.AddIngredient(ModContent.ItemType<DemonshadeGreaves>());
            recipe.AddIngredient(ModContent.ItemType<ShatteredCommunity>());
            recipe.AddIngredient(ModContent.ItemType<GaelsGreatsword>());
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
}
namespace FargowiltasCrossmod.Content.Calamity
{
    public partial class CrossplayerCalamity : ModPlayer
    {
        public void DemonshadeEffects()
        {

            if (Player.dead || !Player.active)
            {
                DemonshadeLevel = 0;
                DemonshadeXP = 0;
            }

            if (!Player.Calamity().rageModeActive)
            {
                DemonshadeLevel = 0;
                DemonshadeXP = 0;
            }
            if (!Player.Calamity().rageModeActive && !Player.Calamity().shatteredCommunity && !Player.Calamity().heartOfDarkness)
            {
                Player.Calamity().rageCombatFrames = 20;
                Player.Calamity().rage += 0.02f;
            }
            Player.Calamity().RageDamageBoost += DemonshadeLevel / 100f;
            if (CalamityKeybinds.RageHotKey.JustPressed)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/DemonshadeEnrage"), Player.Center);
                for (int i = 0; i < 36; i++)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(Player.position.X, Player.position.Y + 16f), Player.width, Player.height - 16, DustID.LifeDrain, 0f, 0f, 0, default, 1f);
                    dust.velocity *= 3f;
                    dust.scale *= 1.15f;
                }
                int num507 = 36;
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = (Vector2.Normalize(Player.velocity) * new Vector2(Player.width / 2f, Player.height) * 0.75f).RotatedBy((double)((i - (num507 / 2 - 1)) * 6.2831855f / num507), default) + Player.Center;
                    Vector2 speed = pos - Player.Center;
                    Dust dust = Dust.NewDustDirect(pos + speed, 0, 0, DustID.LifeDrain, speed.X * 1.5f, speed.Y * 1.5f, 100, default, 1.4f);
                    dust.noGravity = true;
                    dust.noLight = true;
                    dust.velocity = speed;
                }
                if (Player.whoAmI == Main.myPlayer)
                {
                    Player.AddBuff(ModContent.BuffType<Enraged>(), 600, false, false);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && !npc.dontTakeDamage && Vector2.Distance(Player.Center, npc.Center) <= 3000f)
                        {
                            npc.AddBuff(ModContent.BuffType<Enraged>(), 600, false);
                        }
                    }
                }
            }
        }
        public void DemonshadeHitEffect(int damage)
        {
            if (Player.Calamity().rageModeActive && DemonshadeLevel < 25)
            {
                DemonshadeXP += damage;
            }

            if (DemonshadeXP > 20000 * DemonshadeLevel + 4000 && DemonshadeLevel < 25)
            {

                DemonshadeLevel++;
                DemonshadeXP = 0;

            }

        }
        public void DemonshadeHurtEffect(int damage)
        {
            float rageIncrease = damage / 5f;
            if (rageIncrease > 20) rageIncrease = 20;
            Player.Calamity().rage += rageIncrease;
            if (Player.Calamity().rage > Player.Calamity().rageMax * 2f)
            {
                Player.Calamity().rage = Player.Calamity().rageMax * 2f;
            }
        }

    }
}
