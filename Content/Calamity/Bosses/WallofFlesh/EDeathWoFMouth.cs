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
    public class EDeathWoFMouth : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.WallofFlesh;

        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            FargowiltasSouls.Content.Bosses.VanillaEternity.WallofFlesh wof = NPC.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.WallofFlesh>();
            //Main.NewText(wof.WorldEvilAttackCycleTimer);
            //if (wof.WorldEvilAttackCycleTimer == 150) Old condition
            if (wof.WorldEvilAttackCycleTimer == (wof.InDesperationPhase ? 300 : 600 - 120) - 90) // 1.5 seconds before telegraphing world evil attack
            {
                if (DLCUtils.HostCheck)
                {
                    Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * NPC.velocity.Length();
                    Vector2 projectileSpawn = NPC.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 50f;

                    int damage = NPC.GetProjectileDamage(ProjectileID.DemonSickle);
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, ProjectileID.DemonSickle, damage, 0f, Main.myPlayer, 0f, projectileVelocity.Length() * 3f);
                    Main.projectile[proj].timeLeft = 600;
                    Main.projectile[proj].tileCollide = false;
                }
                // Old sickle
                //if (DLCUtils.HostCheck)
                //    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(1, 0) * NPC.spriteDirection, ModContent.ProjectileType<HomingSickle>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                SoundEngine.PlaySound(SoundID.Item8 with { Volume = 1.75f, Pitch = -0.5f}, NPC.Center);
            }
            return true;
        }
    }
}
