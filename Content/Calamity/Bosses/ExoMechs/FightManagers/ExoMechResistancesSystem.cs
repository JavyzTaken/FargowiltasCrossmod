using CalamityMod.NPCs.ExoMechs.Thanatos;
using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Content.Projectiles.Minions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.FightManagers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public sealed class ExoMechResistancesSystem : GlobalNPC
    {
        private static int blenderYoyoProjID;

        private static int blenderOrbitalProjID;

        private static int blenderPetalID;

        public override void SetStaticDefaults()
        {
            // These are internal for some reason so they need to be found without generics.
            blenderYoyoProjID = ModContent.Find<ModProjectile>("FargowiltasSouls/BlenderYoyoProj").Type;
            blenderOrbitalProjID = ModContent.Find<ModProjectile>("FargowiltasSouls/BlenderOrbital").Type;
            blenderPetalID = ModContent.Find<ModProjectile>("FargowiltasSouls/BlenderPetal").Type;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!CalDLCWorldSavingSystem.E_EternityRev)
                return;

            // Cache the projectile ID for ease of use.
            int projID = projectile.type;

            if (npc.type == ExoMechNPCIDs.AresBodyID || npc.type == ModContent.NPCType<AresHand>())
                ApplyAresResists(projID, ref modifiers);

            if (npc.type == ExoMechNPCIDs.HadesHeadID || npc.type == ModContent.NPCType<ThanatosBody1>())
                ApplyHadesResists(projID, projectile, ref modifiers);
        }

        private static void ApplyAresResists(int projID, ref NPC.HitModifiers modifiers)
        {
            // Gemini Glaives do 0.4x damage to Ares.
            bool isGeminiGlaivesProjectile = projID == ModContent.ProjectileType<Retiglaive>() || projID == ModContent.ProjectileType<Spazmaglaive>() ||
                                             projID == ModContent.ProjectileType<RetiDeathray>() || projID == ModContent.ProjectileType<MechElectricOrbHomingFriendly>() ||
                                             projID == ModContent.ProjectileType<SpazmaglaiveExplosion>();
            if (isGeminiGlaivesProjectile)
                modifiers.FinalDamage *= 0.4f;

            // Blender projectiles do 0.5x damage to Ares.
            bool isBlenderProjectile = projID == blenderYoyoProjID || projID == blenderOrbitalProjID || projID == blenderPetalID;
            if (isBlenderProjectile)
                modifiers.FinalDamage *= 0.5f;

            // Dragon's Demise projectiles do 0.5x damage to Ares.
            bool isDragonsDemiseProjectile = projID == ModContent.ProjectileType<DragonFireball>();
            if (isDragonsDemiseProjectile)
                modifiers.FinalDamage *= 0.5f;

            // The laserbeams from the Diffractor Blaster do 0.5x damage to Ares.
            bool isDiffractorBlasterLaser = projID == ModContent.ProjectileType<PrimeDeathray>();
            if (isDiffractorBlasterLaser)
                modifiers.FinalDamage *= 0.5f;
        }

        private static void ApplyHadesResists(int projID, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Gemini Glaives do 0.25x damage to Hades.
            bool isGeminiGlaivesProjectile = projID == ModContent.ProjectileType<Retiglaive>() || projID == ModContent.ProjectileType<Spazmaglaive>() ||
                                             projID == ModContent.ProjectileType<RetiDeathray>() || projID == ModContent.ProjectileType<MechElectricOrbHomingFriendly>() ||
                                             projID == ModContent.ProjectileType<SpazmaglaiveExplosion>();
            if (isGeminiGlaivesProjectile)
                modifiers.FinalDamage *= 0.25f;


            // The laserbeams from the Diffractor Blaster do 0.25x damage to Hades.
            bool isDiffractorBlasterLaser = projID == ModContent.ProjectileType<PrimeDeathray>();
            if (isDiffractorBlasterLaser)
                modifiers.FinalDamage *= 0.25f;

            // Umbra Regalia projectiles do 0.25x damage to Hades.
            bool isUmbraRegaliaProjectile = (projID == ModContent.ProjectileType<UmbraRegaliaProj>() && projectile.ai[0] >= 1f) || projID == ProjectileID.FairyQueenMagicItemShot;
            if (isUmbraRegaliaProjectile)
                modifiers.FinalDamage *= 0.25f;

            // Blender projectiles do 0.5x damage to Hades.
            bool isBlenderProjectile = projID == blenderYoyoProjID || projID == blenderOrbitalProjID || projID == blenderPetalID;
            if (isBlenderProjectile)
                modifiers.FinalDamage *= 0.5f;

            // Dragon's Demise projectiles do 0.5x damage to Hades.
            bool isDragonsDemiseProjectile = projID == ModContent.ProjectileType<DragonFireball>();
            if (isDragonsDemiseProjectile)
                modifiers.FinalDamage *= 0.5f;
        }
    }
}
