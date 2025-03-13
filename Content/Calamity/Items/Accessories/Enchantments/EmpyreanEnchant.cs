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
using Terraria.Audio;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using CalamityMod.Items.Armor.TitanHeart;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Items.Armor.Empyrean;
using CalamityMod.Items.Weapons.Magic;
using Luminance.Core.Graphics;
using FargowiltasSouls.Common.Graphics.Particles;
using rail;
using CalamityMod.Projectiles.Rogue;
using FargowiltasSouls.Content.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Projectiles.Typeless;
using Mono.Cecil;
using static System.Net.Mime.MediaTypeNames;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class EmpyreanEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override Color nameColor => new Color(20, 20, 100);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Purple;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<EmpyreanEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<EmpyreanMask>(1);
            recipe.AddIngredient<EmpyreanCloak>(1);
            recipe.AddIngredient<EmpyreanCuisses>(1);
            recipe.AddIngredient<TomeofFates>(1);
            recipe.AddIngredient<StarofDestruction>(1);
            recipe.AddIngredient<UtensilPoker>(1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EmpyreanEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<EmpyreanEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            var calPlayer = player.Calamity();
            bool wiz = player.ForceEffect<EmpyreanEffect>();
            var dlc = player.CalamityAddon();

            if (player.velocity.Length() <= 3 && dlc.EmpyreanCooldown == 0)
            {
                dlc.EmpyreanSlowTimer++;
                if (dlc.EmpyreanSlowTimer == 120)
                {
                    SoundEngine.PlaySound(CalamityPlayer.RogueStealthSound, player.Center);
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 pos = player.Center + new Vector2(Main.rand.NextFloat(10, 20), 0).RotatedByRandom(MathHelper.TwoPi);
                        Particle sp = new AlphaSparkParticle(pos, pos.AngleTo(player.Center).ToRotationVector2() * 4, Color.Black, 0.8f, 20, true, Color.LightSeaGreen);
                        sp.Spawn();
                    }
                    dlc.EmpyreanEmpowered = true;
                }
            }
            else if (dlc.EmpyreanSlowTimer < 120 && dlc.EmpyreanSlowTimer > 0)
            {
                dlc.EmpyreanSlowTimer--;

            }
            if (dlc.EmpyreanCooldown > 0)
            {
                CooldownBarManager.Activate("EmpyreanCooldown", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/EmpyreanEnchant").Value, new Color(20, 210, 120),
                () =>  dlc.EmpyreanCooldown / (float)dlc.EmpyreanCooldownMax);
            }
            else if (dlc.EmpyreanSlowTimer > 0)
            {
                CooldownBarManager.Activate("EmpyreanCooldown", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/EmpyreanEnchant").Value, new Color(20, 210, 120),
                () => dlc.EmpyreanSlowTimer / 120f);
            }

        }
        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (player.CalamityAddon().EmpyreanEmpowered)
            {
                if (player.ForceEffect<EmpyreanEffect>())
                {
                    player.Heal(80);
                    player.Calamity().rage += 40f;
                }
                else
                {
                    player.Heal(40);
                    player.Calamity().rage += 25f;
                }

                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 pos = target.Center;
                    Vector2 vel = new Vector2(10, 0).RotatedBy(Main.rand.NextFloat(0, MathF.PI));
                    if (projectile != null)
                    {
                        pos = projectile.Center;
                        vel = projectile.velocity.SafeNormalize(Vector2.Zero) * 30;
                    }
                    else if (item != null)
                    {
                        vel = player.AngleTo(target.Center).ToRotationVector2() * 30;
                    }
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile p = Projectile.NewProjectileDirect(GetSource_EffectItem(player), pos, vel.RotatedBy(MathHelper.ToRadians(i * 10 + 5)), ModContent.ProjectileType<EmpyreanStar>(), baseDamage, 1, player.whoAmI, ai0: target.whoAmI);
                        NetMessage.SendData(MessageID.SyncProjectile, number: p.whoAmI);
                    }
                }
                var addonPlayer = player.CalamityAddon();
                addonPlayer.EmpyreanEmpowered = false;
                addonPlayer.EmpyreanCooldown = (int)MathHelper.Lerp(12f, 4f, player.Calamity().rage / 100) * 60;
                addonPlayer.EmpyreanSlowTimer = 0;
                addonPlayer.EmpyreanCooldownMax = addonPlayer.EmpyreanCooldown;
            }
            
        }
    }
}
