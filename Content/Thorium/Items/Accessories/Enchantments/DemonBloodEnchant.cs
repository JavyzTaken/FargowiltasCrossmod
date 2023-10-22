using System;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DemonBloodEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Red;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.FleshEnch = true;
            DLCPlayer.DemonBloodEnch = true;
            DLCPlayer.FleshEnchItem = Item;
            DLCPlayer.DemonBloodEnchItem = Item;

            DemonBloodEffect(player);
        }

        public static void DemonBloodEffect(Player player)
        {
            int bloodType = ModContent.ProjectileType<Projectiles.DemonBloodSpill>();
            int bloodProjs = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type != bloodType || !proj.active || proj.Center.DistanceSQ(player.Center) > 25600f) continue;
                proj.timeLeft--;
                bloodProjs++;
            }

            if (bloodProjs == 0) return;
            player.GetDamage(DamageClass.Generic) += MathF.Min(bloodProjs, 5) * 0.1f;

            for (int i = 0; i < MathF.Min(bloodProjs, 5); i++)
            {
                if (Main.rand.NextBool(15))
                {
                    Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Blood);
                    dust.noGravity = false;
                }
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void SpawnDemonBlood(Vector2 pos)
        {
            Projectile.NewProjectile(Player.GetSource_Accessory(FleshEnchItem), pos, Vector2.Zero, ModContent.ProjectileType<Projectiles.DemonBloodSpill>(), 0, 0, Player.whoAmI);
        }
    }
}
