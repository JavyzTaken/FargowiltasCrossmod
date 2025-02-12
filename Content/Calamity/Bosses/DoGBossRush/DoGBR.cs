using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ProvidenceBossRush
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DoGBR : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<CalamityMod.NPCs.DevourerofGods.DevourerofGodsHead>();
        }
        public int SentinelSpawnCounter;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(SentinelSpawnCounter);
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            SentinelSpawnCounter = binaryReader.Read7BitEncodedInt();
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        
        public override bool PreAI(NPC npc)
        {
            if (!BossRushEvent.BossRushActive)
                return base.PreAI(npc);

            CalamityGlobalNPC calnpc = npc.Calamity();
            DevourerofGodsHead dog = (DevourerofGodsHead)npc.ModNPC;
            //Main.NewText("ai: " + npc.ai[0] + ", " + npc.ai[1] + ", " + npc.ai[2] + ", " + npc.ai[3]);
            //Main.NewText("calai: " + calnpc.newAI[0] + ", " + calnpc.newAI[1] + ", " + calnpc.newAI[2] + ", " + calnpc.newAI[3]);
            
            FieldInfo laserwallphase = typeof(DevourerofGodsHead).GetField("laserWallPhase", LumUtils.UniversalBindingFlags);
            FieldInfo alphagatevalue = typeof(DevourerofGodsHead).GetField("alphaGateValue", LumUtils.UniversalBindingFlags);
            //Main.NewText(laserwallphase.GetValue(dog));
            //Main.NewText(alphagatevalue.GetValue(dog));
            if (SentinelSpawnCounter < 4 && npc.GetLifePercent() < 0.6f)
            {
                DontDoLaserWalls(npc);
            }
            if (npc.GetLifePercent() <= 0.5f && SentinelSpawnCounter == 0)
            {
                NPC.SpawnOnPlayer(npc.target, ModContent.NPCType<StormWeaverHead>());
                SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound);
                SentinelSpawnCounter = 1;
            }
            if (npc.GetLifePercent() <= 0.4f && SentinelSpawnCounter == 1)
            {
                NPC.SpawnOnPlayer(npc.target, ModContent.NPCType<CeaselessVoid>());
                SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound);
                SentinelSpawnCounter = 2;
            }
            if (npc.GetLifePercent() <= 0.3f && SentinelSpawnCounter == 2)
            {
                NPC.SpawnOnPlayer(npc.target, ModContent.NPCType<Signus>());
                SoundEngine.PlaySound(DevourerofGodsHead.SpawnSound);
                SentinelSpawnCounter = 3;
            }
            if (SentinelSpawnCounter == 3 && !NPC.AnyNPCs(ModContent.NPCType<Signus>()))
            {
                SentinelSpawnCounter = 4;
            }
            if (NPC.AnyNPCs(ModContent.NPCType<Signus>()) || NPC.AnyNPCs(ModContent.NPCType<CeaselessVoid>()) || NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()))
            {

                //ForceDocileState(npc);
            }
            //ForceDocileState(npc);
            
            return base.PreAI(npc);
        }
        public override void PostAI(NPC npc)
        {
            if (!BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC calnpc = npc.Calamity();
            DevourerofGodsHead dog = (DevourerofGodsHead)npc.ModNPC;
            FieldInfo alphagatevalue = typeof(DevourerofGodsHead).GetField("alphaGateValue", LumUtils.UniversalBindingFlags);
            if (NPC.AnyNPCs(ModContent.NPCType<Signus>()) || NPC.AnyNPCs(ModContent.NPCType<CeaselessVoid>()) || NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()))
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i] != null && Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<DevourerofGodsBody>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsTail>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsHead>()))
                    {
                        Main.npc[i].dontTakeDamage = true;
                        Main.npc[i].Opacity = 0;

                    }
                }
                //FieldInfo posttptimer = typeof(DevourerofGodsHead).GetField("postTeleportTimer", LumUtils.UniversalBindingFlags);
                //posttptimer.SetValue(dog, -100);
                FieldInfo idlecountdown = typeof(DevourerofGodsHead).GetField("idleCounter", LumUtils.UniversalBindingFlags);
                //idlecountdown.SetValue(dog, 1000);
                //calnpc.newAI[1] = 10;
            }
            else if (calnpc.newAI[3] < (float)alphagatevalue.GetValue(dog) && npc.GetLifePercent() > 0.2f && npc.GetLifePercent() <= 0.55f)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i] != null && Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<DevourerofGodsBody>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsTail>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsHead>()))
                    {
                        Main.npc[i].dontTakeDamage = false;
                        if (Main.npc[i].Opacity < 1)
                        Main.npc[i].Opacity += 0.01f;

                    }
                }
            }
            FieldInfo posttptimer = typeof(DevourerofGodsHead).GetField("postTeleportTimer", LumUtils.UniversalBindingFlags);
            //Main.NewText(posttptimer.GetValue(dog));
            base.PostAI(npc);
        }
        public void DontDoLaserWalls(NPC npc)
        {
            CalamityGlobalNPC calnpc = npc.Calamity();
            calnpc.newAI[3] = 100;
        }
        public void ForceDocileState(NPC npc)
        {
            
            CalamityGlobalNPC calnpc = npc.Calamity();
            DevourerofGodsHead dog = (DevourerofGodsHead)npc.ModNPC;
            FieldInfo laserwallphase = typeof(DevourerofGodsHead).GetField("laserWallPhase", LumUtils.UniversalBindingFlags);
            if (calnpc.newAI[3] == 719)
            {
                laserwallphase.SetValue(dog, 0);
            }
            
            
            
            //required to be constantly set to not end laser wall phase
            FieldInfo idlecountdown = typeof(DevourerofGodsHead).GetField("idleCounter", LumUtils.UniversalBindingFlags);
            idlecountdown.SetValue(dog, 1000);
            //required to be 0 for laser wall phase code
            FieldInfo posttptimer = typeof(DevourerofGodsHead).GetField("postTeleportTimer", LumUtils.UniversalBindingFlags);
            posttptimer.SetValue(dog, 0);
            //controls opacity. alpha gate is ~700, this must be significantly higher than it
            calnpc.newAI[3] = 718;
            //required to be constantly set to not shoot laser walls
            calnpc.newAI[1] = 10;
        }
    }
}
