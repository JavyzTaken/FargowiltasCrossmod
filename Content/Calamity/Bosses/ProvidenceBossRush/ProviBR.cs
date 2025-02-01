using CalamityMod.Events;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
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
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ProvidenceBossRush
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ProviBR : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<Providence>();
        }
        public int GuardiansSpawnCounter = 0;
        public int attackTimer = 0;
        public int attackType = 0;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(GuardiansSpawnCounter);
            binaryWriter.Write7BitEncodedInt(attackTimer);
            binaryWriter.Write7BitEncodedInt(attackType);
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            GuardiansSpawnCounter = binaryReader.Read7BitEncodedInt();
            attackTimer = binaryReader.Read7BitEncodedInt();
            attackType = binaryReader.Read7BitEncodedInt();
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        
        public override bool PreAI(NPC npc)
        {
            if (!BossRushEvent.BossRushActive)
                return base.PreAI(npc);

            if (npc.GetLifePercent() < 0.33f && GuardiansSpawnCounter < 2)
            {
                //int healer = ModContent.NPCType<ProfanedGuardianHealer>();
                //int defender = ModContent.NPCType<ProfanedGuardianDefender>();
                int commander = ModContent.NPCType<ProfanedGuardianCommander>();
                //Main.NewText(GuardiansSpawnCounter);
                npc.ai[0] = 2;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 1;
                if (GuardiansSpawnCounter == 1 && Main.netMode != NetmodeID.MultiplayerClient && !NPC.AnyNPCs(commander))
                {
                    GuardiansSpawnCounter = 2;
                    npc.ai[0] = 0;
                }
                if (GuardiansSpawnCounter == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, commander);
                    SoundEngine.PlaySound(Providence.SpawnSound);
                    GuardiansSpawnCounter = 1;
                }
                //Main.NewText(attackTimer);
                attackTimer++;
                if (attackTimer == 100)
                {
                    attackType = Main.rand.NextBool() ? 1 : 0;
                }
                //attackType = 1;
                if (attackTimer >= 100 && attackType == 0)
                {
                    attackTimer = 0;
                    int amount = 10;
                    for (int i = 0; i < amount; i++)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(3, 0).RotatedBy(MathHelper.ToRadians(360 * (i / (float)amount))), ModContent.ProjectileType<HolySpear>(), 200, 0, ai0:0);
                    }
                }
                if (attackTimer >= 100 && attackType == 1 && npc.HasValidTarget)
                {
                    if (attackTimer % 2 == 0)
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, new Vector2(3, 0).RotatedBy(npc.AngleTo(Main.player[npc.target].Center) + MathHelper.ToRadians(attackTimer - 110) * 5), ModContent.ProjectileType<HolyBurnOrb>(), 200, 0, ai0: 1);
                    }
                    if (attackTimer >= 120)
                    {
                        attackTimer = 0;
                    }
                }
                
                
            }
            return base.PreAI(npc);
        }
    }
}
