using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using System;
using FargowiltasCrossmod.Content.Thorium.PlayerLayers;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    [AutoloadEquip(EquipType.Wings)]
    public class NagaSkinEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Teal;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new(100);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.NagaSkinEnch = true;

            NagaSkinEffect(player);
        }

        public const int rangeHalfWidth = 12;
        public const int rangeHalfHeight = 12;
        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse || player.wingFrame == 2)
            {
                for (int i = -rangeHalfWidth; i < rangeHalfWidth; i++)
                {
                    for (int j = -rangeHalfHeight; j < rangeHalfHeight; j++)
                    {
                        Tile tile = Main.tile[i + (int)player.Center.X / 16, j + (int)player.Center.Y / 16];
                        if (tile.HasTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                        {
                            player.wingTime = 2f;
                            break;
                        }
                        else
                        {
                            player.slowFall = false;
                        }
                    }
                }
            }
            return base.WingUpdate(player, inUse);
        }

        static Tile? GetTileSafely(int x, int y)
        {
            if (x < 0 || x >= Main.tile.Width || y < 0 || y >= Main.tile.Height) return null;
            return Main.tile[x, y];
        }

        public static void NagaSkinEffect(Player player)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();

            for (int l = 0; l < DLCPlayer.nagaSkinLegs.Length; l++)
            {
                Vector2 lead = DLCPlayer.nagaSkinLegs[l];
                Vector2 target = DLCPlayer.nagaSkinLegTargets[l];

                if (lead.Distance(player.Center) > 320f)
                {
                    DLCPlayer.nagaSkinLegs[l] = player.Center;
                    Main.NewText("leg teleported");
                }

                Tile? t = GetTileSafely((int)target.X / 16, (int)target.Y / 16);
                if (MathF.Abs(target.X - player.Center.X) > rangeHalfWidth * 16f || MathF.Abs(target.Y - player.Center.Y) > rangeHalfHeight * 16f || !t.HasValue || !t.Value.HasTile)
                {
                    for (int i = 0; i < 2 * rangeHalfWidth; i++)
                    {
                        for (int j = 0; j < 2 * rangeHalfHeight; j++)
                        {
                            int xStep = l >= 2 ? 1 : -1;
                            int yStep = l % 2 == 0 ? 1 : -1;
                            int x = ((int)player.Center.X / 16) + (rangeHalfWidth * xStep) - (i * xStep);
                            int y = ((int)player.Center.Y / 16) + (rangeHalfHeight * yStep) - (j * yStep);

                            bool flag = false;
                            for (int c = 0; c < l; c++)
                            {
                                if ((int)DLCPlayer.nagaSkinLegTargets[c].X == x * 16 && (int)DLCPlayer.nagaSkinLegTargets[c].Y == y * 16)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag) continue;

                            Tile? tile = GetTileSafely(x, y);

                            if (tile.HasValue && tile.Value.HasTile && (Main.tileSolid[tile.Value.TileType] || Main.tileSolidTop[tile.Value.TileType]))
                            {
                                DLCPlayer.nagaSkinLegTargets[l] = new Vector2(x, y) * 16f;
                            }
                        }
                    }

                    Main.NewText("Leg changed");
                }

                float dist = lead.Distance(DLCPlayer.nagaSkinLegTargets[l]);
                if (dist > 8f)
                {
                    DLCPlayer.nagaSkinLegs[l] += (DLCPlayer.nagaSkinLegTargets[l] - DLCPlayer.nagaSkinLegs[l]) / 10f;
                }
            }
        }
    }
}