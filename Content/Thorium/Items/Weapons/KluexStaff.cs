using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using FargowiltasCrossmod.Content.Thorium.Projectiles;

namespace FargowiltasCrossmod.Content.Thorium.Items.Weapons
{
    // Todo: Fix rotation when held to be like a normal staff
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class KluexStaff : ModItem
    {

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            //DisplayName.SetDefault("Staff of Copyrighted Content");
            //Tooltip.SetDefault("Summons self-aiming orbs at the cursor. Lwft click summons red damage orbs and right click summons green healing orbs. Use speed increases with use.");
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DamageType = ThoriumMod.ThoriumDamageBase<ThoriumMod.HealerDamage>.Instance;
            Item.damage = 15;
            Item.mana = 10;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 6);
            Item.rare = ItemRarityID.Yellow;

            Item.autoReuse = true;
            Item.shootSpeed = 0f;
            Item.channel = true;
            Item.InterruptChannelOnHurt = true;
            Item.shoot = ModContent.ProjectileType<DmgOrbHack>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanShoot(Player player)
        {
            Item.shoot = player.altFunctionUse == 2 ? ModContent.ProjectileType<HealOrbHack>() : ModContent.ProjectileType<DmgOrbHack>();
            Item.DamageType = player.altFunctionUse == 2 ? ThoriumMod.ThoriumDamageBase<ThoriumMod.HealerTool>.Instance : ThoriumMod.ThoriumDamageBase<ThoriumMod.HealerDamage>.Instance;
            return base.CanShoot(player);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.useTime = (int)MathF.Max(Item.useTime - 10, 10);
            Item.useAnimation = (int)MathF.Max(Item.useAnimation - 10, 10);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && !Main.mouseLeft && !Main.mouseRight || player.statMana < Item.mana)
            {
                Item.useTime = 120;
                Item.useAnimation = 120;
            }
        }

        private class HealOrbHack : KluexOrb
        {
            public override string Texture => "FargowiltasCrossmod/Content/Thorium/Projectiles/KluexOrb";
            public override void OnSpawn(IEntitySource source)
            {
                //Projectile.ai[0] = StaffHeal;
                base.OnSpawn(source);
            }
        }
        private class DmgOrbHack : KluexOrb
        {
            public override string Texture => "FargowiltasCrossmod/Content/Thorium/Projectiles/KluexOrb";
            public override void OnSpawn(IEntitySource source)
            {
                //Projectile.ai[0] = StaffDmg;
                base.OnSpawn(source);
            }
        }
    }
}
