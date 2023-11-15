using System.Collections.Generic;
using CalamityMod.NPCs.Perforator;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SmallPerforator : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
        ModContent.NPCType<PerforatorHeadSmall>(),
        ModContent.NPCType<PerforatorBodySmall>(),
        ModContent.NPCType<PerforatorTailSmall>()
        );
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override void SpawnNPC(int npc, int tileX, int tileY)
        {
            base.SpawnNPC(npc, tileX, tileY);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            npc.netUpdate = true; //fuck you worm mp code
            if (npc.type == ModContent.NPCType<PerforatorBodySmall>() || npc.type == ModContent.NPCType<PerforatorTailSmall>())
            {
                NPC owner = null;
                if (npc.realLife >= 0)
                {
                    owner = Main.npc[npc.realLife];

                }
                if (owner != null && owner.active && owner.ai[2] == 1)
                {
                    npc.velocity = (owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * 15;
                    if (npc.type == ModContent.NPCType<PerforatorTailSmall>() && npc.Distance(owner.Center) <= 20)
                    {
                        foreach (NPC n in Main.npc)
                        {
                            if (n != null && n.active && new List<int>() { ModContent.NPCType<PerforatorHeadSmall>(), ModContent.NPCType<PerforatorBodySmall>(), ModContent.NPCType<PerforatorTailSmall>() }
                            .Contains(n.type) && (n.realLife == owner.whoAmI || n.whoAmI == owner.whoAmI))
                            {
                                n.active = false;
                            }
                        }
                        owner.active = false;
                    }
                    return false;
                }
                if (owner != null && owner.active && owner.ai[3] < 60)
                {
                    NPC perf = null;
                    foreach (NPC n in Main.npc)
                    {
                        if (n != null && n.active && n.type == ModContent.NPCType<PerforatorHive>())
                        {
                            perf = n;
                        }
                    }
                    npc.TargetClosest();
                    NetSync(npc);
                    if (perf != null && npc.target >= 0)
                    {
                        Player target = Main.player[npc.target];
                        npc.Center = perf.Center + (target.Center - perf.Center).SafeNormalize(Vector2.Zero) * 40;
                        return false;
                    }
                }

            }
            if (npc.type == ModContent.NPCType<PerforatorHeadSmall>())
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<PerforatorHive>()))
                {
                    npc.active = false;
                    return false;
                }
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                if (npc.ai[3] >= 10)
                {
                    if (npc.ai[3] < 60)
                    {
                        NPC perf = null;
                        foreach (NPC n in Main.npc)
                        {
                            if (n != null && n.active && n.type == ModContent.NPCType<PerforatorHive>())
                            {
                                perf = n;
                            }
                        }
                        Player target = null;
                        npc.TargetClosest();
                        if (npc.target >= 0)
                        {
                            target = Main.player[npc.target];
                        }
                        if (perf != null && target != null)
                        {
                            npc.Center = perf.Center + (target.Center - perf.Center).SafeNormalize(Vector2.Zero) * 80;
                            npc.rotation = npc.AngleTo(target.Center) + MathHelper.PiOver2;
                        }
                    }
                    if (npc.ai[3] == 60)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath23, npc.Center);
                        npc.TargetClosest();
                        if (npc.target >= 0)
                        {
                            Player target = Main.player[npc.target];
                            npc.velocity = (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 20;

                        }
                        NetSync(npc);
                    }
                    if (npc.ai[3] >= 120 && npc.ai[2] == 0)
                    {
                        NPC perf = null;
                        foreach (NPC n in Main.npc)
                        {
                            if (n != null && n.active && n.type == ModContent.NPCType<PerforatorHive>())
                            {
                                perf = n;
                            }
                        }
                        if (perf != null && perf.active && perf.type == ModContent.NPCType<PerforatorHive>())
                        {
                            float vel = 20; // 16 + perf.velocity.Length();
                            npc.velocity = Vector2.Lerp(npc.velocity, (perf.Center - npc.Center).SafeNormalize(Vector2.Zero) * vel, 0.1f);
                            if (npc.Distance(perf.Center) <= 20)
                            {
                                npc.ai[2] = 1;
                            }
                        }

                    }
                    if (npc.ai[2] == 1)
                    {
                        NPC perf = null;
                        foreach (NPC n in Main.npc)
                        {
                            if (n != null && n.active && n.type == ModContent.NPCType<PerforatorHive>())
                            {
                                perf = n;
                            }
                        }
                        if (perf != null && perf.active && perf.type == ModContent.NPCType<PerforatorHive>())
                        {
                            float vel = 16 + perf.velocity.Length();
                            npc.velocity = Vector2.Lerp(npc.velocity, (perf.Center - npc.Center).SafeNormalize(Vector2.Zero) * vel, 0.03f);
                            if (npc.Distance(perf.Center) <= 20)
                            {
                                npc.ai[2] = 1;
                                NetSync(npc);
                            }
                        }
                        npc.Center = perf.Center;
                    }
                    npc.ai[3]++;
                    return false;
                }
                else
                {
                    //???
                }
                npc.ai[3]++;

            }
            return true;
        }
    }
}
