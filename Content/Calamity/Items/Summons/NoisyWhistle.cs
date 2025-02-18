using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs.Ravager;
using Fargowiltas.Items.Summons;
using Fargowiltas.Projectiles;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class NoisyWhistle : BaseSummon
    {
        public override string Texture => "CalamityMod/Items/SummonItems/DeathWhistle";
        public override int NPCType => ModContent.NPCType<RavagerBody>();
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<DeathWhistle>().AddTile(TileID.WorkBenches).Register();
            Recipe.Create(ModContent.ItemType<DeathWhistle>()).AddIngredient(Type).AddTile(TileID.WorkBenches).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ResetTimeWhenUsed)
            {
                Main.time = 0;

                if (Main.netMode == NetmodeID.Server) //sync time
                    NetMessage.SendData(MessageID.WorldData, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }

            Vector2 pos = new Vector2((int)player.position.X + Main.rand.Next(-800, 800), (int)player.position.Y + Main.rand.Next(-800, -250));

            if (NPCType == NPCID.Golem)
            {
                pos = player.Center;
                for (int i = 0; i < 30; i++)
                {
                    pos.Y -= 16;

                    if (pos.Y <= 0 || WorldGen.SolidTile((int)pos.X / 16, (int)pos.Y / 16))
                    {
                        pos.Y += 16;
                        break;
                    }
                }
            }

            Projectile.NewProjectile(player.GetSource_ItemUse(source.Item), pos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, NPCType);

            LocalizedText text = Language.GetText("Announcement.HasAwoken");
            string npcName = NPCName ?? (ModContent.GetModNPC(NPCType) == null ? Lang.GetNPCNameValue(NPCType) : ModContent.GetModNPC(NPCType).DisplayName.Value);

            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(text.ToNetworkText(npcName), new Color(175, 75, 255));
            }
            else if (NPCType != NPCID.KingSlime)
            {
                Main.NewText(text.Format(npcName), new Color(175, 75, 255));
            }

            SoundEngine.PlaySound(SoundID.ScaryScream, player.position);

            return false;
        }
    }
}
