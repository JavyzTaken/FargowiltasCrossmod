using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.BossMini;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
    public class NoviceClericEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => Color.White;
        internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.NoviceClericEnch && DLCPlayer.EbonEnch;

        internal override int SynergyEnch => ModContent.ItemType<EbonEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.NoviceClericEnch = true;
            DLCPlayer.NoviceClericEnchItem = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NoviceClericCowl>()
                .AddIngredient<NoviceClericTabard>()
                .AddIngredient<NoviceClericPants>()
                .AddIngredient<PalmCross>()
                .AddIngredient<HereticBreaker>()
                .AddIngredient<TheGoodBook>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void NoviceClericEffect()
        {
            if (!NoviceClericEnch)
            {
                NoviceClericCrosses = 0;
                NoviceClericTimer = 0;
                return;
            }

            NoviceClericTimer++;
            crossOrbitalRotation = Utils.RotatedBy(crossOrbitalRotation, -0.05, default);

            int maxCrosses = SynergyEffect(NoviceClericEnchItem.type) ? 8 : 5;

            if (NoviceClericCrosses < maxCrosses)
            {
                if (NoviceClericTimer > (SynergyEffect(NoviceClericEnchItem.type) ? 180 : 300))
                {
                    NoviceClericCrosses++;
                    NoviceClericTimer = 0;
                }
            }
            else
            {
                NoviceClericTimer = 0;
            }
        }

        public void NoviceClericOnManaUse()
        {
            if (NoviceClericCrosses <= 0 || !NoviceClericEnch) return;

            NoviceClericCrosses--;
            bool synergy = SynergyEffect(NoviceClericEnchItem.type);
            int burstNum = (synergy && NoviceClericCrosses > 0) ? 5 : 3;

            if ((synergy && NoviceClericCrosses % 2 == 0) || !synergy)
            {
                Projectile.NewProjectile(Player.GetSource_Accessory(NoviceClericEnchItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.HealPulse>(), 0, 0, Player.whoAmI, 5);
            }
            else
            {
                // ebon crux
                Vector2 vector = Player.Center.DirectionTo(Main.MouseWorld) * 8f;
                for (int i = 0; i < burstNum; i++)
                {
                    vector = Utils.RotatedBy(vector, MathF.Tau / burstNum);
                    Projectile.NewProjectile(Player.GetSource_Accessory(NoviceClericEnchItem), Player.Center, vector, ModContent.ProjectileType<Projectiles.DLCShadowWispPro>(), 15, 2f, Player.whoAmI, 0f, 0f, 0f);
                }
            }
        }
    }
}