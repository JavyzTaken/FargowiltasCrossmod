using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using ThoriumMod.Items.HealerItems;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Souls
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ArchangelSoul : BaseSoul
    {
        protected override Color? nameColor => new(255, 0, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var thoriumPlayer = player.Thorium();
            player.GetDamage<ThoriumMod.HealerDamage>() += 0.30f;
            player.GetCritChance<ThoriumMod.HealerDamage>() += 0.15f;
            player.GetAttackSpeed<ThoriumMod.HealerDamage>() += 0.15f;
            player.GetAttackSpeed<ThoriumMod.HealerTool>() += 0.15f;
            thoriumPlayer.healBonus += 6;

            player.statLifeMax += 50;
            thoriumPlayer.darkAura = true;

            thoriumPlayer.medicalAcc = true;
            thoriumPlayer.headMirror.Set(Item);
            if (Main.myPlayer != player.whoAmI && Main.netMode != NetmodeID.Server)
            {
                Main.LocalPlayer.Thorium().needsOutOfCombatSync = true;
            }

            player.aggro -= 400;
            thoriumPlayer.graveGoods = true;
            //int aegisBuff = ModContent.BuffType<ThoriumMod.Buffs.Healer.>();
            //player.AddBuff(aegisBuff, 30, true, false);
            //if (Main.netMode == 1 && Main.myPlayer != player.whoAmI)
            //{
            //    Player localPlayer = Main.LocalPlayer;
            //    if (localPlayer.DistanceSQ(player.Center) < 160000f)
            //    {
            //        localPlayer.AddBuff(aegisBuff, 30, true, false);
            //    }
            //}

            thoriumPlayer.ascensionStatuette = true;
            thoriumPlayer.rebirthStatuette = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ArchangelHeart>()
                .AddIngredient<ArchDemonCurse>()
                .AddIngredient<MedicalBag>()
                .AddIngredient<SoulGuard>()
                .AddIngredient<NirvanaStatuette>()

                .AddIngredient<LifeAndDeath>()
                .AddIngredient<NecroticStaff>()
                .AddIngredient<SunrayStaff>()
                .AddIngredient<AncientTome>()
                .AddIngredient<ThoriumMod.Items.Donate.ValhallasDescent>()
                .AddIngredient<TerraScythe>()
                .AddIngredient<ThoriumMod.Items.Terrarium.TerrariumHolyScythe>()
                .AddIngredient<LightBringerWarhammer>()
                .AddIngredient<MindMelter>()
                .AddIngredient<MolecularStabilizer>()
                .AddIngredient<Essences.MendersEssence>()
                .AddTile<Fargowiltas.Items.Tiles.CrucibleCosmosSheet>()
                .Register();
        }
    }
}