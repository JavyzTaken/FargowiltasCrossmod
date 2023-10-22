using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.BossBuriedChampion;
using ThoriumMod.Items.BossStarScouter;
using ThoriumMod.Items.Depths;
using ThoriumMod.Items.Granite;
using ThoriumMod.Items.NPCItems;
using ThoriumMod.Items.ThrownItems;
using ThoriumMod.Utilities;
using FargowiltasSouls.Content.Items.Accessories.Essences;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Essences
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SlingersEssence : BaseEssence
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        protected override Color nameColor => new Color(163, 102, 255);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetDamage(DamageClass.Throwing) += 0.18f;
            player.GetCritChance(DamageClass.Throwing) += 0.05f;
            player.ThrownVelocity += 0.05f;
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<NinjaEmblem>())
                .AddIngredient(ModContent.ItemType<EnchantedKnife>(), 100)
                .AddIngredient(ModContent.ItemType<SeaNinjaStar>(), 100)
                .AddIngredient(ModContent.ItemType<GraniteThrowingAxe>(), 300)
                .AddIngredient(ModContent.ItemType<MeteoriteClusterBomb>(), 375)
                .AddIngredient(ModContent.ItemType<ChampionsGodHand>())
                .AddIngredient(ModContent.ItemType<GaussFlinger>())
                .AddIngredient(ModContent.ItemType<BaseballBat>())
                .AddIngredient(ModContent.ItemType<LastingPliers>())
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
