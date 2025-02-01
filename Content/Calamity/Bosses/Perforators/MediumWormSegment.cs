using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.Perforator;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public abstract class MediumWormSegment : ModNPC
    {
        public abstract int Segment { get; }
        public string SegmentTypeName => Segment switch
        {
            0 => "Head",
            1 => "Body",
            _ => "Tail"
        };
        public int SegmentType => Segment switch
        {
            0 => ModContent.NPCType<PerforatorHeadMedium>(),
            1 => ModContent.NPCType<PerforatorBodyMedium>(),
            _ => ModContent.NPCType<PerforatorTailMedium>()
        };
        public override string Texture => $"CalamityMod/NPCs/Perforator/Perforator{SegmentTypeName}Medium";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(SegmentType);
            NPC.Opacity = 1f;
            NPC.lifeMax /= 4;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 60, true);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = ModContent.Request<Texture2D>($"CalamityMod/NPCs/Perforator/Perforator{SegmentTypeName}Medium").Value;
            Vector2 halfSizeTexture = new((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height * NPC.scale / 2f);
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>($"CalamityMod/NPCs/Perforator/Perforator{SegmentTypeName}MediumGlow").Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void AI()
        {
            NPC.velocity.Y += 0.15f;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                NPC.StrikeInstantKill();
                return;
            }
        }
    }
    public class MediumWormHeadSegment : MediumWormSegment
    {
        public override int Segment => 0;
    }
    public class MediumWormBodySegment : MediumWormSegment
    {
        public override int Segment => 1;
    }
    public class MediumWormTailSegment : MediumWormSegment
    {
        public override int Segment => 2;
    }
}
