
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeGodCoreEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<SlimeGodCore>());


        #region Variables
        public static int CrimsonGodType => ModContent.NPCType<CrimulanPaladin>();
        public static int CorruptionGodType => ModContent.NPCType<EbonianPaladin>();
        public bool AttachToCrimson = WorldGen.crimson; //start off with the current world evil
        public int AttachedSlime;
        public bool ContactDamage = false;
        public enum Phases
        {
            ShouldAttach,
            TryAttach,
            Attached,
            Attacking
        }
        #endregion
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt(AttachedSlime);
            binaryWriter.Write(AttachToCrimson);
            binaryWriter.Write(ContactDamage);
            
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            AttachToCrimson = binaryReader.ReadBoolean();
            AttachedSlime = binaryReader.Read7BitEncodedInt();
            ContactDamage = binaryReader.ReadBoolean();
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => ContactDamage;

        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            #region Passive
            //FargoSoulsUtil.PrintAI(npc);
            CalamityGlobalNPC.slimeGod = npc.whoAmI;
            ref float timer = ref npc.ai[0];
            ref float phase = ref npc.ai[3];

            npc.dontTakeDamage = !(phase == (int)Phases.Attacking);

            if (phase != (int)Phases.Attached)
            {
                npc.Opacity = 1f;
                npc.rotation += npc.velocity.Length() * (MathHelper.Pi / 300f);
            }
            else
            {
                npc.Opacity = 0.3f;
            }

            if (!Targeting())
                return false;

            switch (phase)
            {
                case (int)Phases.ShouldAttach:
                    {
                        SummonSlimes();
                    }
                    break;
                case (int)Phases.TryAttach:
                    {
                        TryAttaching();
                    }
                    break;
                case (int)Phases.Attached:
                    {
                        Attached();
                    }
                    break;
                case (int)Phases.Attacking:
                    {

                    }
                    break;
            }
            timer++;
            return false;
            #endregion
            #region Common Methods
            bool Targeting()
            {
                const float despawnRange = 5000f;
                Player p = Main.player[npc.target];
                if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                {
                    npc.TargetClosest();
                    p = Main.player[npc.target];
                    if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                    {
                        npc.noTileCollide = true;
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;
                        npc.velocity.Y -= 1f;
                        if (npc.timeLeft == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
            #endregion
            #region Special Actions
            void SummonSlimes()
            {
                ref float timer = ref npc.ai[0];
                ref float phase = ref npc.ai[3];
                if (timer < 120)
                {
                    Vector2 desiredPos = Main.player[npc.target].Center - Vector2.UnitY * 300;
                    npc.velocity = (desiredPos - npc.Center) * 0.05f;
                }
                if (timer >= 120)
                {
                    if (DLCUtils.HostCheck)
                    {
                        const int spawnTossSpeed = 30;
                        if (!NPC.AnyNPCs(CrimsonGodType))
                        {
                            int crim = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, CrimsonGodType, Target: npc.target);
                            if (crim != Main.maxNPCs)
                            {
                                Main.npc[crim].velocity = Vector2.UnitX * -spawnTossSpeed;
                                if (AttachToCrimson)
                                {
                                    AttachedSlime = crim;
                                    phase = (int)Phases.TryAttach;
                                    AttachToCrimson = !AttachToCrimson;
                                    Reset();
                                    NetSync(npc);
                                    npc.netUpdate = true;
                                }
                            }
                        }
                        if (!NPC.AnyNPCs(CorruptionGodType))
                        {
                            int corr = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, CorruptionGodType, Target: npc.target);
                            if (corr != Main.maxNPCs)
                            {
                                Main.npc[corr].velocity = Vector2.UnitX * spawnTossSpeed;
                                if (!AttachToCrimson)
                                {
                                    AttachedSlime = corr;
                                    phase = (int)Phases.TryAttach;
                                    AttachToCrimson = !AttachToCrimson;
                                    Reset();

                                    NetSync(npc);
                                    npc.netUpdate = true;
                                }
                            }
                        }
                    }
                }
            }
            void TryAttaching()
            {
                ref float timer = ref npc.ai[0];
                ref float phase = ref npc.ai[3];
                NPC slime = Main.npc[AttachedSlime];
                ContactDamage = false;
                if (slime != null && slime.active)
                {
                    float modifier = timer / 60f;
                    npc.velocity = npc.DirectionTo(slime.Center) * 20f * modifier;
                    if (npc.Distance(slime.Center) <= npc.velocity.Length() * 2f)
                    {
                        phase = (int)Phases.Attached;
                        Reset();
                    }
                }
                else
                {
                    Reset();
                }
            }
            void Attached()
            {
                ref float timer = ref npc.ai[0];
                ref float phase = ref npc.ai[3];
                NPC slime = Main.npc[AttachedSlime];
                ContactDamage = false;
                if (slime != null && slime.active)
                {
                    npc.velocity = (slime.Center - npc.Center) + slime.velocity;
                }
                else
                {

                }
            }
            void SlimeBreakout()
            {
                ref float timer = ref npc.ai[0];
                ref float phase = ref npc.ai[3];
            }
            void Reset()
            {
                npc.ai[0] = 0;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
            }
            #endregion
            #region Attacks
            #endregion
        }
    }
}