using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items
{
    public class CalamityAddonGlobalItem : GlobalItem
    {
        public override void OnConsumeItem(Item item, Player player)
        {
            int bats = item.healLife / 25;
            int minBats = bats - 2;
            int maxBats = bats + 2;
            int batDamage = 50;
            if (player.ForceEffect<UmbraphileEffect>()) batDamage *= 3;
            if (item.healLife > 0 && player.HasEffect<UmbraphileEffect>())
            {
                for (int i = 0; i < Main.rand.Next(minBats, maxBats+1); i++) {
                    Projectile.NewProjectile(player.GetSource_EffectItem<UmbraphileEffect>(), player.Center, new Microsoft.Xna.Framework.Vector2(0, 5).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<VampireBat>(), batDamage, 1, player.whoAmI);
                }
            }
        }
        public override void GetHealLife(Item item, Player player, bool quickHeal, ref int healValue)
        {
            if (player.HasEffect<UmbraphileEffect>())
            {
                healValue = 0;
            }
        }
    }
}
