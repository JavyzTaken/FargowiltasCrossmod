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
using FargowiltasCrossmod.Core.Calamity.Globals;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("PlaguebringerEnchantment")]
    public class PlaguebringerEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Color nameColor => new Color(128, 188, 67);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
            Item.value = 250000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<PlaguebringerEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Plaguebringer.PlaguebringerVisor>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Plaguebringer.PlaguebringerCarapace>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Plaguebringer.PlaguebringerPistons>());
            recipe.AddIngredient(ModContent.ItemType<BeeEnchant>());
            //recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.PlagueHive>()); they DELETED IT
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.EpidemicShredder>());
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PlaguebringerEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<PlaguebringerEnchant>();

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
        }

        public override void PostUpdateEquips(Player player)
        {
            float maxCharge = 400;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PlagueCloud>()] <= 0)
            {
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_EffectItem<PlaguebringerEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<PlagueCloud>(), 100, 0, player.whoAmI, player.whoAmI, 1);
                NetMessage.SendData(MessageID.SyncProjectile, number: proj.whoAmI);
            }
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc != null && npc.active && npc.Calamity().pFlames > 0 && npc.Distance(player.Center) < (player.ForceEffect<PlaguebringerEffect>() ? 450 : 250) && player.ownedProjectileCounts[ModContent.ProjectileType<PlagueCloud>()] < 2)
                {
                    player.CalamityAddon().PlagueCharge++;
                }
            }
            //player.CalamityDLC().PlagueCharge++;
            if (player.CalamityAddon().PlagueCharge >= maxCharge)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/NuclearTerrorDeath"), player.Center);
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_EffectItem<PlaguebringerEffect>(), player.Center, new Vector2(0, 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<PlagueCloud>(), player.ForceEffect<PlaguebringerEffect>() ? 800 : 400, 0, player.whoAmI);
                NetMessage.SendData(MessageID.SyncProjectile, number: proj.whoAmI);
                for (int i = 0; i < 50; i++)
                {
                    Particle part = new TimedSmokeParticle(player.Center, new Vector2(0, Main.rand.NextFloat(0, 40)).RotatedByRandom(MathHelper.TwoPi), Color.Green * 0, Color.LimeGreen, 2f, 0.5f, 100, Main.rand.NextFloat(-0.02f, 0.02f));
                    GeneralParticleHandler.SpawnParticle(part);
                }

                player.CalamityAddon().PlagueCharge = 0;
            }
        }
    }
}
