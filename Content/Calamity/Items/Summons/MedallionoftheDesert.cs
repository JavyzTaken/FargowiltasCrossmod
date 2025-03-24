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
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<DesertMedallion>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<DesertMedallion>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertScourgeHead>());
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertScourgeHead>());

            /*
            if (CalamityWorld.revenge)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
            }
            */
            return false;
        }
    }
}
