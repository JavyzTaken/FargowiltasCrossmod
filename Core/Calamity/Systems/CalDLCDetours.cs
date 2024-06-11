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
using Terraria.DataStructures;
using CalamityMod.Items;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCDetours : ModSystem
    {
        public override void Load()
        {

            CalamityPreAIHook = new(CalamityPreAIMethod, CalamityPreAI_Detour);
            CalamityPreAIHook.Apply();

            CalamityProjectilePreAIHook = new(CalamityProjectilePreAIMethod, CalamityProjectilePreAI_Detour);
            CalamityProjectilePreAIHook.Apply();

            CalamityPreDrawHook = new(CalamityPreDrawMethod, CalamityPreDraw_Detour);
            CalamityPreDrawHook.Apply();

            CalamityPostDrawHook = new(CalamityPostDrawMethod, CalamityPostDraw_Detour);
            CalamityPostDrawHook.Apply();

            CalamityShootHook = new(CalamityShootMethod, CalamityShoot_Detour);
            CalamityShootHook.Apply();

            FMSVerticalSpeedHook = new(FMSVerticalSpeedMethod, FMSVerticalSpeed_Detour);
            FMSVerticalSpeedHook.Apply();

            FMSHorizontalSpeedHook = new(FMSHorizontalSpeedMethod, FMSHorizontalSpeed_Detour);
            FMSHorizontalSpeedHook.Apply();
        }

        public override void Unload()
        {
            CalamityPreAIHook.Undo();
            CalamityProjectilePreAIHook.Undo();

            CalamityPreDrawHook.Undo();
            CalamityPostDrawHook.Undo();

            CalamityShootHook.Undo();

            FMSVerticalSpeedHook.Undo();
            FMSHorizontalSpeedHook.Undo();
        }

        Hook CalamityPreAIHook;
        Hook CalamityProjectilePreAIHook;
        public delegate bool Orig_CalamityPreAI(CalamityGlobalNPC self, NPC npc);
        public delegate bool Orig_CalamityProjectilePreAI(CalamityGlobalProjectile self, Projectile projectile);
        private static readonly MethodInfo CalamityPreAIMethod = typeof(CalamityGlobalNPC).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityProjectilePreAIMethod = typeof(CalamityGlobalProjectile).GetMethod("PreAI", LumUtils.UniversalBindingFlags);

        Hook CalamityPreDrawHook;
        Hook CalamityPostDrawHook;
        public delegate bool Orig_CalamityPreDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        public delegate void Orig_CalamityPostDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        private static readonly MethodInfo CalamityPreDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PreDraw", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityPostDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PostDraw", LumUtils.UniversalBindingFlags);

        Hook CalamityShootHook;
        public delegate bool Orig_CalamityShoot(CalamityGlobalItem self, Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack);
        private static readonly MethodInfo CalamityShootMethod = typeof(CalamityGlobalItem).GetMethod("Shoot", LumUtils.UniversalBindingFlags);

        Hook FMSVerticalSpeedHook;
        Hook FMSHorizontalSpeedHook;
        public delegate void Orig_FMSVerticalSpeed(FlightMasteryWings self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);
        public delegate void Orig_FMSHorizontalSpeed(FlightMasteryWings self, Player player, ref float speed, ref float acceleration);
        private static readonly MethodInfo FMSVerticalSpeedMethod = typeof(FlightMasteryWings).GetMethod("VerticalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo FMSHorizontalSpeedMethod = typeof(FlightMasteryWings).GetMethod("HorizontalWingSpeeds", LumUtils.UniversalBindingFlags);

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
            bool shouldDisableNPC = CalamityLists.DestroyerIDs.Contains(npc.type) || npc.type == NPCID.SkeletronPrime;
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
            NPCID.SkeletronPrime
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

        internal static bool CalamityShoot_Detour(Orig_CalamityShoot orig, CalamityGlobalItem self, Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            // Luxor's Gift banlist
            if (item.type == ModContent.ItemType<KamikazeSquirrelStaff>())
                player.Calamity().luxorsGift = false;

            return orig(self, item, player, source, position, velocity, type, damage, knockBack);
        }
        public static bool NonFargoBossAlive() => Main.npc.Any(n => n.Alive() && n.boss && n.ModNPC != null && n.ModNPC.Mod != ModCompatibility.SoulsMod.Mod);
        internal static void FMSVerticalSpeed_Detour(Orig_FMSVerticalSpeed orig, FlightMasteryWings self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            if (NonFargoBossAlive())
            {
                player.wingsLogic = ArmorIDs.Wing.LongTrailRainbowWings;
                if (!DownedBossSystem.downedYharon) // pre yharon, use Silva Wings stats
                {
                    ascentWhenFalling = 0.95f;
                    ascentWhenRising = 0.16f;
                    maxCanAscendMultiplier = 1.1f;
                    maxAscentMultiplier = 3.2f;
                    constantAscend = 0.145f;
                }
                else // post yharon, use Drew's Wings stats
                {
                    ascentWhenFalling = 1f;
                    ascentWhenRising = 0.17f;
                    maxCanAscendMultiplier = 1.2f;
                    maxAscentMultiplier = 3.25f;
                    constantAscend = 0.15f;
                }
            }
            else
            {
                orig(self, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
            }
        }
        internal static void FMSHorizontalSpeed_Detour(Orig_FMSHorizontalSpeed orig, FlightMasteryWings self, Player player, ref float speed, ref float acceleration)
        {
            if (NonFargoBossAlive())
            {
                if (!DownedBossSystem.downedYharon) // pre yharon, use Silva Wings stats
                {
                    speed = 10.5f;
                    acceleration = 2.8f;
                }
                else // post yharon, use Drew's Wings stats
                {
                    speed = 11.5f;
                    acceleration = 2.9f;
                }
                   
                //ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(361, 11.5f, 2.9f);
            }
            else
            {
                orig(self, player, ref speed, ref acceleration);
            }
        }
    }
}
