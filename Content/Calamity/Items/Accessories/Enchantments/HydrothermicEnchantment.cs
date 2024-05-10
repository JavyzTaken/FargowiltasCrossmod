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
using Luminance.Core.Graphics;
using FargowiltasCrossmod.Assets.Particles;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using CalamityMod.Projectiles.Typeless;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class HydrothermicEnchantment : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Color nameColor => new Color(248, 182, 89);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<HydrothermicEffect>(Item);
            player.CalamityAddon().HydrothermicHide = hideVisual;
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyHydrothermHelms");
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicArmor>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicSubligar>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Helstorm>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.HavocsBreath>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.SlagsplitterPauldron>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class HydrothermicEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Header ToggleHeader => Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<HydrothermicEnchantment>();

        public override void PostUpdateEquips(Player player)
        {
            int maxCharge = 800;
            float attackSpeed = 0.4f;
            if (player.ForceEffect<HydrothermicEffect>())
            {
                maxCharge = 1400;
                attackSpeed = 0.6f;
            }
            CalamityAddonPlayer dlc = player.CalamityAddon();
            if (player.controlUseItem && player.HeldItem.damage > 0 && player.HeldItem.shoot > 0 && !dlc.Overheating)
            {
                dlc.ThermalCharge += 2;
            }
            else if (!dlc.Overheating && dlc.ThermalCharge > 0)
            {
                dlc.ThermalCharge--;
            }
            if (dlc.Overheating)
            {
                player.GetAttackSpeed(DamageClass.Generic) += attackSpeed;
                player.endurance -= 20;
                dlc.ThermalCharge -= 3;
                if (dlc.ThermalCharge <= 0)
                {
                    dlc.Overheating = false;
                    dlc.ThermalCharge = 0;
                }
                if (!dlc.HydrothermicHide)
                {
                    player.Calamity().ProvidenceBurnEffectDrawer.ParticleSpawnRate = 1;
                    player.Calamity().ProvidenceBurnEffectDrawer.Update();
                }
                if (!player.CCed)
                {
                    player.controlUseItem = true;
                    player.releaseUseItem = true;
                }
            }
            if (dlc.ThermalCharge >= maxCharge && !dlc.Overheating)
            {
                dlc.Overheating = true;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/ThanatosVent"), player.Center);
                
            }
            
            
            
            //Main.NewText(dlc.ThermalCharge);
        }
        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(player, drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            int maxCharge = 800;
            if (player.ForceEffect<HydrothermicEffect>())
            {
                maxCharge = 1400;
            }
            if (drawInfo.shadow == 0f && !player.CalamityAddon().HydrothermicHide)
            {
                
                Asset<Texture2D> t = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                Main.spriteBatch.SetBlendState(BlendState.Additive);
                Main.spriteBatch.Draw(t.Value, player.Center - Main.screenPosition, null, Color.Orange * (player.CalamityAddon().ThermalCharge / (float)(maxCharge+1)), 0, t.Size() / 2, 1, SpriteEffects.None, 0);
                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            }
            
        }
    }
}
