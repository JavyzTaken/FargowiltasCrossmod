
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls;
using Terraria.Audio;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Plantera
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathPlantera : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Plantera);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write7BitEncodedInt(dashTimer);
            binaryWriter.Write(dashing);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            timer = binaryReader.Read7BitEncodedInt();
            dashTimer = binaryReader.Read7BitEncodedInt();
            dashing = binaryReader.ReadBoolean();
        }
        
        public int timer = 0;
        public int dashTimer = 0;
        public bool dashing = false;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            Player target = Main.player[npc.target];
            FargowiltasSouls.Content.Bosses.VanillaEternity.Plantera plant = npc.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.Plantera>();
            
            if (npc.localAI[0] == 1)
            {
                
                timer++;
                if (timer >= 800 )
                {
                    timer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<HomingGasBulb>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
            }
            else
            {
                if (dashing)
                {
                    if (dashTimer == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Roar, npc.Center);
                        npc.velocity = (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 15;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 15; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, Main.rand.Next(5, 10)).RotatedBy(MathHelper.ToRadians(360 / 15f * i)), ModContent.ProjectileType<SporeGasPlantera>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage / 2), 0);
                            }
                        }
                    }
                    dashTimer++;
                    if (dashTimer >= 60)
                    {
                        dashing = false;
                        dashTimer = 0;
                    }
                    return false;
                }
                if (plant.TentacleTimer == -390)
                {
                    dashing = true;
                }
                //Main.NewText(plant.TentacleTimer);
            }
            return base.SafePreAI(npc);
        }
    }
}
