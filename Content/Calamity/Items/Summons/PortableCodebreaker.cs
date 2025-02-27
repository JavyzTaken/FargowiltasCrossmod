using CalamityMod.Items.DraedonMisc;
using CalamityMod.NPCs.ExoMechs;
using Fargowiltas.Items.Summons;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items.Summons
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PortableCodebreaker : BaseSummon
    {
        //public override string Texture => "CalamityMod/Items/Materials/ExoPrism";
        public override int NPCType => ModContent.NPCType<Draedon>();
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Everything burns if there's more than one Draedon/Exo Mech pair.
            if (NPC.AnyNPCs(NPCType))
                return false;

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            Recipe.Create(Type).AddIngredient<AuricQuantumCoolingCell>().AddTile(TileID.WorkBenches).Register();
        }
    }
}
