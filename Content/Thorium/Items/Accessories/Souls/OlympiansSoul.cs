using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Tiles.SunkenSea;
using Fargowiltas.Items.Tiles;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Essences;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.BossBuriedChampion;
using ThoriumMod.Items.BossStarScouter;
using ThoriumMod.Items.BossThePrimordials.Aqua;
using ThoriumMod.Items.Depths;
using ThoriumMod.Items.Granite;
using ThoriumMod.Items.Mechs;
using ThoriumMod.Items.NPCItems;
using ThoriumMod.Items.Terrarium;
using ThoriumMod.Items.ThrownItems;
using ThoriumMod.Items.Valadium;
using ThoriumMod.Utilities;
using FargowiltasSouls.Content.Items.Accessories.Souls;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Souls
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class OlympiansSoul : BaseSoul
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetDamage(DamageClass.Throwing) += 0.3f;
            player.GetCritChance(DamageClass.Throwing) += 0.15f;
            player.ThrownVelocity += 0.15f;
            ModContent.GetInstance<ThrowingGuideVolume3>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<MermaidCanteen>().UpdateAccessory(player, hideVisual);
            ModContent.GetInstance<DeadEyePatch>().UpdateAccessory(player, hideVisual);
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type)
                .AddIngredient(ModContent.ItemType<SlingersEssence>())
                .AddIngredient(ModContent.ItemType<MagnetoGrip>())
                .AddIngredient(ModContent.ItemType<ThrowingGuideVolume3>())
                .AddIngredient(ModContent.ItemType<MermaidCanteen>())
                .AddIngredient(ModContent.ItemType<DeadEyePatch>())
                .AddIngredient(ModContent.ItemType<TerrariumRippleKnife>())
                .AddIngredient(ModContent.ItemType<VoltHatchet>())
                .AddIngredient(ModContent.ItemType<TidalWave>())
                .AddIngredient(ModContent.ItemType<PharaohsSlab>())
                .AddIngredient(ModContent.ItemType<TerraKnife>())
                .AddIngredient(ModContent.ItemType<AngelsEnd>())
                .AddIngredient(ModContent.ItemType<LihzahrdKukri>(), 300)
                .AddIngredient(ModContent.ItemType<CrystalBalloon>(), 300)
                .AddIngredient(ModContent.ItemType<WhiteDwarfKunai>(), 600)
                .AddTile(ModContent.TileType<CrucibleCosmosSheet>())
                .Register();
        }
    }
}