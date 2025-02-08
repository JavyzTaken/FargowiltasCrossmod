using CalamityMod.NPCs.OldDuke;
using FargowiltasCrossmod.Assets.Particles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
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

        // TODO -- Sync.
        /// <summary>
        /// An extra AI value for this sharkron.
        /// </summary>
        public float ExtraAI
        {
            get;
            set;
        }

        public override int NPCOverrideID => ModContent.NPCType<SulphurousSharkron>();

        public override bool PreAI()
        {
            int oldDukeIndex = NPC.FindFirstNPC(ModContent.NPCType<CalamityOD>());
            if (oldDukeIndex == -1)
            {
                Die();
                return false;
            }
            if (!Main.npc[oldDukeIndex].TryGetDLCBehavior(out OldDukeEternity oldDuke))
            {
                Die();
                return false;
            }

            NPC.Opacity = LumUtils.Saturate(NPC.Opacity + 0.1f);
            NPC.dontTakeDamage = false;
            oldDuke.SharkronPuppeteerAction?.Invoke(this);
            Time++;

            return false;
        }

        /// <summary>
        /// Makes this sharkron die and violently explode, all lowtiergod-like.
        /// </summary>
        public void Die()
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);

            for (int i = 0; i < 24; i++)
            {
                Vector2 vomitVelocity = Main.rand.NextVector2Circular(9f, 15f);
                ModContent.GetInstance<BileMetaball>().CreateParticle(NPC.Center, vomitVelocity, Main.rand.NextFloat(32f, 44f), Main.rand.NextFloat());
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 goreDirection = NPC.velocity.SafeNormalize(Vector2.Zero);
                goreDirection.Y = -MathF.Abs(goreDirection.Y);
                goreDirection = Vector2.Lerp(goreDirection, -Vector2.UnitY, 0.75f);

                for (int i = 0; i < 5; i++)
                    LumUtils.NewProjectileBetter(NPC.GetSource_Death(), NPC.Center, goreDirection * Main.rand.NextFloat(16f, 40f) * new Vector2(Main.rand.NextFloat(1f, 1.7f), 1f), ModContent.ProjectileType<FallingVomitGore>(), 270, 0f);
            }

            NPC.active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            return true;
        }
    }
}
