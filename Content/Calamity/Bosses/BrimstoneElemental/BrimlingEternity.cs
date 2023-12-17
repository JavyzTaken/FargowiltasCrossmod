using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimlingEternity : EModeCalBehaviour
    {
        public const bool Enabled = true;
        public override bool IsLoadingEnabled(Mod mod) => BrimstoneEternity.Enabled;

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.Brimling>());
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> fire = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneHellfireball");
            if (npc.ai[1] == 1)
            {
                int frame = (int)MathHelper.Lerp(0, 5, npc.ai[2] / 50f);
                Main.NewText(frame);
                spriteBatch.Draw(fire.Value, npc.Center - screenPos, new Rectangle(0, fire.Height() / 6 * frame, fire.Width(), fire.Height() / 6), drawColor * (npc.velocity.Length() / 10), npc.velocity.ToRotation() - MathHelper.PiOver2, new Vector2(fire.Width() / 2, fire.Height() / 12 + 20), 2, SpriteEffects.None, 0);
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);

        }
        public override bool SafePreAI(NPC npc)
        {
            npc.TargetClosest();
            if (!npc.HasValidTarget || !DLCCalamityConfig.Instance.EternityPriorityOverRev || !DLCWorldSavingSystem.EternityRev)
                return false;
            
            Player target = Main.player[npc.target];
            NPC owner = Main.npc[(int)npc.ai[0]];
            npc.spriteDirection = npc.Center.X > target.Center.X ? -1 : 1;
            if (owner == null || !owner.active)
            {
                npc.StrikeInstantKill();
                return false;
            }
            if (npc.ai[1] == 0)
            {
                Vector2 targetP = owner.Center + new Vector2(0, 200).RotatedBy(npc.ai[2]);
                npc.velocity = Vector2.Lerp(npc.velocity, (targetP - npc.Center).SafeNormalize(Vector2.Zero) * npc.Distance(targetP) / 30f, 0.03f);
                npc.ai[2] += MathHelper.ToRadians(Main.rand.NextFloat(1, 1.5f));
            }
            if (npc.ai[1] == 1)
            {
                npc.ai[2]++;
                if (npc.ai[2] == 200)
                {
                    npc.ai[2] = 0;
                    npc.velocity = (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 10;
                }
                npc.velocity *= 0.98f;
            }
            
            return false;
        }
        public override bool PreKill(NPC npc)
        {
            return base.PreKill(npc);
        }
    }
}
