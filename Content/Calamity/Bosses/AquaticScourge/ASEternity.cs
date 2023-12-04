using CalamityMod.Events;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using CalamityMod;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ASEternity : EModeCalBehaviour
    {
        public static bool Enabled = false;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<AquaticScourgeHead>());

        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            entity.lifeMax = (int)Math.Round(entity.lifeMax * 1.15f);
        }

    }
    public class ASBodyEternity : EModeCalBehaviour
    {
        public override bool IsLoadingEnabled(Mod mod) => ASEternity.Enabled;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<AquaticScourgeBody>());

        public bool SpikeAttackSprite = false;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!SpikeAttackSprite)
                return true;

            if (npc.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/AquaticScourge/SpikelessASBody").Value;
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);

            Vector2 vector43 = npc.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            Color color = npc.GetAlpha(drawColor);

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive || Main.zenithWorld)
            {
                if (Main.npc[(int)npc.ai[2]].Calamity().newAI[3] > 300f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp((Main.npc[(int)npc.ai[2]].Calamity().newAI[3] - 300f) / 180f, 0f, 1f));
                else if (Main.npc[(int)npc.ai[2]].localAI[3] > 0f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp(Main.npc[(int)npc.ai[2]].localAI[3] / 90f, 0f, 1f));
            }

            spriteBatch.Draw(texture2D15, vector43, npc.frame, color, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }
    }
}
