using CalamityMod.Systems;
using CalamityMod.World;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Luminance.Core.Hooking;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.GraphicsReplacements
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CodebreakerProjectileReplacer : ModSystem
    {
        private static readonly MethodInfo handleDraedonSummoningMethod = typeof(WorldMiscUpdateSystem).GetMethod("HandleDraedonSummoning", LumUtils.UniversalBindingFlags);

        public delegate void Orig_ExoMechSelectionUIDraw();

        public override void OnModLoad()
        {
            if (handleDraedonSummoningMethod is null)
                return;

            HookHelper.ModifyMethodWithDetour(handleDraedonSummoningMethod, HandleSummonBehaviors);
        }

        /// <summary>
        /// Handles custom summon behaviors for Draedon.
        /// </summary>
        public static void HandleSummonBehaviors(Orig_ExoMechSelectionUIDraw orig)
        {
            if (!CalDLCWorldSavingSystem.E_EternityRev)
            {
                orig();
                return;
            }

            if (CalamityWorld.DraedonSummonCountdown == CalamityWorld.DraedonSummonCountdownMax - 45)
            {
                IEntitySource source = new EntitySource_WorldEvent();
                Projectile.NewProjectile(source, CalamityWorld.DraedonSummonPosition + new Vector2(6f, 56f), -Vector2.UnitY, ModContent.ProjectileType<CodebreakerDataStream>(), 0, 0f);
            }

            if (CalamityWorld.DraedonSummonCountdown == 0)
            {
                IEntitySource source = new EntitySource_WorldEvent();
                NPC.NewNPC(source, (int)CalamityWorld.DraedonSummonPosition.X, (int)CalamityWorld.DraedonSummonPosition.Y, ModContent.NPCType<CalamityMod.NPCs.ExoMechs.Draedon>());
            }
        }
    }
}
