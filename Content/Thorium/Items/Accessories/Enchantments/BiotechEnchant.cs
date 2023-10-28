using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class BiotechEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Green;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.BiotechEnch = true;
            DLCPlayer.BiotechEnchItem = Item;

            BiotechEffect(player, DLCPlayer);
        }

        public static void BiotechEffect(Player player, CrossplayerThorium DLCPlayer)
        {
            if (++DLCPlayer.BiotechSpawnTimer > 60)
            {
                DLCPlayer.BiotechSpawnTimer = 0;

                int maxMachines = player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().ForceEffect(ModContent.ItemType<BiotechEnchant>()) ? 10 : 5;
                int nanomachineType = ModContent.ProjectileType<Projectiles.BiotechNanomachine>();

                if (player.ownedProjectileCounts[nanomachineType] < maxMachines)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(DLCPlayer.BiotechEnchItem), player.Center, Vector2.Zero, nanomachineType, 0, 0, player.whoAmI);
                }
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        internal int BiotechSpawnTimer;
    }
}