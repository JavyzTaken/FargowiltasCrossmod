using System;
using System.IO;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.EyeOfCthulhu
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathEoC : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.EyeofCthulhu);

        private int TeleportCounter = 0;
        private int Side = 0;
        private bool CheckedTeleport = false;
        private bool HorizDash = false;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(TeleportCounter);
            binaryWriter.Write7BitEncodedInt(Side);
            binaryWriter.Write(CheckedTeleport);
            binaryWriter.Write(HorizDash);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            TeleportCounter = binaryReader.Read7BitEncodedInt();
            Side = binaryReader.Read7BitEncodedInt();
            CheckedTeleport = binaryReader.ReadBoolean();
            HorizDash = binaryReader.ReadBoolean();
        }
        public override bool SafePreAI(NPC npc)
        {
            ref float ai_Phase = ref npc.ai[0];
            ref float ai_AttackState = ref npc.ai[1];
            ref float ai_Timer = ref npc.ai[2];

            if (!npc.HasValidTarget || npc == null)
            {
                return true;
            }
            npc.GetGlobalNPC<EyeofCthulhu>().RunEmodeAI = true;
            if (HorizDash)
            {
                npc.GetGlobalNPC<EyeofCthulhu>().RunEmodeAI = false;
                HorizontalDash(npc);
                return true;
            }
            if (ai_Phase == 3 && (ai_AttackState == 0 || ai_AttackState == 5)) 
            {
                if (ai_Timer >= 2 && npc.alpha <= 60) //after teleport and post-teleport dash
                {
                    if (!CheckedTeleport)
                    {
                        TeleportCounter++;
                        if (TeleportCounter >= 2)
                        {
                            TeleportCounter = 0;
                            HorizDash = true;
                            ai_AttackState = 15;
                            ai_Timer = 0;
                            Side = Math.Sign(npc.Center.X - Main.player[npc.target].Center.X);
                        }
                        CheckedTeleport = true;
                    }
                }
                
            }
            else
            {
                CheckedTeleport = false;
            }
            return true;
        }
        const int WindupTime = 100;
        private void HorizontalDash(NPC npc) //i stole calamity horizontal dash ai and adapted it a bit
        {
            ref float ai_Phase = ref npc.ai[0];
            ref float ai_AttackState = ref npc.ai[1];
            ref float ai_Timer = ref npc.ai[2];

            if (npc.alpha > 0)
                npc.alpha--;


            float lifeRatio = npc.life / (float)npc.lifeMax;
            float br = BossRushEvent.BossRushActive ? 1f : 0f;
            if (ai_AttackState == 15)
            {
                float num40 = 600f;
                float num41 = 6f * (0.4f - lifeRatio);
                float num42 = 0.15f * (0.4f - lifeRatio);
                float num43 = 8f + num41;
                float num44 = 0.25f + num42;

                Vector2 destination;
                //bool flag6 = newAI0 == 1f || newAI0 == 3f;
                num40 = Side == -1 ? -500f : 500f;
                num43 *= 1.5f;
                num44 *= 1.5f;
                destination = Main.player[npc.target].Center + Vector2.UnitX * num40;
                Vector2 val13 = npc.SafeDirectionTo(destination) * num43;
                npc.rotation = Rotate(npc, npc.rotation + MathHelper.PiOver2, Main.player[npc.target].Center, 2) - MathHelper.PiOver2;
                npc.SimpleFlyMovement(val13, num44);

                ai_Timer += 1;
                int eyeDelay = FargowiltasSouls.Core.Systems.WorldSavingSystem.MasochistModeReal ? 2 : 45; //funny old gigavomit crossmod bug on maso
                if (ai_Timer % eyeDelay == 0f)
                {
                    Vector2 val14 = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 5f;
                    Vector2 val15 = npc.Center + val14 * 10f;
                    if (DLCUtils.HostCheck)
                    {
                        int num45 = NPC.NewNPC(npc.GetSource_FromAI(null), (int)val15.X, (int)val15.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num45].velocity.X = val14.X;
                        Main.npc[num45].velocity.Y = val14.Y;
                        if (Main.netMode == NetmodeID.Server && num45 < Main.maxNPCs)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num45, 0f, 0f, 0f, 0, 0, 0);
                        }
                        if (CalamityWorld.LegendaryMode)
                        {
                            int num46 = 814;
                            Vector2 val16 = val14 * 2f;
                            int num47 = 3;
                            float num48 = MathHelper.ToRadians(20f);
                            for (int n = 0; n < num47; n++)
                            {
                                Vector2 val17 = val16.RotatedBy((double)MathHelper.Lerp(0f - num48, num48, n / (float)(num47 - 1)), default);
                                Projectile.NewProjectile(npc.GetSource_FromAI(null), npc.Center + Vector2.Normalize(val17) * 10f, val17, num46, 15, 0f, Main.myPlayer, 0f, 0f, 0f);
                            }
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath13, (Vector2?)npc.position, null);
                    for (int num49 = 0; num49 < 10; num49++)
                    {
                        Dust.NewDust(val15, 20, 20, 5, val14.X * 0.4f, val14.Y * 0.4f, 0, default, 1f);
                    }
                }
                float num50 = WindupTime;
                if (ai_Timer >= num50)
                {
                    ai_AttackState = 16;
                    ai_Timer = 0;
                    npc.TargetClosest(true);
                    npc.SyncExtraAI();
                }
                npc.netUpdate = true;
                if (npc.netSpam > 10)
                {
                    npc.netSpam = 10;
                }
            }
            else if (ai_AttackState == 16)
            {
                if (DLCUtils.HostCheck)
                {
                    float num51 = 6f * (0.4f - lifeRatio);
                    float num52 = 18f + num51;
                    num52 += 10f * br;
                    npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * num52;
                    ai_AttackState = 17;
                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                    {
                        npc.netSpam = 10;
                    }
                }
            }
            else if (ai_AttackState == 17)
            {
                if (ai_Timer == 0)
                {
                    SoundEngine.PlaySound(SoundID.Roar, npc.position, null);
                }

                float dashTime = 40;
                if (ai_Timer % (dashTime / 2) == 0 && ai_Timer < dashTime)
                {
                    if (DLCUtils.HostCheck)
                        FargoSoulsUtil.XWay(8, npc.GetSource_FromThis(), npc.Center, ModContent.ProjectileType<BloodScythe>(), 1.5f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
                ai_Timer += 1;
                if (ai_Timer == dashTime && Vector2.Distance(npc.position, Main.player[npc.target].position) < 200f)
                {
                    ai_Timer -= 1;
                }
                if (ai_Timer >= dashTime)
                {
                    npc.velocity = npc.velocity * 0.95f;
                    if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
                    {
                        npc.velocity.X = 0f;
                    }
                    if (npc.velocity.Y > -0.1 && npc.velocity.Y < 0.1)
                    {
                        npc.velocity.Y = 0f;
                    }
                }
                else
                {
                    npc.rotation = npc.velocity.ToRotation() - (float)Math.PI / 2f;
                }
                float endTime = dashTime + 13f;
                if (ai_Timer >= endTime)
                {
                    npc.netUpdate = true;
                    if (npc.netSpam > 10)
                    {
                        npc.netSpam = 10;
                    }
                    ai_Timer = 2;
                    ai_AttackState = npc.GetLifePercent() <= 0.5f ? 5 : 0;
                    HorizDash = false;
                }
            }
            if (npc.GetLifePercent() <= 0.1f)
            {
                npc.netUpdate = true;
                if (npc.netSpam > 10)
                {
                    npc.netSpam = 10;
                }
                ai_Timer = 2;
                ai_AttackState = 0;
                HorizDash = false;
            }
        }
        float Rotate(NPC npc, float rotation, Vector2 target, float speed)
        {
            Vector2 PV = npc.DirectionTo(target);
            Vector2 LV = rotation.ToRotationVector2();
            float anglediff = (float)Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y); //real
            //change rotation towards target
            return rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
        }
    }
}
