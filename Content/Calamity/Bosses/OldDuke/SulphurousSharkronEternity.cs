using CalamityMod.Dusts;
using CalamityMod.NPCs.OldDuke;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityOD = CalamityMod.NPCs.OldDuke.OldDuke;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.OldDuke
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SulphurousSharkronEternity : CalDLCEmodeBehavior
    {
        /// <summary>
        /// How long this sharkron has existed for.
        /// </summary>
        public ref float Time => ref NPC.ai[0];

        /// <summary>
        /// The opacity of this sharkron's afterimages.
        /// </summary>
        public ref float AfterimageOpacity => ref NPC.localAI[0];

        public override int NPCOverrideID => ModContent.NPCType<SulphurousSharkron>();

        public override bool PreAI()
        {
            int oldDukeIndex = NPC.FindFirstNPC(ModContent.NPCType<CalamityOD>());
            if (oldDukeIndex == -1)
            {
                Die(false);
                return false;
            }
            if (!Main.npc[oldDukeIndex].TryGetDLCBehavior(out OldDukeEternity oldDuke))
            {
                Die(false);
                return false;
            }

            NPC.Opacity = LumUtils.Saturate(NPC.Opacity + 0.1f);
            NPC.dontTakeDamage = false;
            oldDuke.SharkronPuppeteerAction?.Invoke(this);
            Time++;

            AfterimageOpacity = MathHelper.Lerp(AfterimageOpacity, 1f, 0.02f);

            return false;
        }

        /// <summary>
        /// Makes this sharkron die and violently explode, all lowtiergod-like.
        /// </summary>
        public void Die(bool createGore)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);

            for (int i = 0; i < 24; i++)
            {
                Vector2 vomitVelocity = Main.rand.NextVector2Circular(9f, 15f);
                ModContent.GetInstance<BileMetaball>().CreateParticle(NPC.Center, vomitVelocity, Main.rand.NextFloat(32f, 44f), Main.rand.NextFloat());
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && createGore)
            {
                Vector2 goreDirection = NPC.velocity.SafeNormalize(Vector2.Zero);
                goreDirection.Y = -MathF.Abs(goreDirection.Y);
                goreDirection = Vector2.Lerp(goreDirection, -Vector2.UnitY, 0.4f);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 goreVelocity = goreDirection * Main.rand.NextFloat(16f, 40f);
                    goreVelocity.X *= 1.2f;
                    LumUtils.NewProjectileBetter(NPC.GetSource_Death(), NPC.Center, goreVelocity * new Vector2(Main.rand.NextFloat(0.8f, 2f), 1f), ModContent.ProjectileType<FallingVomitGore>(), 270, 0f);
                }
            }

            for (int i = 0; i < 15; i++)
            {
                Dust toxicDust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulphurousSeaAcid, 0f, 0f, 100, default, 2f);
                toxicDust.velocity *= new Vector2(3f, 6f);

                if (Main.rand.NextBool())
                {
                    toxicDust.scale = 0.5f;
                    toxicDust.fadeIn = Main.rand.NextFloat(1f, 2f);
                }
            }

            if (Main.netMode != NetmodeID.Server && createGore)
            {
                Mod calamity = ModContent.GetInstance<CalamityMod.CalamityMod>();
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 0.2f, calamity.Find<ModGore>("SulphurousSharkronGore").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * 0.2f, calamity.Find<ModGore>("SulphurousSharkronGore2").Type, NPC.scale);
            }

            NPC.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            float rotation = NPC.rotation;
            SpriteEffects direction = SpriteEffects.None;

            for (int i = 8; i >= 0; i--)
            {
                float afterimageOpacity = (1f - i / 9f).Squared() * AfterimageOpacity;
                Vector2 afterimageDrawPosition = NPC.oldPos[i] + NPC.Size * 0.5f - Main.screenPosition;
                Main.EntitySpriteDraw(texture, afterimageDrawPosition, NPC.frame, NPC.GetAlpha(lightColor) * afterimageOpacity, rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction);
            }

            Vector2 drawPosition = NPC.Center - screenPos;
            Main.EntitySpriteDraw(texture, drawPosition, NPC.frame, NPC.GetAlpha(lightColor), rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction);
            return false;
        }
    }
}
