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
using FargowiltasCrossmod.Core.Calamity.ModPlayers;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.ModPlayers;
using Mono.Cecil;
using FargowiltasSouls.Assets.Sounds;
using CalamityMod.Buffs.StatBuffs;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("HydrothermicEnchantment")]
    public class HydrothermicEnchant : BaseEnchant
    {
        public static readonly Color NameColor = new Color(248, 182, 89);
        public override Color nameColor => NameColor;

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
        public override Header ToggleHeader => Header.GetHeader<ElementsHeader>();
        public override int ToggleItemType => ModContent.ItemType<HydrothermicEnchant>();

        public const int MaxHeat = 60 * 8;
        public override void PostUpdateEquips(Player player)
        {
            var dlc = player.CalamityAddon();
            var modPlayer = player.FargoSouls();
            bool force = player.ForceEffect<HydrothermicEffect>();

            bool disabled = player.HasEffect<ElementsForceEffect>() && dlc.ReaverToggle;

            if (player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("HydrothermicHeat", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/HydrothermicEnchant").Value, HydrothermicEnchant.NameColor,
                    () => Main.LocalPlayer.CalamityAddon().HydrothermicHeat / MaxHeat, true, 60 * 10, activeFunction: player.HasEffect<HydrothermicEffect>);

            float heatLevel = dlc.HydrothermicHeat / MaxHeat;
            if (player.HasEffectEnchant<HydrothermicEffect>())
            {
                player.endurance += (force ? 0.45f : 0.3f) * heatLevel;
            }
                
            if (dlc.HydrothermicOverheat)
            {
                player.Calamity().ProvidenceBurnEffectDrawer.ParticleSpawnRate = 1;
                player.Calamity().ProvidenceBurnEffectDrawer.Update();

                dlc.HydrothermicFlareCooldown -= heatLevel / 10f;
                if (dlc.HydrothermicFlareCooldown <= 0)
                {
                    dlc.HydrothermicFlareCooldown = 1;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int flareDamage = force ? 350 : 200;
                        if (player.HasEffect<ElementsForceEffect>())
                            flareDamage = 1000;
                        flareDamage = FargoSoulsUtil.HighestDamageTypeScaling(player, flareDamage);
                        Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, player.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.PiOver2 * 0.25f) * Main.rand.NextFloat(13f, 17f), 
                            ModContent.ProjectileType<HydrothermicVentShot>(), flareDamage, 2f, player.whoAmI);
                    }
                }

                float time = 60 * 3;
                if (force)
                    time *= 1.75f;
                dlc.HydrothermicHeat -= MaxHeat / time; // depletion time is 3 seconds
                if (dlc.HydrothermicHeat <= 0)
                {
                    dlc.HydrothermicHeat = 0;
                    dlc.HydrothermicOverheat = false;
                }
            }
            else
            {
                if (modPlayer.WeaponUseTimer > 0 && !disabled)
                {
                    if (dlc.HydrothermicHeat < MaxHeat)
                        dlc.HydrothermicHeat += 1;
                    if (dlc.HydrothermicHeat == MaxHeat - 1 && player.whoAmI == Main.myPlayer)
                        SoundEngine.PlaySound(FargosSoundRegistry.ChargeSound, player.Center);
                }
                else
                {
                    if (dlc.HydrothermicHeat >= MaxHeat)
                    {
                        Overheat(player);
                    }
                    else
                    {
                        if (dlc.HydrothermicHeat > 0)
                            dlc.HydrothermicHeat -= 1;
                    }
                }
            }
        }
        public override void OnHitByEither(Player player, NPC npc, Projectile proj)
        {
            var dlc = player.CalamityAddon();
            if (dlc.HydrothermicHeat > MaxHeat / 3)
                Overheat(player);
        }
        public static void Overheat(Player player)
        {
            if (!player.CalamityAddon().HydrothermicOverheat)
            {
                player.CalamityAddon().HydrothermicOverheat = true;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/ThanatosVent"), player.Center);

                if (player.HasEffect<ElementsForceEffect>())
                {
                    player.AddBuff(ModContent.BuffType<ReaverRage>(), 60 * 10);
                }
            }
        }
        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(player, drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            if (drawInfo.shadow == 0f)
            {
                Asset<Texture2D> t = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                Main.spriteBatch.SetBlendState(BlendState.Additive);
                Main.spriteBatch.Draw(t.Value, player.Center - Main.screenPosition, null, Color.Orange * (player.CalamityAddon().HydrothermicHeat / MaxHeat), 0, t.Size() / 2, 1, SpriteEffects.None, 0);
                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            }
            
        }
    }
}
