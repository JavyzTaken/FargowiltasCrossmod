using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GraniteEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => Color.DarkBlue;
        internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.GraniteEnch && DLCPlayer.BronzeEnch;

        protected override Color SynergyColor1 => Color.DarkBlue with { A = 0 };
        protected override Color SynergyColor2 => Color.Gold with { A = 0 };
        internal override int SynergyEnch => ModContent.ItemType<BronzeEnchant>();

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.GraniteEnch = true;
            DLCPlayer.GraniteEnchItem = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.Granite.GraniteHelmet>()
                .AddIngredient<ThoriumMod.Items.Granite.GraniteChestGuard>()
                .AddIngredient<ThoriumMod.Items.Granite.GraniteGreaves>()
                .Register();
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void GraniteEffect(Vector2 pos, Projectile proj)
        {
            if (SynergyEffect(GraniteEnchItem.type) && !Player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().ForceEffect(GraniteEnchItem.type)) return;

            if (proj.type != ModContent.ProjectileType<GraniteExplosion>() || !Main.rand.NextBool(3))
            {
                Projectile.NewProjectileDirect(Player.GetSource_Accessory(GraniteEnchItem), pos, Vector2.Zero, ModContent.ProjectileType<GraniteExplosion>(), 0, 0f, Player.whoAmI, 1f);
            }
        }
    }
}