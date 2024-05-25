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
    public class BrimlingEternity : CalDLCEmodeBehavior
    {
        public const bool Enabled = true;
        public override bool IsLoadingEnabled(Mod mod) => BrimstoneEternity.Enabled;
        public override int NPCOverrideID => ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.Brimling>();
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> fire = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneHellfireball");
            if (NPC.ai[1] == 1)
            {
                int fard = (int)NPC.ai[2];
                while (fard > 50)
                {
                    fard -= 50;
                }
                int frame = (int)MathHelper.Lerp(0, 5, fard / 50f);
                //Main.NewText(frame);
                spriteBatch.Draw(fire.Value, NPC.Center - screenPos, new Rectangle(0, fire.Height() / 6 * frame, fire.Width(), fire.Height() / 6), drawColor * (NPC.velocity.Length() / 15), NPC.velocity.ToRotation() - MathHelper.PiOver2, new Vector2(fire.Width() / 2, fire.Height() / 12 + 20), 2, SpriteEffects.None, 0);
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);

        }
        public override bool PreAI()
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget || !CalDLCConfig.Instance.EternityPriorityOverRev || !CalDLCWorldSavingSystem.EternityRev)
                return false;
            
            Player target = Main.player[NPC.target];

            
            NPC owner = Main.npc[(int)NPC.ai[0]];
            NPC.spriteDirection = NPC.Center.X > target.Center.X ? -1 : 1;
            if (owner.ai[0] == 0) NPC.Opacity -= 0.01f;
            if (NPC.Opacity <= 0)
            {
                NPC.active = false;
            }
            if (owner == null || !owner.active)
            {
                NPC.StrikeInstantKill();
                return false;
            }

            
            if (NPC.ai[1] < 0)
            {
                NPC.dontTakeDamage = true;
                NPC.rotation += 0.15f;
                float speed = 10 + NPC.ai[2];
                NPC.ai[2] += 0.1f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, (owner.Center - NPC.Center).SafeNormalize(Vector2.Zero) * speed, 0.08f);
                if (NPC.Distance(owner.Center) <= 20 && DLCUtils.HostCheck)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BrimstonePulse>(), 0, 0, ai1:2);
                    owner.SimpleStrikeNPC(300, 1, true);
                    NPC.StrikeInstantKill();
                    NetSync(NPC);
                }
                return false;
            }

            if (NPC.ai[1] == 0)
            {
                Vector2 targetP = owner.Center + new Vector2(0, 50 * NPC.ai[3]).RotatedBy(MathHelper.ToRadians((int)NPC.ai[2]));
                NPC.velocity = (targetP - NPC.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(targetP) / 30f;
                NPC.ai[2] += 0.01f;
                if (NPC.ai[2] - (int)NPC.ai[2] >= 0.6f && DLCUtils.HostCheck)
                {
                    NPC.ai[2] = Main.rand.Next(0, 360);
                    NPC.ai[3] = Main.rand.NextFloat(3, 4f);
                    if (Main.rand.NextBool(3))
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 4.5f, ModContent.ProjectileType<BrimstoneBarrage>(), NPC.damage, 0);
                    }
                    NetSync(NPC);
                }
                
            }
            if (NPC.ai[1] == 1)
            {
                NPC.ai[2]++;
                if (NPC.ai[2] == 200)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                    NPC.ai[2] = 0;
                    NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                }
                
                NPC.velocity *= 0.98f;
            }
            
            return false;
        }
        public override void OnKill()
        {
            if (NPC.ai[1] >= 0)
            {
                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X + 20, (int)NPC.Center.Y + 20, ModContent.NPCType<Brimling>(), 0, NPC.ai[0], -1);
            }
        }
    }
}
