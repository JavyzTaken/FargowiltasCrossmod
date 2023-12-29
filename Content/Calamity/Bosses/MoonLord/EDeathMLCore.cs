
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
/*
using Terraria.ModLoader;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Effects;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using CalamityMod;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls.Content.Projectiles.Masomode;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.MoonLord
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathMLCore : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.MoonLordCore);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
            binaryWriter.Write7BitEncodedInt(timer);
            binaryWriter.Write(roguePhase);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
            timer = binaryReader.Read7BitEncodedInt();
            roguePhase = binaryReader.ReadBoolean();
        }
        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            if (item.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 0 && !player.buffImmune[ModContent.BuffType<NullificationCurseBuff>()]&& !WorldSavingSystem.SwarmActive)
                return false;
            if (!item.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && roguePhase && npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4 && !player.buffImmune[ModContent.BuffType<NullificationCurseBuff>()] && !WorldSavingSystem.SwarmActive)
                return false;

            return base.CanBeHitByItem(npc, player, item);
        }

        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            if (projectile.owner < 0) return base.CanBeHitByProjectile(npc, projectile);

            if (projectile.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && !Main.player[projectile.owner].buffImmune[ModContent.BuffType<NullificationCurseBuff>()] && !WorldSavingSystem.SwarmActive)
            {
                if (npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4) return true;
                else return false;
            }
            if (!projectile.CountsAsClass(ModContent.GetInstance<RogueDamageClass>()) && roguePhase && npc.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4 && !Main.player[projectile.owner].buffImmune[ModContent.BuffType<NullificationCurseBuff>()] && !WorldSavingSystem.SwarmActive)
                return false;

            return base.CanBeHitByProjectile(npc, projectile);
        }
        public int timer = 0;
        public bool roguePhase = false;
        public override bool SafePreAI(NPC npc)
        {
            bool result = base.SafePreAI(npc);
            //if (!npc.HasValidTarget) return true;
            MoonLordCore ml = npc.GetGlobalNPC<MoonLordCore>();
            
            //Main.NewText(ml.VulnerabilityState);
            //ml.VulnerabilityState = 4;
            //Main.NewText(ml.VulnerabilityTimer);
            if (ml.VulnerabilityState == 3) roguePhase = true;
            ml.RunEmodeAI = true;
            if (roguePhase && ml.VulnerabilityState == 4)
            {
                ml.RunEmodeAI = false;

                //default stuff from emode AI
                EModeGlobalNPC.moonBoss = npc.whoAmI;
                if (WorldSavingSystem.SwarmActive)
                {
                    return result;
                }

                if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && ml.VulnerabilityState >= 0 && ml.VulnerabilityState <= 4)
                {
                    Main.LocalPlayer.AddBuff(ModContent.BuffType<NullificationCurseBuff>(), 2);
                }

                npc.position -= npc.velocity * 2f / 3f;

                
                Filters.Scene.Activate("FargowiltasSouls:Vortex");
                Filters.Scene.Activate("FargowiltasSouls:Nebula");
                Filters.Scene.Activate("FargowiltasSouls:Solar");
                Filters.Scene.Activate("FargowiltasSouls:Stardust");

                ml.VulnerabilityTimer++;
                
                if (ml.VulnerabilityTimer >= 1800)
                {
                    roguePhase = false;
                    ml.RunEmodeAI = true;
                    ml.VulnerabilityTimer = 0;
                    npc.netUpdate = true;
                    return result;
                }
                
                bool gophase2 = true;
                foreach (NPC n in Main.npc)
                {
                    if (n != null && n.active && (n.type == NPCID.MoonLordHand || npc.type == NPCID.MoonLordHead) && n.ai[3] == npc.whoAmI && n.ai[0] != -2)
                    {
                        gophase2 = false;
                    }
                }
                if (gophase2)
                {
                    npc.ai[0] = 1;
                    ml.EnteredPhase2 = true;
                }
                if (ml.EnteredPhase2)
                {
                    timer++;
                    if (timer == 300)
                    {
                        timer = 0;
                        if (DLCUtils.HostCheck)
                        {
                            float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                            for (int i = 0; i < 5; i++)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BigLight>(), FargoSoulsUtil.ScaledProjectileDamage(200), 0, ai1: npc.whoAmI, ai2: MathHelper.ToRadians((360f / 5) * i) + angle);
                            }
                        }
                    }
                }
                if (npc.ai[0] == 2f) //moon lord is dead
                {
                    ml.VulnerabilityState = 4;
                    ml.VulnerabilityTimer = 0;
                    ml.AttackTimer = 0;
                }
            }
            return result;
        }
    }
}
*/