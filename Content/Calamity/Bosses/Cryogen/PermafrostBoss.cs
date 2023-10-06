using FargowiltasCrossmod.Core;
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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core.Calamity;
using Terraria.GameContent.Bestiary;
using System.Threading;
using CalamityMod;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    public class PermafrostBoss : ModNPC
    {
        public override string Texture => "CalamityMod/NPCs/TownNPCs/DILF";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            NPC.width = 100;
            NPC.height = 100;
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0;
            NPC.HitSound = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3);
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            ref float shieldRot = ref NPC.localAI[0];
            shieldRot += 0.05f;
            Asset<Texture2D> t = TextureAssets.Npc[Type];
            Asset<Texture2D> encasement = ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/GlacialEmbraceBody");
            Asset<Texture2D> shield = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/IceClasperSummonProjectile");
            spriteBatch.Draw(t.Value, NPC.Center - screenPos, new Rectangle(6, 62, 32, 48), drawColor, NPC.rotation, new Vector2(16, 24), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            if (Phase == 0)
            {
                spriteBatch.Draw(encasement.Value, NPC.Center - screenPos + new Vector2(Main.rand.NextFloat(-Timer, Timer), Main.rand.NextFloat(-Timer, Timer)), null, drawColor * 0.7f, NPC.rotation, encasement.Size()/2, NPC.scale, SpriteEffects.None, 0);
            }
            for (int i = 0; i < 15; i++)
            {
                float rotation = MathHelper.ToRadians(360f / 15 * i) + shieldRot;
                if (Phase == 0.5f)
                {
                    float x = (Timer / 120);
                    spriteBatch.Draw(shield.Value, NPC.Center - screenPos + new Vector2(MathHelper.Lerp(1500, 60, (float)Math.Sin((x * Math.PI) / 2)), 0).RotatedBy(rotation), null, drawColor * 0.7f, rotation + MathHelper.PiOver2, shield.Size() / 2, NPC.scale *0.75f, SpriteEffects.None, 0);
                }
                if (Phase > 0.5f)
                {
                    spriteBatch.Draw(shield.Value, NPC.Center - screenPos + new Vector2(60, 0).RotatedBy(rotation), null, drawColor * 0.7f, rotation + MathHelper.PiOver2, shield.Size() / 2, NPC.scale * 0.75f, SpriteEffects.None, 0);
                }
            }
            return false;
        }
        protected static void NetSync(NPC npc, bool onlySendFromServer = true)
        {
            if (onlySendFromServer && Main.netMode != NetmodeID.Server)
                return;

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }
        public ref float Phase => ref NPC.ai[0];
        public ref float Timer => ref NPC.ai[1];
        public ref float Attack => ref NPC.ai[2];
        public ref float Data => ref NPC.ai[3];
        public override void AI()
        {
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
                NetSync(NPC);
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += -1;
                return;
            }
            Player target = Main.player[NPC.target];
            if (Phase == 0)
            {
                Timer += 0.01f;
                if (Timer >= 3)
                {
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenShieldBreak"), NPC.Center);
                    for (int i = 0; i < 3; i++)
                    {
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)), ModCompatibility.Calamity.Mod.Find<ModGore>("CryoDeathGore" + Main.rand.Next(2, 4)).Type);
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModCompatibility.Calamity.Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type);
                        Gore.NewGoreDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModCompatibility.Calamity.Mod.Find<ModGore>("CryoShieldGore" + Main.rand.Next(1, 5)).Type);
                    }
                    Timer = 0;
                    Phase = 0.5f;
                }
            }
            if (Phase == 0.5f)
            {
                Timer++;
                if (Timer >= 120)
                {
                    Phase = 1;
                    Timer = 0;
                }
            }
            if (Phase == 1)
            {
                NPC.spriteDirection = NPC.Center.X > target.Center.X ? 1 : -1;
                if (Attack == 0)
                {
                    
                    CornerMovement();
                }
                if (Attack == 1)
                {
                    Attack = 0;
                    return;
                    Timer++;
                    if (Timer >= 120)
                    {
                        Timer = 0;
                        Attack = 0;
                    }
                    NPC.velocity *= 0.98f;
                }
                
            }
            base.AI();
            void CornerMovement(float distance = 500, float maxSpeed = 30, float acceleration = 1.05f, int time = 600, int timeAtPos = 120)
            {
                Timer++;
                if (Timer % timeAtPos == 0)
                {
                    Data++;
                    if (Data >= 4)
                    {
                        Data = 0;
                    }
                }
                Vector2 pos = new Vector2(-distance, -distance).RotatedBy(Data * MathHelper.PiOver2)/2;
                //if (NPC.Distance(target.Center + pos) > 20)
                Movement(target.Center + pos, 100, 30, acceleration);
                if (Timer >= time)
                {
                    Data = 0;
                    Timer = 0;
                    Attack = 1;
                }
            }
        }
        public void Movement(Vector2 position, float slowdownDistance = 100, float maxSpeed = 20, float acceleration = 1.05f)
        {
            
            Vector2 toPos = (position - NPC.Center).SafeNormalize(Vector2.Zero);
            Dust.NewDustPerfect(position, DustID.TerraBlade);
            if (NPC.Distance(position) > slowdownDistance)
            {
                
                NPC.velocity += toPos * 0.5f;
                if (NPC.velocity.Length() > maxSpeed)
                {
                    NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
                }
                Main.NewText(NPC.velocity.AngleBetween(toPos));
                if (NPC.velocity.AngleBetween(toPos) > 3f)
                {
                    NPC.velocity += toPos;
                }
            }
            else if (NPC.velocity.Length() > 1)
            {
                NPC.velocity *= 0.99f;
            }
        }
    }
}
