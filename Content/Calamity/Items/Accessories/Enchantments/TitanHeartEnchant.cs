using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using System.Security.Policy;
using Terraria.Graphics.Renderers;
using CalamityMod.Graphics.Renderers;
using Terraria.Graphics;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using CalamityMod.Projectiles.Turret;
using CalamityMod.Particles;
using Terraria.Audio;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using CalamityMod.Items.Armor.TitanHeart;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("TitanHeartEnchantment")]
    public class TitanHeartEnchant : BaseEnchant
    {

        public override Color nameColor => new Color(102, 96, 117);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TitanHeartEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<TitanHeartMask>(1);
            recipe.AddIngredient<TitanHeartMantle>(1);
            recipe.AddIngredient<TitanHeartBoots>(1);
            recipe.AddIngredient<StressPills>(1);
            recipe.AddIngredient<GacruxianMollusk>(1);
            recipe.AddIngredient<UrsaSergeant>(1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class TitanHeartEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<TitanHeartEnchant>();
        
        public override void PostUpdateEquips(Player player)
        {
            var calPlayer = player.Calamity();
            bool wiz = player.ForceEffect<TitanHeartEffect>();

            // dr
            float dr = wiz ? 0.3f : 0.15f;
            if (calPlayer.adrenaline > 0 && calPlayer.adrenaline < calPlayer.adrenalineMax)
                player.endurance += dr * calPlayer.adrenaline / calPlayer.adrenalineMax;
            // charge speed when grounded
            if (player.velocity.Y == 0) // grounded
            {
                // cal adren conditions
                bool wofAndNotHell = Main.wofNPCIndex >= 0 && player.position.Y < (float)(Main.UnderworldLayer * 16);
                if (CalamityPlayer.areThereAnyDamnBosses && calPlayer.AdrenalineEnabled && calPlayer.adrenalinePauseTimer == 0 && !wofAndNotHell)
                {
                    float defaultRate = calPlayer.adrenalineMax / calPlayer.AdrenalineChargeTime; // base cal charge rate, do not change
                    float balanceFactor = 0.75f; // change this to tune charge speed
                    float wizMod = wiz ? 1.6f : 1f; // change this to tune wizard buff

                    calPlayer.adrenaline += defaultRate * balanceFactor * wizMod;

                    // base cal "adren full" trigger
                    if (calPlayer.adrenaline >= calPlayer.adrenalineMax)
                    {
                        calPlayer.adrenaline = calPlayer.adrenalineMax;

                        // Play a sound when the Adrenaline Meter is full
                        if (player.whoAmI == Main.myPlayer && calPlayer.playFullAdrenalineSound)
                        {
                            calPlayer.playFullAdrenalineSound = false;
                            SoundEngine.PlaySound(CalamityPlayer.AdrenalineFilledSound);
                        }
                    }
                    else
                    {
                        calPlayer.playFullAdrenalineSound = true;

                        // dust
                        for (int i = 0; i < 1; i++)
                        {
                            Vector2 pos = player.Bottom;
                            pos.X += Main.rand.NextFloat(-50, 50);
                            Vector2 aimPos = player.Center + Main.rand.NextVector2Circular(player.width / 2, player.width / 2);
                            Vector2 aim = (aimPos - pos) / 10;
                            int d = Dust.NewDust(pos, 0, 0, DustID.PurpleTorch, aim.X, aim.Y, Scale: 2);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = aim;
                        }
                    }
                }
            }
        }
    }
}
