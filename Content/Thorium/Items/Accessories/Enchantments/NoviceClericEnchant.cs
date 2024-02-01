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
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
    public class NoviceClericEnchant : BaseSynergyEnchant<EbonSynEffect>
    {
        public override Color nameColor => Color.White;
        internal override int SynergyEnch => ModContent.ItemType<EbonEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<NoviceClericEffect>(Item);
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

    [ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
    public class NoviceClericEffect : SynergyEffect<EbonSynEffect>
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.AlfheimHeader>();
        public override bool ExtraAttackEffect => true;
        public override int ToggleItemType => ModContent.ItemType<NoviceClericEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.crossOrbitalRotation = Utils.RotatedBy(DLCPlayer.crossOrbitalRotation, -0.05, default);

            if (DLCPlayer.NoviceClericCrosses < 0) DLCPlayer.NoviceClericCrosses = 0;

            if (player.controlUseItem)
            {
                return;
            }

            DLCPlayer.NoviceClericTimer++;

            int maxCrosses = SynergyActive(player) ? 8 : 5;

            if (DLCPlayer.NoviceClericCrosses < maxCrosses)
            {
                if (DLCPlayer.NoviceClericTimer > (SynergyActive(player) ? 180 : 300))
                {
                    DLCPlayer.NoviceClericCrosses++;
                    DLCPlayer.NoviceClericTimer = 0;
                }
            }
            else
            {
                DLCPlayer.NoviceClericTimer = 0;
            }
        }

        public override void TryAdditionalAttacks(Player player, int damage, DamageClass damageType)
        {
            var DLCPlayer = player.ThoriumDLC();
            if (DLCPlayer.NoviceClericCrosses <= 0) return;

            DLCPlayer.NoviceClericCrosses--;
            bool synergy = SynergyActive(player);
            int burstNum = (synergy && DLCPlayer.NoviceClericCrosses > 0) ? 5 : 3;

            if ((synergy && DLCPlayer.NoviceClericCrosses % 2 == 0) || !synergy)
            {
                Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.HealPulse>(), 0, 0, player.whoAmI, 5);
            }
            else
            {
                // ebon crux
                Vector2 vector = player.Center.DirectionTo(Main.MouseWorld) * 8f;
                for (int i = 0; i < burstNum; i++)
                {
                    vector = Utils.RotatedBy(vector, MathF.Tau / burstNum);
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, vector, ModContent.ProjectileType<Projectiles.DLCShadowWispPro>(), 15, 2f, player.whoAmI, 0f, 0f, 0f);
                }
            }
        }

        public void NoviceClericOnManaUse(Player player)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.NoviceClericCrosses--;
            bool synergy = SynergyActive(player);
            int burstNum = (synergy && DLCPlayer.NoviceClericCrosses > 0) ? 5 : 3;

            if ((synergy && DLCPlayer.NoviceClericCrosses % 2 == 0) || !synergy)
            {
                Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.HealPulse>(), 0, 0, player.whoAmI, 5);
            }
            else
            {
                // ebon crux
                Vector2 vector = player.Center.DirectionTo(Main.MouseWorld) * 8f;
                for (int i = 0; i < burstNum; i++)
                {
                    vector = Utils.RotatedBy(vector, MathF.Tau / burstNum);
                    Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, vector, ModContent.ProjectileType<Projectiles.DLCShadowWispPro>(), 15, 2f, player.whoAmI, 0f, 0f, 0f);
                }
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        internal int NoviceClericCrosses = 0;
        internal int NoviceClericTimer = 0;
        public Vector2 crossOrbitalRotation = Vector2.UnitY;
    }
}