using System.IO;
using System.Linq;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using EmodePlantera = FargowiltasSouls.Content.Bosses.VanillaEternity.Plantera;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Plantera
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathPlantera : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.Plantera;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(dashTimer);
            binaryWriter.Write(dashing);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            timer = binaryReader.Read7BitEncodedInt();
            dashTimer = binaryReader.Read7BitEncodedInt();
            dashing = binaryReader.ReadBoolean();
        }

        public int timer = 0;
        public int dashTimer = 0;
        public bool dashing = false;
        public bool Phase3Clear = false;
        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            Player target = Main.player[NPC.target];
            EmodePlantera plant = NPC.GetGlobalNPC<EmodePlantera>();

            if (NPC.localAI[0] == 1)
            {
                NPC.localAI[1] -= 0.5f; // to compensate for spore clouds; less mouth seeds
                timer++;
                if (timer >= 800)
                {
                    timer = 0;
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<HomingGasBulb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }

                if (plant.CrystalRedirectTimer >= 2 && plant.RingTossTimer == 360 + 65) // crystal redirect
                {
                    timer = 800 - (60 * 4); // reset gas bulbs, fire in 3 seconds
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile p = Main.projectile[i];
                        if (p.TypeAlive<HomingGasBulbSporeGas>() && p.ai[1] < 900f)
                        {
                            p.ai[1] = 900f;
                        }
                            
                    }
                }
            }
            else
            {
                if (dashing)
                {
                    if (dashTimer == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie21, NPC.Center);
                        if (FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, -1, NPC.whoAmI, -11);
                        }
                    }
                    if (dashTimer < 0)
                    {
                        NPC.rotation = NPC.AngleTo(target.Center);

                    }
                    if (dashTimer == 30)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                        NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                        if (DLCUtils.HostCheck)
                        {
                            for (int i = 0; i < 15; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.Next(5, 10)).RotatedBy(MathHelper.ToRadians(360 / 15f * i)), ModContent.ProjectileType<SporeGasPlantera>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage / 2), 0);
                            }
                        }
                    }
                    dashTimer++;
                    if (dashTimer >= 90)
                    {
                        dashing = false;
                        dashTimer = 0;
                    }
                    return false;
                }
                if (plant.EnteredPhase3 && !Phase3Clear) // for clearing anything we need to clear
                {
                    
                }
                if (plant.TentacleTimer == -390)
                {
                    dashing = true;
                }
                //Main.NewText(plant.TentacleTimer);
            }
            return true;
        }
    }
}
