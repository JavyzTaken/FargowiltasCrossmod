using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DepthDiverEnchant : BaseSynergyEnchant<IcyEffect>
    {
        public override Color nameColor => new(11, 86, 255);
        internal override int SynergyEnch => ModContent.ItemType<IcyEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<DepthDiverEffect>(Item);
		}
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DepthDiverEffect : SynergyEffect<IcyEffect>
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.JotunheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<DepthDiverEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            var DLCPlayer = player.ThoriumDLC();
            if (DLCPlayer.DepthBubble > 0)
            {
                player.GetDamage(player.ProcessDamageTypeFromHeldItem()) += 0.10f;
                player.statDefense += 12;
                player.endurance += 0.15f;
                player.dripping = true;
            }

            if (!player.wet && DLCPlayer.lastWet)
            {
                DLCPlayer.DepthBubble = 1;
            }

            DLCPlayer.lastWet = player.wet;
        }

        public override void OnHitByEither(Player player, NPC npc, Projectile proj)
        {
            var DLCPlayer = player.ThoriumDLC();
            if (DLCPlayer.DepthBubble > 0)
            {
                DLCPlayer.DepthBubble = 0;

                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDustPerfect(Main.rand.NextVector2CircularEdge(player.width, player.height), DustID.Water);
                }

                if (SynergyActive(player))
                {
                    Vector2 vector = Vector2.UnitY * 4;
                    for (int i = 0; i < 8; i++)
                    {
                        vector = vector.RotatedBy(i * MathHelper.TwoPi / 8);
                        Projectile icicle = Projectile.NewProjectileDirect(GetSource_EffectItem(player), player.Center + vector, vector, ProjectileID.IceSpike, 25, 1f, player.whoAmI);
                        icicle.friendly = true;
                        icicle.hostile = false;
                    }
                }
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        internal bool lastWet;
        internal int DepthBubble;
    }
}