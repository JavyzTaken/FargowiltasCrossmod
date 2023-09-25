using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs;
using CalamityMod.World;
using CalamityMod;
using FargowiltasCrossmod.Core.Systems;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;
using CalamityMod.Events;
using FargowiltasSouls.Content.Projectiles.Masomode;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Vanilla.EyeOfCthulhu
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathEoC : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.EyeofCthulhu);

        private int TeleportCounter = 0;
        private int Side = 0;
        private bool CheckedTeleport = false;
        private bool HorizDash = false;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            binaryWriter.Write7BitEncodedInt(TeleportCounter);
            binaryWriter.Write7BitEncodedInt(Side);
            binaryWriter.Write(CheckedTeleport);
            binaryWriter.Write(HorizDash);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            TeleportCounter = binaryReader.Read7BitEncodedInt();
            Side = binaryReader.Read7BitEncodedInt();
            CheckedTeleport = binaryReader.ReadBoolean();
            HorizDash = binaryReader.ReadBoolean();
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            npc.GetGlobalNPC<EModeNPCBehaviour>().RunEmodeAI = true;
            if (HorizDash)
            {
                npc.GetGlobalNPC<EModeNPCBehaviour>().RunEmodeAI = false;
                HorizontalDash(npc);
                return false;
            }
            if (npc.ai[0] == 3 && (npc.ai[1] == 0 || npc.ai[1] == 5))
            {
                if (!CheckedTeleport)
                {
                    TeleportCounter++;
                    if (TeleportCounter >= 2)
                    {
                        TeleportCounter = 0;
                        HorizDash = true;
                        npc.ai[1] = 15;
                        Side = Math.Sign(npc.Center.X - Main.player[npc.target].Center.X);
                    }
                    CheckedTeleport = true;
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
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;
            float br = BossRushEvent.BossRushActive ? 1f : 0f;
            if (npc.ai[1] == 15)
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
                destination = (Main.player[npc.target]).Center + Vector2.UnitX * num40;
                Vector2 val13 = npc.SafeDirectionTo(destination) * num43;
                npc.rotation = Rotate(npc, npc.rotation + MathHelper.PiOver2, Main.player[npc.target].Center, 2) - MathHelper.PiOver2;
                npc.SimpleFlyMovement(val13, num44);

                npc.ai[2] += 1;
                int eyeDelay = FargowiltasSouls.Core.Systems.WorldSavingSystem.MasochistModeReal ? 2 : 45; //funny old gigavomit crossmod bug on maso
                if (npc.ai[2] % eyeDelay == 0f)
                {
                    Vector2 val14 = Vector2.Normalize((Main.player[npc.target]).Center - (npc).Center) * 5f;
                    Vector2 val15 = (npc).Center + val14 * 10f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num45 = NPC.NewNPC((npc).GetSource_FromAI((string)null), (int)val15.X, (int)val15.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
                        (Main.npc[num45]).velocity.X = val14.X;
                        (Main.npc[num45]).velocity.Y = val14.Y;
                        if (Main.netMode == NetmodeID.Server && num45 < Main.maxNPCs)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, (NetworkText)null, num45, 0f, 0f, 0f, 0, 0, 0);
                        }
                        if (CalamityWorld.LegendaryMode)
                        {
                            int num46 = 814;
                            Vector2 val16 = val14 * 2f;
                            int num47 = 3;
                            float num48 = MathHelper.ToRadians(20f);
                            for (int n = 0; n < num47; n++)
                            {
                                Vector2 val17 = Utils.RotatedBy(val16, (double)MathHelper.Lerp(0f - num48, num48, (float)n / (float)(num47 - 1)), default(Vector2));
                                Projectile.NewProjectile((npc).GetSource_FromAI((string)null), (npc).Center + Vector2.Normalize(val17) * 10f, val17, num46, 15, 0f, Main.myPlayer, 0f, 0f, 0f);
                            }
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCDeath13, (Vector2?)(npc).position, (SoundUpdateCallback)null);
                    for (int num49 = 0; num49 < 10; num49++)
                    {
                        Dust.NewDust(val15, 20, 20, 5, val14.X * 0.4f, val14.Y * 0.4f, 0, default(Color), 1f);
                    }
                }
                float num50 = WindupTime;
                if (npc.ai[2] >= num50)
                {
                    npc.ai[1] = 16;
                    npc.ai[2] = 0;
                    npc.TargetClosest(true);
                    npc.SyncExtraAI();
                }
                npc.netUpdate = true;
                if (npc.netSpam > 10)
                {
                    npc.netSpam = 10;
                }
            }
            else if (npc.ai[1] == 16)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float num51 = 6f * (0.4f - lifeRatio);
                    float num52 = 18f + num51;
                    num52 += 10f * br;
                    (npc).velocity = npc.SafeDirectionTo((Main.player[npc.target]).Center) * num52;
                    npc.ai[1] = 17;
                    npc.netUpdate = true;
                    
                    if (npc.netSpam > 10)
                    {
                        npc.netSpam = 10;
                    }
                }
            }
            else if (npc.ai[1] == 17)
            {
                if (npc.ai[2] == 0)
                {
                    SoundEngine.PlaySound(SoundID.Roar,npc.position, null);
                }

                float dashTime = 40;
                if (npc.ai[2] % (dashTime / 2) == 0 && npc.ai[2] < dashTime)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        FargoSoulsUtil.XWay(8, npc.GetSource_FromThis(), npc.Center, ModContent.ProjectileType<BloodScythe>(), 1.5f, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
                npc.ai[2] += 1;
                if (npc.ai[2] == dashTime && Vector2.Distance((npc).position, (Main.player[npc.target]).position) < 200f)
                {
                    npc.ai[2] -= 1;
                }
                if (npc.ai[2] >= dashTime)
                {
                    (npc).velocity = (npc).velocity * 0.95f;
                    if ((npc).velocity.X > -0.1 && (double)(npc).velocity.X < 0.1)
                    {
                        (npc).velocity.X = 0f;
                    }
                    if ((npc).velocity.Y > -0.1 && (double)(npc).velocity.Y < 0.1)
                    {
                        (npc).velocity.Y = 0f;
                    }
                }
                else
                {
                    npc.rotation = Utils.ToRotation((npc).velocity) - (float)Math.PI / 2f;
                }
                float endTime = dashTime + 13f;
                if (npc.ai[2] >= endTime)
                {
                    npc.netUpdate = true;
                    if (npc.netSpam > 10)
                    {
                        npc.netSpam = 10;
                    }
                    npc.ai[2] = 0;
                    npc.ai[1] = 0;
                    HorizDash = false;
                }
            }
        }
        float Rotate(NPC npc, float rotation, Vector2 target, float speed)
        {
            Vector2 PV = npc.DirectionTo(target);
            Vector2 LV = rotation.ToRotationVector2();
            float anglediff = (float)(Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y)); //real
            //change rotation towards target
            return rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
        }
    }
}
