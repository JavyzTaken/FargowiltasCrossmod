using FargowiltasSouls.Content.Items.Accessories.Forces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Particles;
using Luminance.Common.Utilities;
using Terraria.DataStructures;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationForce : BaseForce
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ExplorationEffect>(Item);
            //player.AddEffect<AerospecJumpEffect>(Item);
            //MarniteEnchant.AddEffects(player, Item);
            //player.AddEffect<DesertProwlerEffect>(Item);
            //player.AddEffect<WulfrumEffect>(Item);
            //SulphurEnchant.AddEffects(player, Item);
            //VictideEnchant.AddEffects(player, Item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<AerospecEnchant>());
            recipe.AddIngredient(ModContent.ItemType<DesertProwlerEnchant>());
            //recipe.AddIngredient(ModContent.ItemType<MarniteEnchant>());
            //recipe.AddIngredient(ModContent.ItemType<WulfrumEnchant>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SulphurEnchant>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VictideEnchant>(), 1);
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
    }
    public class ExplorationEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ExplorationHeader>();
        public override int ToggleItemType => ModContent.ItemType<ExplorationForce>();
        public bool JumpNoDown = false;
        public bool Slamming = false;
        public override void PostUpdateEquips(Player player)
        {
            
            player.GetCritChance(DamageClass.Generic) += player.CalamityAddon().ExploCritBoost;
            player.GetDamage(DamageClass.Summon) += player.CalamityAddon().ExploCritBoost / 100f;
            player.GetJumpState<ExplorationJump>().Enable();
            player.jumpSpeedBoost += 1;
            player.maxRunSpeed += 1.25f;
            player.runAcceleration *= 1.2f;
            player.noFallDmg = true;
            //Main.NewText(JumpNoUp);
            if (player.GetJumpState<ExplorationJump>().Active && !player.controlDown)
            {
                JumpNoDown = true;
            }
            else
            {
                JumpNoDown = false;
            }
            if (player.controlUp && player.GetJumpState<ExplorationJump>().Active)
            {
                player.jumpSpeedBoost += 8;
            }
            if (player.controlDown && !JumpNoDown && player.GetJumpState<ExplorationJump>().Active)
            {
                player.StopExtraJumpInProgress() ;
                Slamming = true;
                player.velocity.Y = 20;
                player.maxFallSpeed = 20;
            }
            if (!player.controlDown)
            {
                Slamming = false;
            }
            if (Slamming)
            {
                player.maxFallSpeed = 15;
                if (Main.rand.NextBool())
                {
                    Vector2 pos = player.Bottom + new Vector2(Main.rand.NextFloat(-10, 11), Main.rand.NextFloat(-1, 2));
                    Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * new Vector2(Main.rand.NextFloat(0, 0.5f), Main.rand.NextFloat(0, 0.5f));
                    
                    if (Main.rand.NextBool())
                    {
                        GenericSparkle p = new GenericSparkle(pos, vel, Color.SkyBlue, Color.White, 0.7f, 40, Main.rand.NextFloat(-0.2f, 0.2f));
                        GeneralParticleHandler.SpawnParticle(p);
                    }
                    else
                    {
                        SparkleParticle p = new SparkleParticle(pos, vel, Color.SkyBlue, Color.White, 0.7f, 60, Main.rand.NextFloat(-0.2f, 0.2f));
                        GeneralParticleHandler.SpawnParticle(p);
                    }
                    if (Main.rand.NextBool(4))
                    {
                        Gore gore = Main.gore[Gore.NewGore(player.GetSource_EffectItem<ExplorationEffect>(), player.Bottom - new Vector2(15, 0), Vector2.Zero, Main.rand.Next(11, 14))];
                        gore.velocity *= 0.5f;
                    }
                }
            }
            base.PostUpdateEquips(player);
        }
    }
    public class ExplorationJump : ExtraJump
    {
        
        public override float GetDurationMultiplier(Player player)
        {
            return 1.5f;
        }
        public override Position GetDefaultPosition()
        {
            return new Before(ExtraJump.CloudInABottle);
        }
        
        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 4;
            player.maxRunSpeed *= 2;
            base.UpdateHorizontalSpeeds(player);
        }
        public override void OnStarted(Player player, ref bool playSound)
        {
            player.CalamityAddon().ExploCritBoost += 5;
            float boost = player.CalamityAddon().ExploCritBoost;
            float rotation = 0;
            if (boost == 5 || boost == 10) rotation += 20;
            if (boost == 15 || boost == 20) rotation -= 20;
            if (boost == 25 || boost == 30) rotation += 40;
            if (boost == 35 || boost == 40) rotation -= 40;
            if (player.CalamityAddon().ExploCritBoost % 10 == 0) 
                rotation += 180;
            rotation = MathHelper.ToRadians(rotation);
            if (boost > 45)
            {
                player.CalamityAddon().ExploCritBoost = 0;
            }
            else if (boost <= 40)
            {
                Projectile.NewProjectileDirect(player.GetSource_EffectItem<ExplorationEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<ExplorationFeather>(), FargoSoulsUtil.HighestDamageTypeScaling(player, 300), 5, player.whoAmI, rotation);
            }
            player.GetJumpState<ExplorationJump>().Available = true;
            for (int i = 0; i < player.width; i += 5)
            {
                Gore gore = Main.gore[Gore.NewGore(player.GetSource_EffectItem<ExplorationEffect>(), player.BottomLeft + new Vector2(i - 15, Main.rand.NextFloat(-1, 2)), Vector2.Zero, Main.rand.Next(11, 14))];
                gore.velocity *= 0.5f;
            }
            for (int i = -5; i < player.width+5; i += 4)
            {
                Vector2 pos = player.BottomLeft + new Vector2(i, Main.rand.NextFloat(-1, 2));
                Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * new Vector2(Main.rand.NextFloat(1.2f, 1.8f), Main.rand.NextFloat(0, 0.5f));
                if (Main.rand.NextBool())
                {
                    GenericSparkle p = new GenericSparkle(pos, vel, Color.SkyBlue, Color.White, 1, 40, Main.rand.NextFloat(-0.2f, 0.2f));
                    GeneralParticleHandler.SpawnParticle(p);
                }
                else
                {
                    SparkleParticle p = new SparkleParticle(pos, vel, Color.SkyBlue, Color.White, 1, 60, Main.rand.NextFloat(-0.2f, 0.2f));
                    GeneralParticleHandler.SpawnParticle(p);
                }

            }
            
            base.OnStarted(player, ref playSound);
        }
        public override void ShowVisuals(Player player)
        {
            if (Main.rand.NextBool())
            {
                return;
            }
            Vector2 pos = player.Bottom + new Vector2(Main.rand.NextFloat(-10, 11), Main.rand.NextFloat(-1, 2));
            Vector2 vel = Vector2.UnitX.RotateRandom(MathHelper.TwoPi) * new Vector2(Main.rand.NextFloat(0, 0.5f), Main.rand.NextFloat(0, 0.5f));
            
            if (Main.rand.NextBool())
            {
                GenericSparkle p = new GenericSparkle(pos, vel, Color.SkyBlue, Color.White, 0.7f, 40, Main.rand.NextFloat(-0.2f, 0.2f));
                GeneralParticleHandler.SpawnParticle(p);
            }
            else
            {
                SparkleParticle p = new SparkleParticle(pos, vel, Color.SkyBlue, Color.White, 0.7f, 60, Main.rand.NextFloat(-0.2f, 0.2f));
                GeneralParticleHandler.SpawnParticle(p);
            }
            if (Main.rand.NextBool(4))
            {
                Gore gore = Main.gore[Gore.NewGore(player.GetSource_EffectItem<ExplorationEffect>(), player.Bottom - new Vector2(15, 0), Vector2.Zero, Main.rand.Next(11, 14))];
                gore.velocity *= 0.5f;
            }
            base.ShowVisuals(player);
        }
    }
}
