using CalamityMod;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
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
        public override void SetDefaults()
        {
            NPC.damage = WorldSavingSystem.MasochistModeReal ? 80 : 65;
            base.SetDefaults();
        }
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
        Vector2 Aim = Vector2.Zero;
        public override bool PreAI()
        {
            
            NPC.TargetClosest();
            if (!NPC.HasValidTarget || !CalDLCConfig.Instance.EternityPriorityOverRev || !CalDLCWorldSavingSystem.EternityRev)
                return false;
            
            Player target = Main.player[NPC.target];

            if (WorldSavingSystem.MasochistModeReal)
                NPC.dontTakeDamage = true;
            
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

            Vector2 targetP = target.Center + new Vector2(0, 50 * NPC.ai[3]).RotatedBy(MathHelper.ToRadians((int)NPC.ai[2]));
            int distance = 550;
            Vector2 ownerToMe = owner.DirectionTo(NPC.Center);
            int side = -Math.Sign(FargoSoulsUtil.RotationDifference(ownerToMe, owner.DirectionTo(target.Center)));
            targetP += ownerToMe.RotatedBy(side * MathHelper.PiOver2) * distance;
            NPC.velocity = (targetP - NPC.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(targetP) / 160f;

            NPC.ai[2] += 0.01f;
            if (NPC.Opacity < 0.9f)
                return false;
            Vector2 shootPos = NPC.Center + Vector2.UnitX * NPC.spriteDirection * NPC.width / 2;
            Vector2 predictiveAim = CalamityUtils.CalculatePredictiveAimToTarget(shootPos, target, 4.5f);
            Aim = Vector2.Lerp(Aim, predictiveAim, 0.03f);
            if (NPC.ai[2] - (int)NPC.ai[2] >= 0.4f && DLCUtils.HostCheck) // telegraph
            {
                
                Particle p = new SparkParticle(shootPos + Main.rand.NextVector2Circular(4, 4), Aim, Color.DarkRed, Main.rand.NextFloat(0.25f, 0.5f), 10);
                p.Spawn();
            }
            if (NPC.ai[2] - (int)NPC.ai[2] >= 0.6f)
            {
                
                if (DLCUtils.HostCheck)
                {
                    NPC.ai[2] = Main.rand.Next(0, 360);
                    NPC.ai[3] = Main.rand.NextFloat(3, 4f);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), shootPos, Aim, ModContent.ProjectileType<BrimstoneBarrage>(), NPC.damage, 0);
                }
                SoundEngine.PlaySound(SoundID.Item20 with { Pitch = 0.3f, Volume = 0.8f }, NPC.Center);
                NetSync(NPC);
            }

            return false;
        }
        public override void OnKill()
        {
            if (NPC.ai[1] >= 0 && WorldSavingSystem.MasochistModeReal)
            {
                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X + 20, (int)NPC.Center.Y + 20, ModContent.NPCType<Brimling>(), 0, NPC.ai[0], -1);
            }
            int ownerID = (int)NPC.ai[0];
            if (ownerID.IsWithinBounds(Main.maxNPCs) && Main.npc[ownerID] is NPC owner && owner.TypeAlive<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>())
            {
                owner.SimpleStrikeNPC(NPC.lifeMax, 1);
                if (owner.HitSound.HasValue)
                    SoundEngine.PlaySound(owner.HitSound.Value with { Pitch = -0.5f }, owner.Center);
            }
        }
    }
}
