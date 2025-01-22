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
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls;
using ReLogic.Content;
using CalamityMod.Projectiles.Summon;
using Terraria.Audio;
using CalamityMod.Projectiles.Boss;
using Terraria.ID;
using CalamityMod.NPCs.AcidRain;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ASEternity : CalDLCEmodeBehavior
    {
        public static bool Enabled = false;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override int NPCOverrideID => ModContent.NPCType<AquaticScourgeHead>();

    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ASBodyAltEternity : ASBodyEternity
    {
        public override int NPCOverrideID => ModContent.NPCType<AquaticScourgeBodyAlt>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ASBodyEternity : CalDLCEmodeBehavior
    {
        public override bool IsLoadingEnabled(Mod mod) => ASEternity.Enabled;
        public override int NPCOverrideID => ModContent.NPCType<AquaticScourgeBody>();
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!FargoSoulsUtil.IsSummonDamage(projectile) && projectile.damage > 5)
                projectile.damage = (int)Math.Min(projectile.damage - 1, projectile.damage * 0.75);
        }
        public override void UpdateLifeRegen(ref int damage)
        {
            if (NPC.lifeRegen < 0)
            {
                NPC.lifeRegen = (int)Math.Round(NPC.lifeRegen / 4f);
            }
        }
        /*

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
        */
    }
    public class ASPartsEternity : CalDLCEmodeExtraGlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => ASEternity.Enabled;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<AquaticScourgeBody>(), 
            ModContent.NPCType<AquaticScourgeHead>(), 
            ModContent.NPCType<AquaticScourgeBodyAlt>(), 
            ModContent.NPCType<AquaticScourgeTail>());

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() && npc.ai[3] >= 1)
            {
                Asset<Texture2D> t = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/AquaticScourge/SpikelessASBody");
                spriteBatch.Draw(t.Value, npc.Center - screenPos, null, drawColor, npc.rotation, t.Size() / 2, npc.scale, SpriteEffects.None, 0);
                return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            entity.lifeMax = (int)Math.Round(entity.lifeMax * 1.15f);
        }
        public float[] Vars = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int CurAttack = 0;
        public enum Attacks {
            
        };
        
        public void AdvanceAttack(Attacks[] cycle, int startAggro = 220)
        {
            Vars[0]++;
            for (int i = 1; i < Vars.Length; i++)
            {
                Vars[i] = 0;
            }
            Vars[9] = startAggro;
            
            if (Vars[0] >= cycle.Length)
            {
                Vars[0] = 0;
            }
            CurAttack = (int)cycle[(int)Vars[0]];
        }
        public void SlowdownPrep(NPC npc, int time, SoundStyle endSound)
        {
            if (Vars[8] > time)
            {
                Vars[1]++;
                Vars[8] = 0;
                SoundEngine.PlaySound(endSound, npc.Center);
            }
        }
        public override bool SafePreAI(NPC npc)
        {

            if (npc.type == ModContent.NPCType<AquaticScourgeHead>() && npc.ai[0] == 6 && npc.Calamity().newAI[0] >= 1)
            {
                npc.TargetClosest();

                //increases when angle is too different. increases turn rate when gets high enough. decreases over time when angle isnt too different

                //Main.NewText(npc.GetLifePercent());

                Attacks[] attackCycle = {
                    
                };



                ref float aggroTimer = ref Vars[9];
                ref float attackTimer = ref Vars[8];
                int att = CurAttack;
                
                

                //Main.NewText(Vars[9]);
                SoundStyle DSroar = new("CalamityMod/Sounds/Custom/DesertScourgeRoar");
                SoundStyle Mroar = Mauler.RoarSound;




                Player target = null;
                if (npc.HasValidTarget)
                {
                    

                    target = Main.player[npc.target];
                    Vector2 targetPos = target.Center;

                    Follow(10, 1);


                    void Dash(float speed, float time)
                    {

                    }
                    //Follow the targetPos variable
                    void Follow(float maxSpeed, float turnSpeed, float acceleration = 1.05f) {
                        
                        if (Vars[9] > 160 && npc.Distance(target.Center) > 500)
                        {
                            maxSpeed = 15;
                        }
                        //begin charge cycle when close to the player
                        if (npc.Distance(target.Center) < 700 && Vars[9] < 150)
                        {
                            Vars[1]++;
                            Vars[8] = 0;
                        }

                        //speed up if too slow and vice versa
                        if (npc.velocity.Length() < maxSpeed)
                        {
                            npc.velocity *= acceleration;
                        }
                        else
                        {
                            npc.velocity /= acceleration;
                        }

                        //difference between angle of npc and the angle to the player
                        float angle = MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(npc.velocity, npc.AngleTo(targetPos).ToRotationVector2()));

                        //increase aggression if facing the complete wrong direction and vice versa
                        if (Math.Abs(angle) > 80)
                        {
                            Vars[9]++;
                        }
                        else if (Vars[9] > 1)
                        {
                            Vars[9]--;
                        }
                        //turning to face the player
                        if (Math.Abs(angle) > 2f )
                        {

                            //if aggro timer is up and facing the wrong way turn faster
                            if (Math.Abs(angle) > 80)
                            {
                                if (Vars[9] > 60)
                                {
                                    turnSpeed *= 2;
                                }
                                if (Vars[9] > 200)
                                {
                                    turnSpeed *= 2;
                                }
                            }
                            npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(angle > 0 ? turnSpeed : -turnSpeed));

                        }
                    }
                    //Charge

                }
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                return false;
            }
            //stops body segments from shooting spikes randomly from base calamity code
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>() || npc.type == ModContent.NPCType<AquaticScourgeTail>())
            {

                npc.localAI[0] = 0;
               
            }
            return base.SafePreAI(npc);
        }
        
    }
}
