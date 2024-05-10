using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
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
using Terraria.Audio;
using Terraria.ID;
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
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> fire = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneHellfireball");
            if (npc.ai[1] == 1)
            {
                int fard = (int)npc.ai[2];
                while (fard > 50)
                {
                    fard -= 50;
                }
                int frame = (int)MathHelper.Lerp(0, 5, fard / 50f);
                //Main.NewText(frame);
                spriteBatch.Draw(fire.Value, npc.Center - screenPos, new Rectangle(0, fire.Height() / 6 * frame, fire.Width(), fire.Height() / 6), drawColor * (npc.velocity.Length() / 15), npc.velocity.ToRotation() - MathHelper.PiOver2, new Vector2(fire.Width() / 2, fire.Height() / 12 + 20), 2, SpriteEffects.None, 0);
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);

        }
        public override bool SafePreAI(NPC npc)
        {
            npc.TargetClosest();
            if (!npc.HasValidTarget || !DLCCalamityConfig.Instance.EternityPriorityOverRev || !CalDLCWorldSavingSystem.EternityRev)
                return false;
            
            Player target = Main.player[npc.target];

            
            NPC owner = Main.npc[(int)npc.ai[0]];
            npc.spriteDirection = npc.Center.X > target.Center.X ? -1 : 1;
            if (owner.ai[0] == 0) npc.Opacity -= 0.01f;
            if (npc.Opacity <= 0)
            {
                npc.active = false;
            }
            if (owner == null || !owner.active)
            {
                npc.StrikeInstantKill();
                return false;
            }

            
            if (npc.ai[1] < 0)
            {
                npc.dontTakeDamage = true;
                npc.rotation += 0.15f;
                float speed = 10 + npc.ai[2];
                npc.ai[2] += 0.1f;
                npc.velocity = Vector2.Lerp(npc.velocity, (owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * speed, 0.08f);
                if (npc.Distance(owner.Center) <= 20 && DLCUtils.HostCheck)
                {
                    Projectile.NewProjectileDirect(npc.GetSource_Death(), npc.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), 0, 0, ai1:2);
                    owner.SimpleStrikeNPC(300, 1, true);
                    npc.StrikeInstantKill();
                    NetSync(npc);
                }
                return false;
            }

            if (npc.ai[1] == 0)
            {
                Vector2 targetP = owner.Center + new Vector2(0, 50 * npc.ai[3]).RotatedBy(MathHelper.ToRadians((int)npc.ai[2]));
                npc.velocity = (targetP - npc.Center).SafeNormalize(Vector2.Zero) * npc.Distance(targetP) / 30f;
                npc.ai[2] += 0.01f;
                if (npc.ai[2] - (int)npc.ai[2] >= 0.6f && DLCUtils.HostCheck)
                {
                    npc.ai[2] = Main.rand.Next(0, 360);
                    npc.ai[3] = Main.rand.NextFloat(3, 4f);
                    if (Main.rand.NextBool(3))
                    {
                        Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 4.5f, ModContent.ProjectileType<BrimstoneBarrage>(), npc.damage, 0);
                    }
                    NetSync(npc);
                }
                
            }
            if (npc.ai[1] == 1)
            {
                npc.ai[2]++;
                if (npc.ai[2] == 200)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, npc.Center);
                    npc.ai[2] = 0;
                    npc.velocity = (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 15;
                }
                
                npc.velocity *= 0.98f;
            }
            
            return false;
        }
        public override bool PreKill(NPC npc)
        {
            
            return base.PreKill(npc);
        }
        public override void OnKill(NPC npc)
        {
            if (npc.ai[1] >= 0)
            {
                NPC.NewNPC(npc.GetSource_Death(), (int)npc.Center.X + 20, (int)npc.Center.Y + 20, ModContent.NPCType<Brimling>(), 0, npc.ai[0], -1);
            }
            base.OnKill(npc);
        }
    }
}
