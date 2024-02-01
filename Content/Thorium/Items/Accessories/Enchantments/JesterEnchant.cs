using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class JesterEnchant : BaseEnchant
    {
        public override Color nameColor => Color.DarkGreen;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.JesterEnch = true;
            DLCPlayer.JesterEnchItem = Item;

            if (currentEnchant == null || currentEnchant.ModItem == null || --enchantChangeCD <= 0)
            {
                currentEnchant = new Item(enchantItemTypes[Main.rand.Next(0, enchantItemTypes.Count)]);
                enchantChangeCD = 60 * 5;

                Projectile.NewProjectile(player.GetSource_Accessory(Item, currentEnchant.ModItem.Texture), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.JesterIndicatorProj>(), 0, 0, player.whoAmI);

                return;
            }

            if (currentEnchant == null || currentEnchant.ModItem == null)
            {
                Main.NewText("This shouldn't happen, tell Ghoose");
                return;
            }

            var soulsPlayer = player.FargoSouls();
            bool flag = false;
            Item oldwizItem = null;
            if (soulsPlayer.ForceEffect(Item.type))
            {
                oldwizItem = soulsPlayer.WizardedItem;
                flag = true;

                soulsPlayer.WizardedItem = currentEnchant; 
            }

            currentEnchant.ModItem.UpdateAccessory(player, true); 

            if (flag)
            {
                // reset the wizarded item, as it may have been a different enchant that comes after this one in update order.
                soulsPlayer.WizardedItem = oldwizItem;
            }
        }

        private int enchantChangeCD;

        private Item currentEnchant;

        internal static List<int> enchantItemTypes;
        internal static void PostSetup(Mod mod)
        {
            enchantItemTypes = new();

            var BaseEnchants = ModContent.GetContent<BaseEnchant>();
            foreach (var baseEnch in BaseEnchants)
            {
                if (baseEnch == null || baseEnch.Type <= 0)
                {
                    mod.Logger.Debug($"Enchant {baseEnch.Name} created a null or 0 itemType. This is probably due ot it being an abstract class");
                    continue;
                }

                if (baseEnch.Type == ModContent.ItemType<JesterEnchant>()) continue;

                enchantItemTypes.Add(baseEnch.Type);
            }

            //Assembly SoulsAssembly = Core.ModCompatibility.SoulsMod.Mod.GetType().Assembly;
            //Assembly DLCAssembly = mod.GetType().Assembly;

            //var soulsTypes = SoulsAssembly.GetTypes();
            //var DLCTypes = DLCAssembly.GetTypes();

            //// This could potentially break from Soulsmod or DLC somehow adding another class called BaseEnchant that has other classes deriving from it, or by adding classes that are not enchants but derive from BaseEnchant.
            //// If someone does this I will kill them. (ignore BaseSynergyEnchant its not real)
            //// Unfortunatly I cannot figure out how to get a System.Type for an abstract class like BaseEnchant (to use in EqualTo()), so this will have to do.
            //var enchantTypes = soulsTypes.Where(T => T.BaseType != null && T.BaseType.Name == "BaseEnchant");
            //enchantTypes = enchantTypes.Concat(DLCTypes.Where(T => T.BaseType != null && T.BaseType.Name == "BaseEnchant" && T.Name != "BaseSynergyEnchant"));

            //if (ModLoader.TryGetMod("FargowiltasSoulsDLC", out Mod Extras))
            //{
            //    var ExtrasTypes = Extras.GetType().Assembly.GetTypes();
            //    enchantTypes = enchantTypes.Concat(ExtrasTypes.Where(T => T.BaseType != null && T.BaseType.Name == "BaseEnchant"));
            //}

            //MethodInfo itemTypeMethod = typeof(ModContent).GetMethod("ItemType", BindingFlags.Public | BindingFlags.Static);

            //foreach (Type enchType in enchantTypes)
            //{
            //    // equivalent to ModContent.ItemType<enchType>();
            //    object enchID = itemTypeMethod.MakeGenericMethod(enchType).Invoke(null, null);

            //    if (enchID == null || (int)enchID == 0)
            //    {
            //        mod.Logger.Debug($"Type {enchType.Name} created a null or 0 itemType. This is likely due to the item not being loaded due to jit exceptions or isLoadingEnabled() returning false");
            //        continue;
            //    }

            //    enchantItemTypes.Add((int)enchID);
            //}

            //enchantItemTypes.Remove(ModContent.ItemType<JesterEnchant>());
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class JesterIndicatorProj : ModProjectile
    {
        public override string Texture => "FargowiltasCrossmod/icon"; // This is just to make sure that the projectile loads.
        private string _displayEnchant = ""; 

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 255 / 3;
        }

        public override void OnSpawn(IEntitySource source)
        {
            _displayEnchant = source.Context;

            try
            {
                Texture2D texture = ModContent.Request<Texture2D>(_displayEnchant).Value;
            }
            catch
            {
                Main.NewText($"Jester proj spawned with improper texture: '{_displayEnchant}'.");
                Projectile.timeLeft = 0;
                _displayEnchant = Texture;
                return;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(_displayEnchant).Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Bounds, Projectile.GetAlpha(lightColor), 0f, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }

        public override void AI()
        {
            Projectile.Center = Main.player[Projectile.owner].Center + Vector2.UnitY * 64f;
            Projectile.alpha += 3;
            Projectile.scale *= 1.01f;
        }
    }
}