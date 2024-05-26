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
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using CalamityMod;
using FargowiltasSouls.Common.Graphics.Particles;
using CalamityMod.Particles;
using Terraria.Audio;
using FargowiltasSouls;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Armor.DesertProwler;
using FargowiltasCrossmod.Core.Calamity;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DesertProwlerEnchantment : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Color nameColor => new Color(102, 89, 54);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Blue;
            Item.buyPrice(gold: 10);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<DesertProwlerEffect>(Item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<DesertProwlerHat>();
            recipe.AddIngredient<DesertProwlerShirt>();
            recipe.AddIngredient<DesertProwlerPants>();
            recipe.AddIngredient<CrackshotColt>();
            recipe.AddIngredient<SunSpiritStaff>();
            recipe.AddIngredient<Cinquedea>();
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DesertProwlerEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<DesertProwlerEnchantment>();
        public override void PostUpdateEquips(Player player)
        {
            CalDLCAddonPlayer addonPlayer = player.CalamityAddon();
            if (player.rocketBoots != 0)
            {
                player.rocketBoots = 0;
                addonPlayer.DesertProwlerRocketPower = true;
            }
            else
            {
                addonPlayer.DesertProwlerRocketPower = false;
            }
        }
        public static void ProwlerEffect(Player player)
        {
            
            CalDLCAddonPlayer cplayer = player.CalamityAddon();
            
            if (cplayer.ProwlerCharge < 15 && player.controlJump)
            {
                Vector2 pos = player.Bottom + new Vector2(Main.rand.Next(30, 50) * (Main.rand.NextBool() ? 1 : -1), -Main.rand.Next(10, 20));
                FargowiltasSouls.Common.Graphics.Particles.SparkParticle spark = new FargowiltasSouls.Common.Graphics.Particles.SparkParticle(pos, (player.Bottom - pos).SafeNormalize(Vector2.Zero) * 1, new Color(255, 226, 145, 200) * 0.5f, 0.5f, 30);
                CalamityMod.Particles.Particle p = new TimedSmokeParticle(pos, (player.Bottom - pos).SafeNormalize(Vector2.Zero) * 6 + player.velocity, new Color(230, 206, 125, 200) * 0.1f, new Color(255, 226, 145, 200) * 0.5f, 1, 1, 30);
                GeneralParticleHandler.SpawnParticle(p);
                spark.Spawn();
                cplayer.ProwlerCharge += 0.15f;
            }
            if ((!player.controlJump || (cplayer.AutoProwler && cplayer.ProwlerCharge >= 15)) && Collision.SolidCollision(player.BottomLeft, player.width, 6, true) && cplayer.ProwlerCharge > 0)
            {

                float horizontalPower = player.ForceEffect<DesertProwlerEffect>() ? 1f : 0.65f;
                float verticalPower = horizontalPower;
                float maxSpeed = player.ForceEffect<DesertProwlerEffect>() ? 20f : 15f;
                cplayer.ProwlerCharge += 10;
                if (cplayer.DesertProwlerRocketPower)
                    verticalPower += 0.2f;
                player.velocity.Y = -cplayer.ProwlerCharge * verticalPower - player.jumpSpeedBoost;
                player.velocity.X *= cplayer.ProwlerCharge / (6f / horizontalPower);
                player.velocity.X = MathHelper.Clamp(player.velocity.X, -maxSpeed, maxSpeed);
                for (int i = 0; i < player.width+10; i++)
                {
                    float x = (i / (float)(player.width+10));
                    float lerper = (float)Math.Pow(Math.E, -Math.Pow((6 * x - 3), 2)); //bell curve
                    Vector2 pos = (player.Bottom - new Vector2(5, 0)) + new Vector2(i, 0);
                    CalamityMod.Particles.Particle p = new TimedSmokeParticle(pos, player.velocity * MathHelper.Lerp(0.1f, 0.5f, lerper), new Color(230, 206, 125, 200) * 0.1f, new Color(255, 226, 145, 200) * 0.5f, 1, 1, 30);
                    GeneralParticleHandler.SpawnParticle(p);
                }
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/DesertProwlerSmokeBomb"), player.Center);
                cplayer.ProwlerCharge = 0;
                player.RefreshExtraJumps();
                player.RemoveAllGrapplingHooks();
            }
            if (!player.controlJump)
            {
                cplayer.ProwlerCharge = 0;
            }
            if (Collision.SolidCollision(player.BottomLeft, player.width, 6, true))
            {
                player.controlJump = false;
                //player.jump = 1;
                player.releaseJump = false;
            }
        }
    }
}
