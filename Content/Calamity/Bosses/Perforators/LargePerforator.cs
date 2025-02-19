using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class LargePerforator : CalDLCEmodeExtraGlobalNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<PerforatorHeadLarge>(),
            ModContent.NPCType<PerforatorBodyLarge>(),
            ModContent.NPCType<PerforatorTailLarge>()
        );
        public Vector2 VelocityReal = Vector2.UnitY * 16;
        public int Timer = 0;
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            entity.Opacity = 1f;
            entity.CalamityDLC().ImmuneToAllDebuffs = true;
        }
        public override void SpawnNPC(int npc, int tileX, int tileY)
        {
            base.SpawnNPC(npc, tileX, tileY);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == ModContent.NPCType<PerforatorBodyLarge>())
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (npc.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Texture2D texture2D15 = npc.localAI[3] == 1f ? PerforatorBodyLarge.AltTexture.Value : TextureAssets.Npc[npc.type].Value;
                Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[npc.type].Value.Width / 2), (float)(TextureAssets.Npc[npc.type].Value.Height / 2));

                Vector2 drawLocation = npc.Center - screenPos;
                drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
                drawLocation += halfSizeTexture * npc.scale + new Vector2(0f, npc.gfxOffY);
                spriteBatch.Draw(texture2D15, drawLocation, npc.frame, npc.GetAlpha(drawColor), npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);

                texture2D15 = npc.localAI[3] == 1f ? PerforatorBodyLarge.AltTexture_Glow.Value : PerforatorBodyLarge.Texture_Glow.Value;
                Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f) * npc.Opacity;

                spriteBatch.Draw(texture2D15, drawLocation, npc.frame, glowmaskColor, npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);
                return false;
            }
            else if (npc.type == ModContent.NPCType<PerforatorTailLarge>())
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (npc.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Texture2D texture2D15 = TextureAssets.Npc[npc.type].Value;
                Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[npc.type].Value.Width / 2), (float)(TextureAssets.Npc[npc.type].Value.Height / 2));

                Vector2 drawLocation = npc.Center - screenPos;
                drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
                drawLocation += halfSizeTexture * npc.scale + new Vector2(0f, npc.gfxOffY);
                spriteBatch.Draw(texture2D15, drawLocation, npc.frame, npc.GetAlpha(drawColor), npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);

                texture2D15 = PerforatorTailLarge.GlowTexture.Value;
                Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);

                spriteBatch.Draw(texture2D15, drawLocation, npc.frame, glowmaskColor, npc.rotation, halfSizeTexture, npc.scale, spriteEffects, 0f);
                return false;
            }
            return true;
        }
        public override bool SafePreAI(NPC npc)
        {
            npc.netUpdate = true; //fuck you worm mp code
            
            if (npc.type == ModContent.NPCType<PerforatorHeadLarge>())
            {
                npc.velocity = VelocityReal;
            }
            else
            {
                int hiveID = NPC.FindFirstNPC(ModContent.NPCType<PerforatorHive>());
                if (hiveID.IsWithinBounds(Main.maxNPCs) && (Timer == 0 || npc.Distance(Main.npc[hiveID].Center) < 80))
                {
                    npc.Center = Main.npc[hiveID].Center;
                    npc.Opacity = 0f;
                }
                else
                    npc.Opacity = 1f;
            }
            Timer++;

            return true;
        }
        public override void SafePostAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<PerforatorHeadLarge>())
            {
                if (Timer > 10)
                {
                    if (npc.HasPlayerTarget && npc.Center.Y > Main.player[npc.target].Center.Y)
                        VelocityReal.Y -= 1.2f;
                    else
                        VelocityReal.Y += 1.2f;
                    VelocityReal.Y = MathHelper.Clamp(VelocityReal.Y, -30, 30);
                }

                VelocityReal.X += Math.Sign(VelocityReal.X) * 0.1f;
                if (Timer > 60 * 7)
                    npc.StrikeInstantKill();
                //npc.velocity = VelocityReal;
            }
        }
        /*
        public void ManageWormStuff(NPC NPC)
        {
            NPC.realLife = -1;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    int totalSegments = 10;
                    NPC.ai[2] = totalSegments;
                    NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), ModContent.NPCType<PerforatorBodyMedium>(), NPC.whoAmI);
                    Main.npc[(int)NPC.ai[0]].ai[1] = NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                // Splitting effect
                if (!Main.npc[(int)NPC.ai[1]].active && !Main.npc[(int)NPC.ai[0]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
                if (!Main.npc[(int)NPC.ai[0]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }

                if (!NPC.active && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            }

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
        }
        */
    }
}
