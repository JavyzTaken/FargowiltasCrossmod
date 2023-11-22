using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.World;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MedallionoftheDesert : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/DesertMedallion";
        public override int NPCType => ModContent.NPCType<DesertScourgeHead>();
        public override string NPCName => "Desert Scourge";
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<DesertMedallion>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<DesertMedallion>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
        public override bool? UseItem(Player player)
        {
            //ModContent.GetInstance<DesertMedallion>().UseItem(player);
            //FargoSoulsUtil.SpawnBossNetcoded(player, NPCType);

            //just. copied from desert medallion code
            SoundEngine.PlaySound(in SoundID.Roar, player.Center);
            /*
            if (Main.netMode != 1)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertScourgeHead>());
            }
            else
            {
                NetMessage.SendData(61, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertScourgeHead>());
            }
            */
            if (CalamityWorld.revenge)
            {
                if (Main.netMode != 1)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                }
                else
                {
                    NetMessage.SendData(61, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                }

                if (Main.netMode != 1)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                }
                else
                {
                    NetMessage.SendData(61, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                }
            }
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }
}
