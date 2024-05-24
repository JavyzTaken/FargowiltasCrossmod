using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using CalamityMod.World;
using CalamityMod;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCDetours : ModSystem
    {
        public override void Load()
        {
            if (ModLoader.TryGetMod(ModCompatibility.Calamity.Name, out Mod calamity))
            {
                calamity.Call("AddDifficultyToUI", new EternityRevDifficulty());
                calamity.Call("AddDifficultyToUI", new EternityDeathDifficulty());
            }

            CalamityPreAIHook = new(CalamityPreAIMethod, CalamityPreAI_Detour);
            CalamityPreAIHook.Apply();

            CalamityProjectilePreAIHook = new(CalamityProjectilePreAIMethod, CalamityProjectilePreAI_Detour);
            CalamityProjectilePreAIHook.Apply();

            CalamityPreDrawHook = new(CalamityPreDrawMethod, CalamityPreDraw_Detour);
            CalamityPreDrawHook.Apply();

            CalamityPostDrawHook = new(CalamityPostDrawMethod, CalamityPostDraw_Detour);
            CalamityPostDrawHook.Apply();

        }

        public override void Unload()
        {
            CalamityPreAIHook.Undo();
            CalamityProjectilePreAIHook.Undo();

            CalamityPreDrawHook.Undo();
            CalamityPostDrawHook.Undo();
        }

        public delegate bool Orig_CalamityPreAI(CalamityGlobalNPC self, NPC npc);
        public delegate bool Orig_CalamityProjectilePreAI(CalamityGlobalProjectile self, Projectile projectile);

        public delegate bool Orig_CalamityPreDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        public delegate void Orig_CalamityPostDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);

        private static readonly MethodInfo CalamityPreAIMethod = typeof(CalamityGlobalNPC).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityProjectilePreAIMethod = typeof(CalamityGlobalProjectile).GetMethod("PreAI", LumUtils.UniversalBindingFlags);

        private static readonly MethodInfo CalamityPreDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PreDraw", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityPostDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PostDraw", LumUtils.UniversalBindingFlags);

        Hook CalamityPreAIHook;
        Hook CalamityProjectilePreAIHook;

        Hook CalamityPreDrawHook;
        Hook CalamityPostDrawHook;

        internal static bool CalamityPreAI_Detour(Orig_CalamityPreAI orig, CalamityGlobalNPC self, NPC npc)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool shouldDisable = CalDLCConfig.Instance.EternityPriorityOverRev && WorldSavingSystem.EternityMode;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
            }
            bool result = orig(self, npc);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
            }
            return result;
        }

        internal static bool CalamityProjectilePreAI_Detour(Orig_CalamityProjectilePreAI orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool shouldDisable = CalDLCConfig.Instance.EternityPriorityOverRev && WorldSavingSystem.EternityMode;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
            }
            bool result = orig(self, projectile);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
            }
            return result;
        }

        internal static bool CalamityPreDraw_Detour(Orig_CalamityPreDraw orig, CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisableNPC = CalamityLists.DestroyerIDs.Contains(npc.type);
            bool shouldDisable = CalDLCConfig.Instance.EternityPriorityOverRev && WorldSavingSystem.EternityMode && shouldDisableNPC;

            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                BossRushEvent.BossRushActive = false;
            }
            bool result = orig(self, npc, spriteBatch, screenPos, drawColor);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                BossRushEvent.BossRushActive = wasBossRush;
            }
            return result;
        }

        private static readonly List<int> DisablePostDrawNPCS = new(CalamityLists.DestroyerIDs)
            {
            NPCID.WallofFleshEye,
            NPCID.Creeper,
            };

        internal static void CalamityPostDraw_Detour(Orig_CalamityPostDraw orig, CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool shouldDisableNPC = DisablePostDrawNPCS.Contains(npc.type);
            bool shouldDisable = CalDLCConfig.Instance.EternityPriorityOverRev && WorldSavingSystem.EternityMode && shouldDisableNPC;
            if (shouldDisable)
            {
                return;
            }
            orig(self, npc, spriteBatch, screenPos, drawColor);
        }
    }
}
