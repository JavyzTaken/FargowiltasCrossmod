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
using FargowiltasSouls.Content.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod;
using FargowiltasSouls.Content.Items.Accessories.Souls;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("ExplorationForce")]
    public class GaleForce : BaseForce
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
            player.AddEffect<GaleJumpEffect>(Item);
            player.AddEffect<GaleSpineEffect>(Item);
            player.AddEffect<GaleSlowfallEffect>(Item);
            player.AddEffect<StatigelEffect>(Item);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            //recipe.AddIngredient(ModContent.ItemType<AerospecEnchant>());
            recipe.AddIngredient(ModContent.ItemType<DesertProwlerEnchant>());
            recipe.AddIngredient(ModContent.ItemType<SnowRuffianEnchant>());
            recipe.AddIngredient(ModContent.ItemType<StatigelEnchant>(), 1);
            recipe.AddIngredient(ModContent.ItemType<SulphurEnchant>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VictideEnchant>(), 1);
            recipe.AddTile(ModContent.TileType<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>());
            recipe.Register();
        }
        
    }
    public class GaleEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<GaleForce>();
    }
    public class GaleSlowfallEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<GaleHeader>();
        public override int ToggleItemType => ModContent.ItemType<SnowRuffianEnchant>();
        public override void PostUpdateEquips(Player player)
        {
            if (player.controlUp)
            {
                player.slowFall = true;
            }
            base.PostUpdateEquips(player);
        }
    }
    public class GaleSpineEffect : AccessoryEffect
    {
        //handled by explorationjump (only active if jump effect is also active)
        public override Header ToggleHeader => Header.GetHeader<GaleHeader>();
        public override int ToggleItemType => ModContent.ItemType<VictideEnchant>();
        public int SpineSpawnTimer = 0;
        public bool SpawnedAlready = false;
        public override void PostUpdate(Player player)
        {
            if (player.GetJumpState<GaleJump>().Active)
            {
                SpineSpawnTimer++;
                if (SpineSpawnTimer >= 10 && !SpawnedAlready)
                {
                    player.CalamityAddon().ExploFeatherCount += 1;
                    int count = (int)player.CalamityAddon().ExploFeatherCount;
                    float boost = player.CalamityAddon().ExploFeatherCount;
                    float rotation = 0;
                    if (count == 1 || count == 2) rotation = 40;
                    if (count == 3 || count == 4) rotation = 20;
                    if (count == 5 || count == 6) rotation = 0;
                    //rotation = 0;
                    rotation *= count % 2 == 0 ? 1 : -1;
                    rotation = MathHelper.ToRadians(rotation);
                    if (boost >= 8)
                    {
                        player.CalamityAddon().ExploFeatherCount = 0;
                    }
                    else if (boost <= 6)
                    {
                        Projectile.NewProjectileDirect(player.GetSource_EffectItem<GaleSpineEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<GaleSpine>(), FargoSoulsUtil.HighestDamageTypeScaling(player, 800), 5, player.whoAmI, rotation, player.CalamityAddon().ExploFeatherCount % 2 == 0 ? 1 : -1);
                    }
                    SpineSpawnTimer = 0;
                    SpawnedAlready = true;
                }
            }
            else
            {
                SpineSpawnTimer = 0;
                SpawnedAlready = false;
            }
                base.PostUpdate(player);
        }

    }
    public class GaleJumpEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<GaleHeader>();
        public override int ToggleItemType => ModContent.ItemType<GaleForce>();
        public bool JumpNoDown = false;
        public bool Slamming = false;
        public int SlamParticleTimer = 0;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasEffect<JumpsDisabled>())
            {
                player.wings = 0;
                player.wingsLogic = 0;
            }
            player.GetJumpState<GaleJump>().Enable();
            player.jumpSpeedBoost += 1;
            player.maxRunSpeed += 1.25f;
            player.runAcceleration *= 1.2f;
            player.noFallDmg = true;
            //Main.NewText(JumpNoUp);
            if (player.GetJumpState<GaleJump>().Active && !player.controlDown)
            {
                JumpNoDown = true;
            }
            else
            {
                JumpNoDown = false;
            }
            if (player.controlUp && player.GetJumpState<GaleJump>().Active)
            {
                player.jumpSpeedBoost += 8;
            }
            if (player.controlDown && !JumpNoDown && player.GetJumpState<GaleJump>().Active)
            {
                player.StopExtraJumpInProgress() ;
                Slamming = true;
                player.velocity.Y = 20;
                player.maxFallSpeed = 20;
            }
            if (player.jump == 15 && player.controlUp && Collision.SolidCollision(player.BottomLeft, player.width, 12, true))
            {
                player.controlJump = false;
                player.releaseJump = true;
            }
            if (!player.controlDown)
            {
                Slamming = false;
            }
            if (Slamming)
            {
                SlamParticleTimer++;
                player.maxFallSpeed = 15;
                if (SlamParticleTimer < 40)
                {
                    
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
                            Gore gore = Main.gore[Gore.NewGore(player.GetSource_EffectItem<GaleJumpEffect>(), player.Bottom - new Vector2(15, 0), Vector2.Zero, Main.rand.Next(11, 14))];
                            gore.velocity *= 0.5f;
                        }
                    }
                }
            }
            else
            {
                SlamParticleTimer = 0;
            }
                base.PostUpdateEquips(player);
        }
    }
    public class GaleJump : ExtraJump
    {
        
        public override float GetDurationMultiplier(Player player)
        {
            return 2.5f;
        }
        public override Position GetDefaultPosition()
        {
            return new Before(ExtraJump.CloudInABottle);
        }
        
        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 4;
            player.maxRunSpeed *= 1.75f;
            base.UpdateHorizontalSpeeds(player);
        }
        public override void OnStarted(Player player, ref bool playSound)
        {
            
            player.GetJumpState<GaleJump>().Available = true;
            for (int i = 0; i < player.width; i += 5)
            {
                Gore gore = Main.gore[Gore.NewGore(player.GetSource_EffectItem<GaleJumpEffect>(), player.BottomLeft + new Vector2(i - 15, Main.rand.NextFloat(-1, 2)), Vector2.Zero, Main.rand.Next(11, 14))];
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
                Gore gore = Main.gore[Gore.NewGore(player.GetSource_EffectItem<GaleJumpEffect>(), player.Bottom - new Vector2(15, 0), Vector2.Zero, Main.rand.Next(11, 14))];
                gore.velocity *= 0.5f;
            }
            base.ShowVisuals(player);
        }
    }
}
