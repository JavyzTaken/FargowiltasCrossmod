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
using Luminance.Core.Hooking;
using Fargowiltas.NPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Content.Projectiles;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Ranged;
using Fargowiltas.Items;
using CalamityMod.Items.Potions;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using CalamityMod.CalPlayer;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Content.Calamity.Toggles;
using CalamityMod.Systems;
using CalamityMod.Enums;
using Fargowiltas.Items.Vanity;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.UI.Elements;
using Terraria.Localization;
using CalamityMod.Skies;
using Terraria.Graphics.Effects;
using FargowiltasSouls.Content.UI;
using CalamityMod.NPCs.Perforator;
using FargowiltasCrossmod.Content.Calamity.Bosses.Perforators;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Common.Utilities;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Terraria.GameContent.ItemDropRules;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Pets;
using FargowiltasSouls.Content.Items.Misc;
using Fargowiltas.Items.Explosives;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using Fargowiltas.Items.Tiles;
using CalamityMod.Walls;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Tiles.FurnitureAcidwood;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.FurnitureMonolith;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.SunkenSea;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCDetours : ModSystem, ICustomDetourProvider
    {
        // AI override
        // GlobalNPC
        private static readonly MethodInfo CalamityPreAIMethod = typeof(CalamityGlobalNPC).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityOtherStatChangesMethod = typeof(CalamityGlobalNPC).GetMethod("OtherStatChanges", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityPreDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PreDraw", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityPostDrawMethod = typeof(CalamityGlobalNPC).GetMethod("PostDraw", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityBossHeadSlotMethod = typeof(CalamityGlobalNPC).GetMethod("BossHeadSlot", LumUtils.UniversalBindingFlags);
        // NPCStats
        private static readonly MethodInfo CalamityGetNPCDamageMethod = typeof(NPCStats).GetMethod("GetNPCDamage", LumUtils.UniversalBindingFlags);
        // GlobalProjectile
        private static readonly MethodInfo CalamityProjectilePreAIMethod = typeof(CalamityGlobalProjectile).GetMethod("PreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CalamityProjectileCanDamageMethod = typeof(CalamityGlobalProjectile).GetMethod("CanDamage", LumUtils.UniversalBindingFlags);

        // Misc compatibility, fixes and balance
        private static readonly MethodInfo FMSVerticalSpeedMethod = typeof(FlightMasteryWings).GetMethod("VerticalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo FMSHorizontalSpeedMethod = typeof(FlightMasteryWings).GetMethod("HorizontalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo LifeForceVerticalSpeedMethod = typeof(LifeForce).GetMethod("VerticalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo LifeForceHorizontalSpeedMethod = typeof(LifeForce).GetMethod("HorizontalWingSpeeds", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo IsFargoSoulsItemMethod = typeof(Squirrel).GetMethod("IsFargoSoulsItem", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo BrimstoneMonsterCanHitPlayerMethod = typeof(BrimstoneMonster).GetMethod("CanHitPlayer", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo FargoSoulsOnSpawnProjMethod = typeof(FargoSoulsGlobalProjectile).GetMethod("OnSpawn", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TryUnlimBuffMethod = typeof(Fargowiltas.Items.FargoGlobalItem).GetMethod("TryUnlimBuff", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo NatureChampAIMethod = typeof(NatureChampion).GetMethod("AI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TerraChampAIMethod = typeof(TerraChampion).GetMethod("AI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CheckTempleWallsMethod = typeof(Golem).GetMethod("CheckTempleWalls", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo DukeFishronPreAIMethod = typeof(DukeFishron).GetMethod("SafePreAI", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo MoonLordCanBeHitByProjectile_Method = typeof(MoonLord).GetMethod("CanBeHitByProjectile", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TungstenIncreaseWeaponSizeMethod = typeof(TungstenEffect).GetMethod("TungstenIncreaseWeaponSize", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TungstenNerfedProjMetod = typeof(TungstenEffect).GetMethod("TungstenNerfedProj", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo TungstenNeverAffectsProjMethod = typeof(TungstenEffect).GetMethod("TungstenNeverAffectsProj", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo ModifyHurtInfo_CalamityMethod = typeof(CalamityPlayer).GetMethod("ModifyHurtInfo_Calamity", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo MinimalEffects_Method = typeof(ToggleBackend).GetMethod("MinimalEffects", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo BRDialogueTick_Method = typeof(BossRushDialogueSystem).GetMethod("Tick", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo FargoPlayerPreKill_Method = typeof(FargoSoulsPlayer).GetMethod("PreKill", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo EModePlayerPreUpdate_Method = typeof(EModePlayer).GetMethod("PreUpdate", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo CanToggleEternity_Method = typeof(Masochist).GetMethod("CanToggleEternity", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo SoulTogglerOnActivate_Method = typeof(SoulTogglerButton).GetMethod("OnActivate", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo GetAdrenalineDamage_Method = typeof(CalamityUtils).GetMethod("GetAdrenalineDamage", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo DetermineDrawEligibility_Method = typeof(BossRushSky).GetMethod("DetermineDrawEligibility", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo MediumPerforatorHeadOnKill_Method = typeof(PerforatorHeadMedium).GetMethod("OnKill", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo MediumPerforatorBodyOnKill_Method = typeof(PerforatorBodyMedium).GetMethod("OnKill", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo MediumPerforatorTailOnKill_Method = typeof(PerforatorTailMedium).GetMethod("OnKill", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo EmodeBalance_Method = typeof(EmodeItemBalance).GetMethod("EmodeBalance", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo EmodeEditSpawnPool_Method = typeof(EModeGlobalNPC).GetMethod("EditSpawnPool", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo DropSummon_Int_Method = typeof(EModeUtils).GetMethod("DropSummon", LumUtils.UniversalBindingFlags, [typeof(NPC), typeof(int), typeof(bool), typeof(bool).MakeByRefType(), typeof(bool)]);
        private static readonly MethodInfo DropSummon_String_Method = typeof(EModeUtils).GetMethod("DropSummon", LumUtils.UniversalBindingFlags, [typeof(NPC), typeof(string), typeof(bool), typeof(bool).MakeByRefType(), typeof(bool)]);
        private static readonly MethodInfo StarterBag_ModifyItemLoot_Method = typeof(StarterBag).GetMethod("ModifyItemLoot", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo FargosSouls_DropDevianttsGift_Method = typeof(FargowiltasSouls.FargowiltasSouls).GetMethod("DropDevianttsGift", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo Instahouse_GetTiles_Method = typeof(Fargowiltas.Projectiles.Explosives.AutoHouseProj).GetMethod("GetTiles", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo Instahouse_GetFurniture_Method = typeof(Fargowiltas.Projectiles.Explosives.AutoHouseProj).GetMethod("GetFurniture", LumUtils.UniversalBindingFlags);
        // AI override
        // GlobalNPC
        public delegate bool Orig_CalamityPreAI(CalamityGlobalNPC self, NPC npc);
        public delegate void Orig_CalamityOtherStatChanges(CalamityGlobalNPC self, NPC npc);
        public delegate bool Orig_CalamityPreDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        public delegate void Orig_CalamityPostDraw(CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        public delegate void Orig_CalamityBossHeadSlot(CalamityGlobalNPC self, NPC npc, ref int index);
        // NPCStats
        public delegate void Orig_CalamityGetNPCDamage(NPC npc);
        // GlobalProjectile
        public delegate bool Orig_CalamityProjectilePreAI(CalamityGlobalProjectile self, Projectile projectile);
        public delegate bool? Orig_CalamityProjectileCanDamage(CalamityGlobalProjectile self, Projectile projectile);

        // Misc compatibility, fixes and balance
        public delegate void Orig_FMSVerticalSpeed(FlightMasteryWings self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);
        public delegate void Orig_FMSHorizontalSpeed(FlightMasteryWings self, Player player, ref float speed, ref float acceleration);
        public delegate void Orig_LifeForceVerticalSpeed(LifeForce self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend);
        public delegate void Orig_LifeForceHorizontalSpeed(LifeForce self, Player player, ref float speed, ref float acceleration);
        public delegate bool Orig_IsFargoSoulsItem(Item item);
        public delegate bool Orig_BrimstoneMonsterCanHitPlayer(BrimstoneMonster self, Player player);
        public delegate void Orig_FargoSoulsOnSpawnProj(FargoSoulsGlobalProjectile self, Projectile projectile, IEntitySource source);
        public delegate void Orig_TryUnlimBuff( Item item, Player player);
        public delegate void Orig_NatureChampAI(NatureChampion self);
        public delegate void Orig_TerraChampAI(TerraChampion self);
        public delegate bool Orig_CheckTempleWalls(Vector2 pos);
        public delegate bool Orig_DukeFishronPreAI(DukeFishron self, NPC npc);
        public delegate bool? Orig_MoonLordCanBeHitByProjectile(MoonLord self, NPC npc, Projectile projectile);
        public delegate float Orig_TungstenIncreaseWeaponSize(FargoSoulsPlayer modPlayer);
        public delegate bool Orig_TungstenNerfedProj(Projectile projectile);
        public delegate bool Orig_TungstenNeverAffectsProj(Projectile projectile);
        public delegate void Orig_ModifyHurtInfo_Calamity(CalamityPlayer self, ref Player.HurtInfo info);
        public delegate void Orig_MinimalEffects(ToggleBackend self);
        public delegate void Orig_BRDialogueTick();
        public delegate bool Orig_FargoPlayerPreKill(FargoSoulsPlayer self, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
        public delegate void Orig_EModePlayerPreUpdate(EModePlayer self);
        public delegate bool Orig_CanToggleEternity();
        public delegate void Orig_SoulTogglerOnActivate(SoulTogglerButton self);
        public delegate float Orig_GetAdrenalineDamage(CalamityPlayer mp);
        public delegate bool Orig_DetermineDrawEligibility();
        public delegate void Orig_MediumPerforatorHeadOnKill(PerforatorHeadMedium self);
        public delegate void Orig_MediumPerforatorBodyOnKill(PerforatorBodyMedium self);
        public delegate void Orig_MediumPerforatorTailOnKill(PerforatorTailMedium self);
        public delegate EmodeItemBalance.EModeChange Orig_EmodeBalance(ref Item item, ref float balanceNumber, ref string[] balanceTextKeys, ref string extra);
        public delegate void Orig_EmodeEditSpawnPool(EModeGlobalNPC self, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo);
        public delegate void Orig_DropSummon_Int_Method(NPC npc, int itemType, bool downed, ref bool droppedSummon, bool prerequisite = true);
        public delegate void Orig_DropSummon_String_Method(NPC npc, string itemName, bool downed, ref bool droppedSummon, bool prerequisite = true);
        public delegate void Orig_StarterBag_ModifyItemLoot(StarterBag self, ItemLoot itemLoot);
        public delegate void Orig_FargosSouls_DropDevianttsGift(Player player);
        public delegate void Orig_Instahouse_GetTiles(Player player, out int wallType, out int tileType, out int platformStyle, out bool moddedPlatform);
        public delegate void Orig_Instahouse_GetFurniture(Player player, out int doorStyle, out int chairStyle, out int tableStyle, out int torchStyle);

        public override void Load()
        {
            On_NPC.AddBuff += NPCAddBuff_Detour;
        }
        void ICustomDetourProvider.ModifyMethods()
        {
            // AI override
            // GlobalNPC
            HookHelper.ModifyMethodWithDetour(CalamityPreAIMethod, CalamityPreAI_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityOtherStatChangesMethod, CalamityOtherStatChanges_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityPreDrawMethod, CalamityPreDraw_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityPostDrawMethod, CalamityPostDraw_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityBossHeadSlotMethod, CalamityBossHeadSlot_Detour);
            // NPCStats
            HookHelper.ModifyMethodWithDetour(CalamityGetNPCDamageMethod, CalamityGetNPCDamage_Detour);
            // GlobalProjectile
            HookHelper.ModifyMethodWithDetour(CalamityProjectilePreAIMethod, CalamityProjectilePreAI_Detour);
            HookHelper.ModifyMethodWithDetour(CalamityProjectileCanDamageMethod, CalamityProjectileCanDamage_Detour);

            // Misc compatibility, fixes and balance
            HookHelper.ModifyMethodWithDetour(FMSVerticalSpeedMethod, FMSVerticalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(FMSHorizontalSpeedMethod, FMSHorizontalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(LifeForceVerticalSpeedMethod, LifeForceVerticalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(LifeForceHorizontalSpeedMethod, LifeForceHorizontalSpeed_Detour);
            HookHelper.ModifyMethodWithDetour(IsFargoSoulsItemMethod, IsFargoSoulsItem_Detour);
            HookHelper.ModifyMethodWithDetour(BrimstoneMonsterCanHitPlayerMethod, BrimstoneMonsterCanHitPlayer_Detour);
            HookHelper.ModifyMethodWithDetour(FargoSoulsOnSpawnProjMethod, FargoSoulsOnSpawnProj_Detour);
            HookHelper.ModifyMethodWithDetour(TryUnlimBuffMethod, TryUnlimBuff_Detour);
            HookHelper.ModifyMethodWithDetour(NatureChampAIMethod, NatureChampAI_Detour);
            HookHelper.ModifyMethodWithDetour(TerraChampAIMethod, TerraChampAI_Detour);
            HookHelper.ModifyMethodWithDetour(CheckTempleWallsMethod, CheckTempleWalls_Detour);
            HookHelper.ModifyMethodWithDetour(DukeFishronPreAIMethod, DukeFishronPreAI_Detour);
            HookHelper.ModifyMethodWithDetour(MoonLordCanBeHitByProjectile_Method, MoonLordCanBeHitByProjectile_Detour);
            HookHelper.ModifyMethodWithDetour(TungstenIncreaseWeaponSizeMethod, TungstenIncreaseWeaponSize_Detour);
            HookHelper.ModifyMethodWithDetour(TungstenNerfedProjMetod, TungstenNerfedProj_Detour);
            HookHelper.ModifyMethodWithDetour(TungstenNeverAffectsProjMethod, TungstenNeverAffectsProj_Detour);
            HookHelper.ModifyMethodWithDetour(ModifyHurtInfo_CalamityMethod, ModifyHurtInfo_Calamity_Detour);
            HookHelper.ModifyMethodWithDetour(MinimalEffects_Method, MinimalEffects_Detour);
            HookHelper.ModifyMethodWithDetour(BRDialogueTick_Method, DialogueReplacement);
            //HookHelper.ModifyMethodWithDetour(BRSceneWeight_Method, );
            HookHelper.ModifyMethodWithDetour(FargoPlayerPreKill_Method, FargoPlayerPreKill_Detour);
            HookHelper.ModifyMethodWithDetour(EModePlayerPreUpdate_Method, EModePlayerPreUpdate_Detour);
            HookHelper.ModifyMethodWithDetour(CanToggleEternity_Method, CanToggleEternity_Detour);
            HookHelper.ModifyMethodWithDetour(SoulTogglerOnActivate_Method, SoulTogglerOnActivate_Detour);
            HookHelper.ModifyMethodWithDetour(GetAdrenalineDamage_Method, GetAdrenalineDamage_Detour);
            HookHelper.ModifyMethodWithDetour(DetermineDrawEligibility_Method, DetermineDrawEligibility_Detour);
            HookHelper.ModifyMethodWithDetour(MediumPerforatorHeadOnKill_Method, MediumPerforatorHeadOnKill_Detour);
            HookHelper.ModifyMethodWithDetour(MediumPerforatorBodyOnKill_Method, MediumPerforatorBodyOnKill_Detour);
            HookHelper.ModifyMethodWithDetour(MediumPerforatorTailOnKill_Method, MediumPerforatorTailOnKill_Detour);
            HookHelper.ModifyMethodWithDetour(EmodeBalance_Method, EmodeBalance_Detour);
            HookHelper.ModifyMethodWithDetour(EmodeEditSpawnPool_Method, EmodeEditSpawnPool_Detour);
            HookHelper.ModifyMethodWithDetour(DropSummon_Int_Method, DropSummon_Int_Detour);
            HookHelper.ModifyMethodWithDetour(DropSummon_String_Method, DropSummon_String_Detour);
            HookHelper.ModifyMethodWithDetour(StarterBag_ModifyItemLoot_Method, StarterBag_ModifyItemLoot_Detour);
            HookHelper.ModifyMethodWithDetour(FargosSouls_DropDevianttsGift_Method, FargosSouls_DropDevianttsGift_Detour);
            HookHelper.ModifyMethodWithDetour(Instahouse_GetTiles_Method, Instahouse_GetTiles_Detour);
            HookHelper.ModifyMethodWithDetour(Instahouse_GetFurniture_Method, Instahouse_GetFurniture_Detour);
        }
        #region GlobalNPC
        internal static bool CalamityPreAI_Detour(Orig_CalamityPreAI orig, CalamityGlobalNPC self, NPC npc)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;

            int defDamage = npc.defDamage; // do not fuck with defDamage please

            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
                BossRushEvent.BossRushActive = false;
            }
            bool result = orig(self, npc);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                BossRushEvent.BossRushActive = wasBossRush;
                npc.defDamage = defDamage; // do not fuck with defDamage please
            }
            return result;
        }

        internal static void CalamityOtherStatChanges_Detour(Orig_CalamityOtherStatChanges orig, CalamityGlobalNPC self, NPC npc)
        {
            orig(self, npc);
            if (!CalDLCWorldSavingSystem.E_EternityRev)
                return;
            switch (npc.type)
            {
                case NPCID.DetonatingBubble:
                    if (NPC.AnyNPCs(NPCID.DukeFishron))
                        npc.dontTakeDamage = false;
                    break;
                default:
                    break;
            }
        }

        internal static bool CalamityPreDraw_Detour(Orig_CalamityPreDraw orig, CalamityGlobalNPC self, NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisableNPC = CalamityLists.DestroyerIDs.Contains(npc.type) || npc.type == NPCID.SkeletronPrime;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev && shouldDisableNPC;

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
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev && shouldDisableNPC;
            if (shouldDisable)
            {
                return;
            }
            orig(self, npc, spriteBatch, screenPos, drawColor);
        }

        internal static void CalamityBossHeadSlot_Detour(Orig_CalamityBossHeadSlot orig, CalamityGlobalNPC self, NPC npc, ref int index)
        {
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            if (shouldDisable)
                return;
            orig(self, npc, ref index);
        }
        #endregion

        #region NPCStats
        internal static void CalamityGetNPCDamage_Detour(Orig_CalamityGetNPCDamage orig, NPC npc)
        {
            // Prevent vanilla bosses and their segments from having their damage overriden by Calamity
            bool countsAsBoss = npc.boss || NPCID.Sets.ShouldBeCountedAsBoss[npc.type];
            if (npc.type < NPCID.Count && (countsAsBoss || CalamityLists.bossHPScaleList.Contains(npc.type)))
                return;
            orig(npc);
        }
        #endregion

        #region GlobalProjectile
        internal static bool CalamityProjectilePreAI_Detour(Orig_CalamityProjectilePreAI orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            int damage = projectile.damage;
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
                projectile.damage = damage;
            }
            return result;
        }

        public static bool? CalamityProjectileCanDamage_Detour(Orig_CalamityProjectileCanDamage orig, CalamityGlobalProjectile self, Projectile projectile)
        {
            bool wasRevenge = CalamityWorld.revenge;
            bool wasDeath = CalamityWorld.death;
            bool wasBossRush = BossRushEvent.BossRushActive;
            bool shouldDisable = CalDLCWorldSavingSystem.E_EternityRev;
            if (shouldDisable)
            {
                CalamityWorld.revenge = false;
                CalamityWorld.death = false;
                BossRushEvent.BossRushActive = false;
            }
            bool? result = orig(self, projectile);
            if (shouldDisable)
            {
                CalamityWorld.revenge = wasRevenge;
                CalamityWorld.death = wasDeath;
                BossRushEvent.BossRushActive = wasBossRush;
            }
            return result;
        }
        #endregion

        #region Misc
        public static bool NonFargoBossAlive() => Main.npc.Any(n => n.Alive() && n.boss && n.ModNPC != null && n.ModNPC.Mod != ModCompatibility.SoulsMod.Mod);
        internal static void FMSVerticalSpeed_Detour(Orig_FMSVerticalSpeed orig, FlightMasteryWings self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            orig(self, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
            if (NonFargoBossAlive() && self is not EternitySoul)
            {
                player.wingsLogic = ArmorIDs.Wing.LongTrailRainbowWings;
                if (!DownedBossSystem.downedYharon) // pre yharon, use Silva Wings stats
                {
                    if (ascentWhenFalling > 0.95f)
                        ascentWhenFalling = 0.95f;
                    if (ascentWhenRising > 0.16f)
                        ascentWhenRising = 0.16f;
                    if (maxCanAscendMultiplier > 1.1f)
                        maxCanAscendMultiplier = 1.1f;
                    if (maxAscentMultiplier > 3.2f)
                        maxAscentMultiplier = 3.2f;
                    if (constantAscend > 0.145f)
                        constantAscend = 0.145f;
                }
                else // post yharon, use Drew's Wings stats
                {
                    if (ascentWhenFalling > 1f)
                        ascentWhenFalling = 1f;
                    if (ascentWhenRising > 0.17f)
                        ascentWhenRising = 0.17f;
                    if (maxCanAscendMultiplier > 1.2f)
                        maxCanAscendMultiplier = 1.2f;
                    if (maxAscentMultiplier > 3.25f)
                        maxAscentMultiplier = 3.25f;
                    if (constantAscend > 0.15f)
                        constantAscend = 0.15f;
                }
            }
        }
        internal static void FMSHorizontalSpeed_Detour(Orig_FMSHorizontalSpeed orig, FlightMasteryWings self, Player player, ref float speed, ref float acceleration)
        {
            orig(self, player, ref speed, ref acceleration);
            if (NonFargoBossAlive() && self is not EternitySoul)
            {
                if (!DownedBossSystem.downedYharon) // pre yharon, use Silva Wings stats
                {
                    if (speed > 10.5f)
                        speed = 10.5f;
                    if (acceleration > 2.8f)
                        acceleration = 2.8f;
                }
                else // post yharon, use Drew's Wings stats
                {
                    if (speed > 11.5f)
                        speed = 11.5f;
                    if (acceleration > 2.9f)
                        acceleration = 2.9f;
                }
                   
                //ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(361, 11.5f, 2.9f);
            }
        }

        internal static void LifeForceVerticalSpeed_Detour(Orig_LifeForceVerticalSpeed orig, LifeForce self, Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            orig(self, player, ref ascentWhenFalling, ref ascentWhenRising, ref maxCanAscendMultiplier, ref maxAscentMultiplier, ref constantAscend);
            if (NonFargoBossAlive())
            {
                ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(240, 9.5f, 2.7f);
                if (ascentWhenFalling > 0.85f)
                    ascentWhenFalling = 0.85f;
                if (ascentWhenRising > 0.15f)
                    ascentWhenRising = 0.15f;
                if (maxCanAscendMultiplier > 1f)
                    maxCanAscendMultiplier = 1f;
                if (maxAscentMultiplier > 3f)
                    maxAscentMultiplier = 3f;
                if (constantAscend > 0.135f)
                    constantAscend = 0.135f;
            }
            else
                ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new Terraria.DataStructures.WingStats(1000);
        }
        internal static void LifeForceHorizontalSpeed_Detour(Orig_LifeForceHorizontalSpeed orig, LifeForce self, Player player, ref float speed, ref float acceleration)
        {
            orig(self, player, ref speed, ref acceleration);
            if (NonFargoBossAlive())
            {
                
                //ArmorIDs.Wing.Sets.Stats[self.Item.wingSlot] = new WingStats(361, 11.5f, 2.9f);
            }
        }

        internal static bool IsFargoSoulsItem_Detour(Orig_IsFargoSoulsItem orig, Item item)
        {
            bool result = orig(item);
            if (item.ModItem is not null && item.ModItem.Mod == FargowiltasCrossmod.Instance)
                return true;
            return result;
        }
        internal static bool BrimstoneMonsterCanHitPlayer_Detour(Orig_BrimstoneMonsterCanHitPlayer orig, BrimstoneMonster self, Player player)
        {
            if (self.Type != ModContent.ProjectileType<BrimstoneMonster>())
            {
                return orig(self, player);
            }
            float distSQ = self.Projectile.DistanceSQ(player.Center);
            float radiusSQ = MathF.Pow(170f * self.Projectile.scale, 2);
            if (distSQ > radiusSQ)
                return false;
            return orig(self, player);
        }
        internal static void FargoSoulsOnSpawnProj_Detour(Orig_FargoSoulsOnSpawnProj orig, FargoSoulsGlobalProjectile self, Projectile proj, IEntitySource source)
        {
            if (proj.type == ModContent.ProjectileType<TitaniumRailgunScope>())
            {
                proj.FargoSouls().CanSplit = false;
            }
            
            orig(self, proj, source);
        }
        internal static void TryUnlimBuff_Detour(Orig_TryUnlimBuff orig, Item item, Player player)
        {
            if (item.type != ModContent.ItemType<AstralInjection>())
            {
                orig(item, player);

            }
            
        }
        internal static void NatureChampAI_Detour(Orig_NatureChampAI orig, NatureChampion self)
        {
            NPC npc = self.NPC;
            double originalSurface = Main.worldSurface;
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = 0;
            }
            orig(self);
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = originalSurface;
            }
        }
        internal static void TerraChampAI_Detour(Orig_TerraChampAI orig, TerraChampion self)
        {
            NPC npc = self.NPC;
            double originalSurface = Main.worldSurface;
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = 0;
            }
            orig(self);
            if (BossRushEvent.BossRushActive)
            {
                Main.worldSurface = originalSurface;
            }
        }
        internal static bool CheckTempleWalls_Detour(Orig_CheckTempleWalls orig, Vector2 pos)
        {
            
            if (BossRushEvent.BossRushActive)
            {
                return true;
            }
            return orig(pos);
        }
        internal static bool DukeFishronPreAI_Detour(Orig_DukeFishronPreAI orig, DukeFishron self, NPC npc)
        {
            if (BossRushEvent.BossRushActive && npc.HasValidTarget)
            {
                Main.player[npc.target].ZoneBeach = true;
            }
            bool result = orig(self, npc);
            if (BossRushEvent.BossRushActive && npc.HasValidTarget)
            {
                Main.player[npc.target].ZoneBeach = false;
            }
            return result;
        }
        internal static bool? MoonLordCanBeHitByProjectile_Detour(Orig_MoonLordCanBeHitByProjectile orig, MoonLord self, NPC npc, Projectile projectile)
        {
            bool? ret = orig(self, npc, projectile);
            if (!Main.player[projectile.owner].buffImmune[ModContent.BuffType<NullificationCurseBuff>()])
            {

                switch (self.GetVulnerabilityState(npc))
                {
                    case 0: //if (!projectile.CountsAsClass(DamageClass.Melee)) return false; break; melee
                        if (projectile.CountsAsClass<RogueDamageClass>())
                            ret = null;
                        break;
                    //case 1: if (!projectile.CountsAsClass(DamageClass.Ranged)) return false; break;
                    //case 2: if (!projectile.CountsAsClass(DamageClass.Magic)) return false; break;
                    //case 3: if (!FargoSoulsUtil.IsSummonDamage(projectile)) return false; break;
                    default: break;
                }
            }
            return ret;
        }
        internal static float TungstenIncreaseWeaponSize_Detour(Orig_TungstenIncreaseWeaponSize orig, FargoSoulsPlayer modPlayer)
        {
            float value = orig(modPlayer);
            if (modPlayer.Player.HeldItem == null)
                return value;
            if (CalDLCSets.Items.TungstenExclude[modPlayer.Player.HeldItem.type])
                return 1f;
            //if (modPlayer.Player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee))
            //    value -= (value - 1f) * 0.5f;
            return value;
        }
        internal static bool TungstenNerfedProj_Detour(Orig_TungstenNerfedProj orig, Projectile projectile)
        {
            bool value = orig(projectile); 
            /*
            if (!projectile.owner.IsWithinBounds(Main.maxPlayers))
                return value;
            Player player = Main.player[projectile.owner];
            if (!player.Alive())
                return value;
            if (player.HeldItem != null && player.HeldItem.DamageType.CountsAsClass(DamageClass.Melee))
            {
                return true;
            }
            */
            return value;
        }
        internal static bool TungstenNeverAffectsProj_Detour(Orig_TungstenNeverAffectsProj orig, Projectile projectile)
        {
            bool value = orig(projectile);
            if (CalDLCSets.Projectiles.TungstenExclude[projectile.type])
                return true;
            return value;
        }
        internal static void ModifyHurtInfo_Calamity_Detour(Orig_ModifyHurtInfo_Calamity orig, CalamityPlayer self, ref Player.HurtInfo info)
        {
            bool chalice = self.chaliceOfTheBloodGod;
            if (self.Player.FargoSouls().GuardRaised || self.Player.FargoSouls().MutantPresence)
                self.chaliceOfTheBloodGod = false;
            orig(self, ref info);
            self.chaliceOfTheBloodGod = chalice;
        }
        internal static void MinimalEffects_Detour(Orig_MinimalEffects orig, ToggleBackend self)
        {
            orig(self);
            Player player = Main.LocalPlayer;
            player.SetToggleValue<OccultSkullCrownEffect>(true);
            player.SetToggleValue<PurityEffect>(true);
            player.SetToggleValue<TheSpongeEffect>(true);
            player.SetToggleValue<ChaliceOfTheBloodGodEffect>(true);
            player.SetToggleValue<YharimsGiftEffect>(true);
            player.SetToggleValue<DraedonsHeartEffect>(true);
            player.SetToggleValue<NebulousCoreEffect>(false);
            player.SetToggleValue<CalamityEffect>(true);

            player.SetToggleValue<AerospecJumpEffect>(true);

            player.SetToggleValue<NanotechEffect>(true);
            player.SetToggleValue<EclipseMirrorEffect>(true);
            player.SetToggleValue<AbyssalDivingSuitEffect>(true);
            player.SetToggleValue<NucleogenesisEffect>(true);
            player.SetToggleValue<ElementalQuiverEffect>(true);
            player.SetToggleValue<ElementalGauntletEffect>(true);
            player.SetToggleValue<EtherealTalismanEffect>(true);
            player.SetToggleValue<AmalgamEffect>(true);
            player.SetToggleValue<AsgardianAegisEffect>(true);
            player.SetToggleValue<RampartofDeitiesEffect>(true);

        }

        public static void DialogueReplacement(Orig_BRDialogueTick orig)
        {
            
            BossRushDialoguePhase phase = BossRushDialogueSystem.Phase;
            FieldInfo tierInfo = typeof(BossRushEvent).GetField("CurrentTier");
            if (tierInfo != null)
            {
                tierInfo.SetValue(tierInfo, 1);
            }
            else
            {
                //Main.NewText(BossRushEvent.BossRushStage);
            }
            //BossRushEvent.BossRushStage = 16;
            //DownedBossSystem.startedBossRushAtLeastOnce = true;
            //Main.NewText(BossRushEvent.Bosses[BossRushEvent.Bosses.Count - 1].EntityID);
            //Main.NewText(ModContent.NPCType<MutantBoss>());
            if (BossRushDialogueSystem.CurrentDialogueDelay > 0 && phase == BossRushDialoguePhase.Start)
            {
                BossRushDialogueSystem.CurrentDialogueDelay -= 5;
                if (BossRushDialogueSystem.CurrentDialogueDelay < 0)
                {
                    BossRushDialogueSystem.CurrentDialogueDelay = 0;
                }
            }
            if (!BossRushEvent.BossRushActive || BossRushDialogueSystem.Phase == BossRushDialoguePhase.Start || BossRushDialogueSystem.Phase == BossRushDialoguePhase.None)
            {
                
                orig();
                return;
            }
            int currSequenceLength = 0;
            int currLine = BossRushDialogueSystem.currentSequenceIndex;
            
            if (phase == BossRushDialoguePhase.StartRepeat)
            {
                currSequenceLength = 1;
            }
            if (phase == BossRushDialoguePhase.TierOneComplete)
            {
                currSequenceLength = 3;
            }
            
            if (BossRushDialogueSystem.CurrentDialogueDelay == 0)
            {
                if (phase == BossRushDialoguePhase.StartRepeat && currLine == 0)
                {
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.BossRushDialogue.Start"), Color.Teal);
                    BossRushEvent.BossRushStage = 1;
                }
                if (phase == BossRushDialoguePhase.TierOneComplete)
                {
                    if (currLine == 0)
                        Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.BossRushDialogue.EndP1_1"), Color.Teal);
                    //if (currLine == 1)
                        
                    if (currLine == 2)
                        Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.BossRushDialogue.EndP1_2"), Color.Teal);
                }
                BossRushDialogueSystem.CurrentDialogueDelay = 60;
                BossRushDialogueSystem.currentSequenceIndex += 1;
                
            }
            
            else
            {
                --BossRushDialogueSystem.CurrentDialogueDelay;
            }
            if (phase == BossRushDialoguePhase.End || phase == BossRushDialoguePhase.EndRepeat)
            {
                BossRushDialogueSystem.CurrentDialogueDelay = 0;
            }
            if (phase == BossRushDialoguePhase.TierOneComplete && currLine < 6)
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 0, 0.05f);
            }
            if ( phase == BossRushDialoguePhase.TierOneComplete && currLine > 6
                )
            {
                Main.musicFade[Main.curMusic] = MathHelper.Lerp(Main.musicFade[Main.curMusic], 1, 0.001f);
            }
            if (BossRushEvent.BossRushSpawnCountdown < 180 && currLine < currSequenceLength) 
                BossRushEvent.BossRushSpawnCountdown = BossRushDialogueSystem.CurrentDialogueDelay + 180;
        }
        internal static bool FargoPlayerPreKill_Detour(Orig_FargoPlayerPreKill orig, FargoSoulsPlayer self, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool retval = orig(self, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
            if (!retval)
            {
                CalamityPlayer calPlayer = self.Player.Calamity();
                calPlayer.chaliceBleedoutBuffer = 0D;
                calPlayer.chaliceDamagePointPartialProgress = 0D;
                calPlayer.chaliceHitOriginalDamage = 0;
            }
            return retval;
        }

        internal static void EModePlayerPreUpdate_Detour(Orig_EModePlayerPreUpdate orig, EModePlayer self)
        {
            FargoSoulsPlayer soulsPlayer = self.Player.FargoSouls();
            bool antibodies = soulsPlayer.MutantAntibodies;
            if (self.Player.Calamity().oceanCrest || self.Player.Calamity().aquaticEmblem)
            {
                soulsPlayer.MutantAntibodies = true;
            }
            orig(self);
            soulsPlayer.MutantAntibodies = antibodies;
        }

        internal static bool CanToggleEternity_Detour(Orig_CanToggleEternity orig)
        {
            orig();
            return false;
        }

        internal static void SoulTogglerOnActivate_Detour(Orig_SoulTogglerOnActivate orig, SoulTogglerButton self)
        {
            orig(self);
            self.OncomingMutant.TextHoldShift = $"{Language.GetTextValue("Mods.FargowiltasCrossmod.UI.ToggledWithCal")}]\n[c/787878:{self.OncomingMutant.TextHoldShift}";
        }

        internal static float GetAdrenalineDamage_Detour(Orig_GetAdrenalineDamage orig, CalamityPlayer mp)
        {
            float value = orig(mp);
            if (WorldSavingSystem.EternityMode)
                value *= 0.5f;
            return value;
        }

        internal static bool DetermineDrawEligibility_Detour(Orig_DetermineDrawEligibility orig)
        {
            if (SkyManager.Instance["CalamityMod:BossRush"] != null && SkyManager.Instance["CalamityMod:BossRush"].IsActive())
                SkyManager.Instance.Deactivate("CalamityMod:BossRush", new object[0]);
            if (Filters.Scene["CalamityMod:BossRush"].IsActive())
                Filters.Scene["CalamityMod:BossRush"].Deactivate(new object[0]);
            /*
            if (useEffect != Filters.Scene["CalamityMod:BossRush"].IsActive())
            {
                if (useEffect)
                    Filters.Scene.Activate("CalamityMod:BossRush");
                else
                    Filters.Scene["CalamityMod:BossRush"].Deactivate(new object[0]);
            }
            */

            return false;
        }

        internal static void MediumPerforatorHeadOnKill_Detour(Orig_MediumPerforatorHeadOnKill orig, PerforatorHeadMedium self)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
                return;
            orig(self);
        }

        internal static void MediumPerforatorBodyOnKill_Detour(Orig_MediumPerforatorBodyOnKill orig, PerforatorBodyMedium self)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
                return;
            orig(self);
        }
        internal static void MediumPerforatorTailOnKill_Detour(Orig_MediumPerforatorTailOnKill orig, PerforatorTailMedium self)
        {
            if (CalDLCWorldSavingSystem.E_EternityRev)
                return;
            orig(self);
        }

        internal static EmodeItemBalance.EModeChange EmodeBalance_Detour(Orig_EmodeBalance orig, ref Item item, ref float balanceNumber, ref string[] balanceTextKeys, ref string extra)
        {
            if (CalDLCSets.GetValue(CalDLCSets.Items.DisabledEmodeChanges, item.type))
                return EmodeItemBalance.EModeChange.None;
            return orig(ref item, ref balanceNumber, ref balanceTextKeys, ref extra);
        }

        internal static void EmodeEditSpawnPool_Detour(Orig_EmodeEditSpawnPool orig, EModeGlobalNPC self, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            var cal = spawnInfo.Player.Calamity();
            if (cal.ZoneAbyss || cal.ZoneAstral || cal.ZoneCalamity || cal.ZoneSulphur || cal.ZoneSunkenSea)
                return;
            orig(self, pool, spawnInfo);
        }
        internal static void DropSummon_Int_Detour(Orig_DropSummon_Int_Method orig, NPC npc, int itemType, bool downed, ref bool dropped, bool prerequisite = true)
        {
            return;
        }
        internal static void DropSummon_String_Detour(Orig_DropSummon_String_Method orig, NPC npc, string itemType, bool downed, ref bool dropped, bool prerequisite = true)
        {
            return;
        }
        internal static void StarterBag_ModifyItemLoot_Detour(Orig_StarterBag_ModifyItemLoot orig, StarterBag self, ItemLoot itemLoot)
        {
            itemLoot.Add(ItemID.SilverPickaxe);
            itemLoot.Add(ItemID.SilverAxe);
            itemLoot.Add(ItemID.SilverHammer);
            


            LeadingConditionRule tin = itemLoot.DefineConditionalDropSet(() => WorldGen.SavedOreTiers.Copper == TileID.Tin);
            tin.Add(ItemID.TinBroadsword);
            tin.Add(ItemID.TinBow);
            tin.Add(ItemID.TopazStaff);
            tin.OnFailedConditions(new CommonDrop(ItemID.CopperBroadsword, 1));
            tin.OnFailedConditions(new CommonDrop(ItemID.CopperBow, 1));
            tin.OnFailedConditions(new CommonDrop(ItemID.AmethystStaff, 1));
            itemLoot.Add(ItemID.WoodenArrow, 1, 100, 100);
            itemLoot.Add(ModContent.ItemType<SquirrelSquireStaff>());
            itemLoot.Add(ModContent.ItemType<ThrowingBrick>(), 1, 150, 150);

            itemLoot.Add(ItemID.BugNet);
            itemLoot.Add(ItemID.WaterCandle);
            itemLoot.Add(ItemID.Torch, 1, 200, 200);
            itemLoot.Add(ItemID.LesserHealingPotion, 1, 15, 15);
            itemLoot.Add(ItemID.RecallPotion, 1, 15, 15);
            LeadingConditionRule mp = itemLoot.DefineConditionalDropSet(() => Main.netMode != NetmodeID.SinglePlayer);
            mp.Add(ItemID.WormholePotion, 1, 15, 15);
            LeadingConditionRule dontDigUp = itemLoot.DefineConditionalDropSet(() => Main.remixWorld || Main.zenithWorld);
            dontDigUp.Add(ItemID.ObsidianSkinPotion, 1, 5, 5);
            itemLoot.Add(ModContent.ItemType<EternityAdvisor>());
            itemLoot.Add(ModContent.ItemType<AutoHouse>(), 1, 2, 2);
            itemLoot.Add(ModContent.ItemType<MiniInstaBridge>(), 1, 2, 2);
            itemLoot.Add(ModContent.ItemType<EurusSock>());
            itemLoot.Add(ModContent.ItemType<PuffInABottle>());
            itemLoot.Add(ItemID.Squirrel);

            static bool isTerry(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName.ToLower().Contains("terry");
            }
            itemLoot.AddIf(isTerry, ModContent.ItemType<HalfInstavator>());
            itemLoot.AddIf(isTerry, ModContent.ItemType<RegalStatue>());
            itemLoot.AddIf(isTerry, ItemID.PlatinumCoin);
            itemLoot.AddIf(isTerry, ItemID.GrapplingHook);
            itemLoot.AddIf(isTerry, ItemID.LifeCrystal, new Fraction(1, 1), 4, 4);
            itemLoot.AddIf(isTerry, ItemID.ManaCrystal, new Fraction(1, 1), 2, 2);
            itemLoot.AddIf(isTerry, ModContent.ItemType<SandsofTime>());
            
            //fuck you terry
            static bool notreceivedStorage(DropAttemptInfo info)
            {
                return !WorldSavingSystem.ReceivedTerraStorage;
            }
            static bool TerryAndNotReceived(DropAttemptInfo info)
            {
                return notreceivedStorage(info) && isTerry(info);
            }
            static bool notTerryAndNotReceived(DropAttemptInfo info)
            {
                return notreceivedStorage(info) && !isTerry(info);
            }
            IItemDropRule notrecievedStorage = itemLoot.DefineConditionalDropSet(() => !WorldSavingSystem.ReceivedTerraStorage);
            if (ModLoader.HasMod("MagicStorage"))
            {
                itemLoot.AddIf(notreceivedStorage, ModContent.Find<ModItem>("MagicStorage", "StorageHeart").Type);
                itemLoot.AddIf(notreceivedStorage, ModContent.Find<ModItem>("MagicStorage", "CraftingAccess").Type);
                itemLoot.AddIf(TerryAndNotReceived, ModContent.Find<ModItem>("MagicStorage", "StorageUnit").Type, new Fraction(1, 1), 16, 16);
                itemLoot.AddIf(notTerryAndNotReceived, ModContent.Find<ModItem>("MagicStorage", "StorageUnit").Type, new Fraction(1, 1), 4, 4);
                WorldSavingSystem.ReceivedTerraStorage = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
            else if (ModLoader.HasMod("MagicStorageExtra"))
            {
                itemLoot.AddIf(notreceivedStorage, ModContent.Find<ModItem>("MagicStorageExtra", "StorageHeart").Type);
                itemLoot.AddIf(notreceivedStorage, ModContent.Find<ModItem>("MagicStorageExtra", "CraftingAccess").Type);
                itemLoot.AddIf(TerryAndNotReceived, ModContent.Find<ModItem>("MagicStorageExtra", "StorageUnit").Type, new Fraction(1, 1), 16, 16);
                itemLoot.AddIf(notTerryAndNotReceived, ModContent.Find<ModItem>("MagicStorageExtra", "StorageUnit").Type, new Fraction(1, 1), 4, 4);
                WorldSavingSystem.ReceivedTerraStorage = true;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.WorldData);
            }
            //itemLoot.Add(isTerry);
            if (ModLoader.TryGetMod("CalamityModMusic", out Mod musicMod))
                itemLoot.Add(musicMod.Find<ModItem>("CalamityMusicbox").Type);

            // Awakening lore item
            itemLoot.Add(ModContent.ItemType<LoreAwakening>());

            // Aleksh donator item
            // Name specific: "Aleksh" or "Shark Lad"
            static bool getsLadPet(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName == "Aleksh" || playerName == "Shark Lad";
            }
            ;
            itemLoot.AddIf(getsLadPet, ModContent.ItemType<JoyfulHeart>());

            // HPU dev item
            // Name specific: "Heart Plus Up"
            static bool getsHapuFruit(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName == "Heart Plus Up";
            }
            ;
            itemLoot.AddIf(getsHapuFruit, ModContent.ItemType<HapuFruit>());

            // Apelusa dev item
            // Name specific: "Pelusa"
            static bool getsRedBow(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName == "Pelusa";
            }

            itemLoot.AddIf(getsRedBow, ModContent.ItemType<RedBow>());

            // Mishiro dev vanity
            // Name specific: "Amber" or "Mishiro"
            static bool getsOracleHeadphones(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName is "Amber" or "Mishiro";
            }

            itemLoot.AddIf(getsOracleHeadphones, ModContent.ItemType<OracleHeadphones>());

            // Fabsol dev item
            // Name specific: "Fabsol" or "Cirrus"
            static bool getsCrystalHeartVodka(DropAttemptInfo info)
            {
                string playerName = info.player.name;
                return playerName is "Fabsol" or "Cirrus";
            }

            itemLoot.AddIf(getsCrystalHeartVodka, ModContent.ItemType<CrystalHeartVodka>());
        }
        internal static void FargosSouls_DropDevianttsGift_Detour(Orig_FargosSouls_DropDevianttsGift orig, Player player)
        {
            return;
        }
        internal static void Instahouse_GetTiles_Detour(Orig_Instahouse_GetTiles orig, Player player, out int wallType, out int tileType, out int platformStyle, out bool moddedPlatform)
        {
            orig(player, out wallType, out tileType, out platformStyle, out moddedPlatform);
            if (player.Calamity().ZoneSulphur)
            {
                wallType = ModContent.WallType<AcidwoodWall>();
                tileType = ModContent.TileType<AcidwoodTile>();
                platformStyle = ModContent.TileType<AcidwoodPlatformTile>();
                moddedPlatform = true;
            }
            if (player.Calamity().ZoneAbyss)
            {
                wallType = ModContent.WallType<SmoothAbyssGravelWall>();
                tileType = ModContent.TileType<SmoothAbyssGravel>();
                platformStyle = ModContent.TileType<SmoothAbyssGravelPlatform>();
                moddedPlatform = true;
            }
            if (player.Calamity().ZoneAstral)
            {
                wallType = ModContent.WallType<AstralMonolithWall>();
                tileType = ModContent.TileType<AstralMonolith>();
                platformStyle = ModContent.TileType<MonolithPlatform>();
                moddedPlatform = true;
            }
            if (player.Calamity().ZoneCalamity)
            {
                wallType = ModContent.WallType<BrimstoneSlabWall>();
                tileType = ModContent.TileType<BrimstoneSlab>();
                platformStyle = ModContent.TileType<AshenPlatform>();
                moddedPlatform = true;
            }
            if (player.Calamity().ZoneSunkenSea)
            {
                wallType = ModContent.WallType<SmoothNavystoneWall>();
                tileType = ModContent.TileType<SmoothNavystone>();
                platformStyle = ModContent.TileType<EutrophicPlatform>();
                moddedPlatform = true;
            }
        }
        internal static void Instahouse_GetFurniture_Detour(Orig_Instahouse_GetFurniture orig, Player player, out int doorStyle, out int chairStyle, out int tableStyle, out int torchStyle)
        {
            orig(player, out doorStyle, out chairStyle, out tableStyle, out torchStyle);
            if (player.Calamity().ZoneSulphur)
            {
                doorStyle = ModContent.TileType<AcidwoodDoorClosed>();
                chairStyle = ModContent.TileType<AcidwoodChairTile>();
                tableStyle = ModContent.TileType<AcidwoodTableTile>();
                torchStyle = ModContent.TileType<SulphurousTorch>();
            }
            if (player.Calamity().ZoneAbyss)
            {
                doorStyle = ModContent.TileType<AbyssDoorClosed>();
                chairStyle = ModContent.TileType<AbyssChair>();
                tableStyle = ModContent.TileType<AbyssTable>();
                torchStyle = ModContent.TileType<AbyssTorch>();
            }
            if (player.Calamity().ZoneAstral)
            {
                doorStyle = ModContent.TileType<MonolithDoorClosed>();
                chairStyle = ModContent.TileType<MonolithChair>();
                tableStyle = ModContent.TileType<MonolithTable>();
                torchStyle = ModContent.TileType<AstralTorch>();
            }
            if (player.Calamity().ZoneCalamity)
            {
                doorStyle = ModContent.TileType<AshenDoorClosed>();
                chairStyle = ModContent.TileType<AshenChair>();
                tableStyle = ModContent.TileType<AshenTable>();
                torchStyle = ModContent.TileType<GloomTorch>();
            }
            if (player.Calamity().ZoneSunkenSea)
            {
                doorStyle = ModContent.TileType<EutrophicDoorClosed>();
                chairStyle = ModContent.TileType<EutrophicChair>();
                tableStyle = ModContent.TileType<EutrophicTable>();
                torchStyle = ModContent.TileType<NavyPrismTorch>();
            }
        }
        #endregion
        #region Vanilla Detours
        internal static void NPCAddBuff_Detour(On_NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet)
        {
            if (self.CalamityDLC().ImmuneToAllDebuffs)
                return;
            orig(self, type, time, quiet);
        }
        #endregion
    }
}
