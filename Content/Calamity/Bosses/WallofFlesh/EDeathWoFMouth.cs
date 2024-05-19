using CalamityMod;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.WallofFlesh
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathWoFMouth : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.WallofFlesh);

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            FargowiltasSouls.Content.Bosses.VanillaEternity.WallofFlesh wof = npc.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.WallofFlesh>();
            //Main.NewText(wof.WorldEvilAttackCycleTimer);
            //if (wof.WorldEvilAttackCycleTimer == 150) Old condition
            if (wof.WorldEvilAttackCycleTimer == (wof.InDesperationPhase ? 300 : 600 - 120) - 90) // 1.5 seconds before telegraphing world evil attack
            {
                if (DLCUtils.HostCheck)
                {
                    Vector2 projectileVelocity = (Main.player[npc.target].Center - npc.Center).SafeNormalize(Vector2.UnitY) * npc.velocity.Length();
                    Vector2 projectileSpawn = npc.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 50f;

                    int damage = npc.GetProjectileDamage(ProjectileID.DemonSickle);
                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileSpawn, projectileVelocity, ProjectileID.DemonSickle, damage, 0f, Main.myPlayer, 0f, projectileVelocity.Length() * 3f);
                    Main.projectile[proj].timeLeft = 600;
                    Main.projectile[proj].tileCollide = false;
                }
                // Old sickle
                //if (DLCUtils.HostCheck)
                //    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(1, 0) * npc.spriteDirection, ModContent.ProjectileType<HomingSickle>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);
                SoundEngine.PlaySound(SoundID.Item8 with { Volume = 1.75f, Pitch = -0.5f}, npc.Center);
            }
            return base.SafePreAI(npc);
        }
    }
}
