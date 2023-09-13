using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class AuricSoul : ModNPC
    {
        public override string Texture => "CalamityMod/Items/Materials/YharonSoulFragment";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            };
            NPCDebuffImmunityData debuffdata = new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffdata);
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 26;
            NPC.height = 26;
            NPC.scale = 2;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lifeMax = 80000;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.knockBackResist = 0f;
            NPC.damage = 0;

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Asset<Texture2D> texture = TextureAssets.Npc[Type];
            for (int i = 0; i < 7; i++)
            {
                spriteBatch.Draw(texture.Value, NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition, null, new Color(drawColor.R, drawColor.G, drawColor.B, (int)(200f / (i + 1f))) * (1 - i / 7f), NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            }
            return true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.velocity = new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(20, 50);
            Vector2 offset = new Vector2(0, Main.rand.Next(200, 500)).RotatedBy(NPC.velocity.ToRotation());
            NPC.ai[1] = offset.X;
            NPC.ai[2] = offset.Y;
            NPC.ai[3] = Main.rand.Next(-3, 4);
            if (NPC.ai[3] == 0)
            {
                if (Main.rand.NextBool())
                {
                    NPC.ai[3] = 1;
                }
                else
                {
                    NPC.ai[3] = -1;
                }
            }
        }
        public override void OnKill()
        {
            int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310);
            for (int i = 0; i < 7; i++)
            {
                Player target = Main.player[Main.npc[(int)NPC.ai[0]].target];
                float angle = MathHelper.Pi * 2 / 7f * i + NPC.AngleTo(target.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(10, 0).RotatedBy(angle), ProjectileID.CultistBossLightningOrbArc, damage, 0, Main.myPlayer, angle, Main.rand.Next(-10, 10));
                }

            }
            SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Thunder_0") with { Volume = 0.75f }, NPC.Center);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)NPC.ai[0]];
            Vector2 offset = new Vector2(NPC.ai[1], NPC.ai[2]);
            Vector2 targetPos = offset + owner.Center;
            NPC.velocity = (targetPos - NPC.Center) / 5;
            offset = offset.RotatedBy(MathHelper.ToRadians(NPC.ai[3]));
            NPC.ai[1] = offset.X;
            NPC.ai[2] = offset.Y;
        }
    }
}
