using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Twins
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class EDeathRetinazer : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Retinazer);
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);
        }
        public bool DashAttack = false;
        public int dashTime = 150;
        public int timer = 0;
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            
            if (DashAttack)
            {
                Player target = Main.player[npc.target];
                timer++;
                if (timer < 60)
                {
                    int side = 1;
                    if (target.Center.X > npc.Center.X) side = -1;
                    npc.velocity = Vector2.Lerp(npc.velocity, (target.Center + new Vector2(400 * side, -100) - npc.Center).SafeNormalize(Vector2.Zero) * 17, 0.03f);
                    npc.rotation = npc.AngleTo(target.Center) - MathHelper.PiOver2;
                }
                if (timer == 60)
                {
                    npc.velocity = (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 22;
                }
                if (timer > 60 && timer % 20 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item33, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<ScavengerLaser>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
                }
                if (timer >= dashTime)
                {
                    npc.velocity = Vector2.Zero;
                    timer = 0;
                    DashAttack = false;
                }
                return false;
            }
            //Main.NewText(npc.ai[2]);
            if (npc.ai[0] == 3 && npc.ai[1] == 1 && npc.ai[2] == 178)
            {
                DashAttack = true;
                
            }
            return base.SafePreAI(npc);
        }

    }
}
