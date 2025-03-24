using System;
using System.IO;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
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
    public class EDeathEoC : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.EyeofCthulhu;

        private int TeleportCounter = 0;
        private int Side = 0;
        private bool CheckedTeleport = false;
        private bool HorizDash = false;

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(TeleportCounter);
            binaryWriter.Write7BitEncodedInt(Side);
            binaryWriter.Write(CheckedTeleport);
            binaryWriter.Write(HorizDash);
        }

        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            TeleportCounter = binaryReader.Read7BitEncodedInt();
            Side = binaryReader.Read7BitEncodedInt();
            CheckedTeleport = binaryReader.ReadBoolean();
            HorizDash = binaryReader.ReadBoolean();
        }
        public override bool PreAI()
        {
            ref float ai_Phase = ref NPC.ai[0];
            ref float ai_AttackState = ref NPC.ai[1];
            ref float ai_Timer = ref NPC.ai[2];

            if (NPC == null || !NPC.HasValidTarget)
            {
                NPC.GetGlobalNPC<EyeofCthulhu>().RunEmodeAI = true;
                return true;
            }
            NPC.GetGlobalNPC<EyeofCthulhu>().RunEmodeAI = true;
            if (HorizDash)
            {
                NPC.GetGlobalNPC<EyeofCthulhu>().RunEmodeAI = false;
                NPC.dontTakeDamage = false;
                NPC.alpha = 0;
                HorizontalDash(NPC);
                return true;
            }
            if (ai_Phase == 3 && (ai_AttackState == 0 || ai_AttackState == 5)) 
            {
                if (ai_Timer >= 2 && NPC.alpha <= 60) //after teleport and post-teleport dash
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
                            Side = Math.Sign(NPC.Center.X - Main.player[NPC.target].Center.X);
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
        private void HorizontalDash(NPC NPC) //i stole calamity horizontal dash ai and adapted it a bit
        {
            ref float ai_Phase = ref NPC.ai[0];
            ref float ai_AttackState = ref NPC.ai[1];
            ref float ai_Timer = ref NPC.ai[2];

            if (NPC.alpha > 0)
                NPC.alpha--;


            float lifeRatio = NPC.life / (float)NPC.lifeMax;
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
                destination = Main.player[NPC.target].Center + Vector2.UnitX * num40;
                Vector2 val13 = NPC.SafeDirectionTo(destination) * num43;
                NPC.rotation = Rotate(NPC, NPC.rotation + MathHelper.PiOver2, Main.player[NPC.target].Center, 2) - MathHelper.PiOver2;
                NPC.SimpleFlyMovement(val13, num44);

                ai_Timer += 1;
                int eyeDelay = FargowiltasSouls.Core.Systems.WorldSavingSystem.MasochistModeReal ? 4 : 45; //funny old gigavomit crossmod bug on maso
                if (Main.getGoodWorld) eyeDelay = 2;
                if (ai_Timer % eyeDelay == 0f)
                {
                    Vector2 val14 = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 5f;
                    Vector2 val15 = NPC.Center + val14 * 10f;
                    if (DLCUtils.HostCheck)
                    {
                        int num45 = NPC.NewNPC(NPC.GetSource_FromAI(null), (int)val15.X, (int)val15.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
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
                                Projectile.NewProjectile(NPC.GetSource_FromAI(null), NPC.Center + Vector2.Normalize(val17) * 10f, val17, num46, 15, 0f, Main.myPlayer, 0f, 0f, 0f);
                            }
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath13, (Vector2?)NPC.position, null);
                    for (int num49 = 0; num49 < 10; num49++)
                    {
                        Dust.NewDust(val15, 20, 20, DustID.Blood, val14.X * 0.4f, val14.Y * 0.4f, 0, default, 1f);
                    }
                }
                float num50 = WindupTime;
                if (ai_Timer >= num50)
                {
                    ai_AttackState = 16;
                    ai_Timer = 0;
                    NPC.TargetClosest(true);
                    NPC.SyncExtraAI();
                }
                NPC.netUpdate = true;
                if (NPC.netSpam > 10)
                {
                    NPC.netSpam = 10;
                }
            }
            else if (ai_AttackState == 16)
            {
                if (DLCUtils.HostCheck)
                {
                    float num51 = 6f * (0.4f - lifeRatio);
                    float num52 = 18f + num51;
                    num52 += 10f * br;
                    NPC.velocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center) * num52;
                    ai_AttackState = 17;
                    NPC.netUpdate = true;

                    if (NPC.netSpam > 10)
                    {
                        NPC.netSpam = 10;
                    }
                }
            }
            else if (ai_AttackState == 17)
            {
                if (ai_Timer == 0)
                {
                    SoundEngine.PlaySound(SoundID.Roar, NPC.position, null);
                }

                float dashTime = 40;
                if (ai_Timer % (dashTime / 2) == 0 && ai_Timer < dashTime)
                {
                    if (DLCUtils.HostCheck)
                        FargoSoulsUtil.XWay(8, NPC.GetSource_FromThis(), NPC.Center, ModContent.ProjectileType<BloodScythe>(), 1.5f, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                }
                ai_Timer += 1;
                if (ai_Timer == dashTime && Vector2.Distance(NPC.position, Main.player[NPC.target].position) < 200f)
                {
                    ai_Timer -= 1;
                }
                if (ai_Timer >= dashTime)
                {
                    NPC.velocity = NPC.velocity * 0.95f;
                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                    {
                        NPC.velocity.X = 0f;
                    }
                    if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                    {
                        NPC.velocity.Y = 0f;
                    }
                }
                else
                {
                    NPC.rotation = NPC.velocity.ToRotation() - (float)Math.PI / 2f;
                }
                float endTime = dashTime + 13f;
                if (ai_Timer >= endTime)
                {
                    NPC.netUpdate = true;
                    if (NPC.netSpam > 10)
                    {
                        NPC.netSpam = 10;
                    }
                    ai_Timer = 2;
                    ai_AttackState = NPC.GetLifePercent() <= 0.5f ? 5 : 0;
                    HorizDash = false;
                }
            }
            if (NPC.GetLifePercent() <= 0.1f)
            {
                NPC.netUpdate = true;
                if (NPC.netSpam > 10)
                {
                    NPC.netSpam = 10;
                }
                ai_Timer = 2;
                ai_AttackState = 0;
                HorizDash = false;
            }
        }
        float Rotate(NPC NPC, float rotation, Vector2 target, float speed)
        {
            Vector2 PV = NPC.DirectionTo(target);
            Vector2 LV = rotation.ToRotationVector2();
            float anglediff = (float)Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y); //real
            //change rotation towards target
            return rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
        }
    }
}
